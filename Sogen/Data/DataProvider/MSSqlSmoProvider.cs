using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using Smo = Microsoft.SqlServer.Management.Smo;
using Sogen.Data.DataProvider;
using Sogen.Generator.Configuration;
using Sogen.Data.MetaData;
using Sogen.Common;

namespace Sogen.Data.DataProvider {
	internal partial class MSSqlSmoProvider : ProviderBase {

		private Smo.Server server;
		private Smo.Database db;

		public MSSqlSmoProvider(BLToolkitConfiguration config)
			: base(config) {
			try {

				if (config.MSSqlConfig.WindowsAuthentication)
					this.server = new Smo.Server(config.MSSqlConfig.Server);
				else {
					ServerConnection connection = new ServerConnection(config.MSSqlConfig.Server, config.MSSqlConfig.Username, config.MSSqlConfig.Password);
					this.server = new Smo.Server(connection);
				}
				this.db = this.server.Databases[config.MSSqlConfig.Database];


				base.AddMessage("Connected to db Successfully");
			} catch (Exception) {
				throw new Exception("Can't Connect to DataBase");
			}
		}

		public override DB GetMetaData() {

			DB result = new DB();

			foreach (string s in base.Config.MSSqlConfig.Schemas) {
				var tmp = string.Empty;
				try {
					tmp = this.db.Schemas[s].Name;
					this.AddMessage("* Schema {0}", s);
				} catch (Exception) {
					this.AddMessage("* Schema {0} : Can't Find!", s);
					continue;
				}
				var schema = new MetaData.Schema();
				schema.SchemaName = tmp;
				schema.Properties = SMOGetExtendedProperties(this.db.Schemas[s]);
				schema.Description = GetDescription(schema.Namespace, schema.Properties);
				schema.Tables = GetSchemaTables(schema);

				result.Schemas.Add(schema.SchemaName, schema);
			}
			AddForeignKeys(ref result);
			return result;
		}

		public override string ConnectionString {
			get {
				if (base.Config.MSSqlConfig.WindowsAuthentication)
					return string.Format("Data Source={0};Database={1};Integrated Security=SSPI;",
												base.Config.MSSqlConfig.Server,
												base.Config.MSSqlConfig.Database);
				else
					return string.Format("Data Source={0};Database={1};uid={2};pwd={3};",
												base.Config.MSSqlConfig.Server.Replace("\\", "\\\\"),
												base.Config.MSSqlConfig.Database,
												base.Config.MSSqlConfig.Username,
												base.Config.MSSqlConfig.Password);
			}
		}

		private Dictionary<string, MetaData.Table> GetSchemaTables(MetaData.Schema schema) {
			var result = new Dictionary<string, MetaData.Table>();

			int index = 1;
			foreach (Smo.Table t in this.db.Tables) {
				if (t.Schema.ToLower() != schema.SchemaName.ToLower())
					continue;
				MetaData.Table table = GetTable(schema, t);
				table.ID = index++;
				result.Add(table.TableName, table);
			}
			if (base.Config.MSSqlConfig.GenerateObjectForViews)
				foreach (Smo.View v in this.db.Views) {
					if (v.Schema.ToLower() != schema.SchemaName.ToLower())
						continue;
					MetaData.Table table = GetTable(schema, v);
					table.ID = index++;
					result.Add(table.TableName, table);
				}

			return result;
		}

		private MetaData.Table GetTable(MetaData.Schema schema, Smo.View v) {
			var result = new MetaData.Table(schema);
			this.AddMessage("* * View {0}", v.Name);
			result.TableName = v.Name;
			result.Properties = SMOGetExtendedProperties(v);
			result.Description = GetDescription(result.ClassName, result.Properties);
			result.IsView = true;
			result.Columns = GetColumns(result, v.Columns);
			
			return result;

			
		}

		private MetaData.Table GetTable(MetaData.Schema schema, Smo.Table t) {
			var result = new MetaData.Table(schema);
			this.AddMessage("* * Table {0}", t.Name);
			result.TableName = t.Name;
			result.Properties = SMOGetExtendedProperties(t);
			result.Description = GetDescription(result.ClassName, result.Properties);
			result.IsView = false;
			result.Columns = GetColumns(result, t.Columns);
			result.UniqueKeys = GetUniqueKeys(result, t);
			// Foreign Key for next Level

			return result;
		}

		private Dictionary<string, MetaData.UniqueKey> GetUniqueKeys(MetaData.Table table, Smo.Table t) {
			var result = new Dictionary<string, MetaData.UniqueKey>();
			int index = 1;

			this.AddMessage("* * * Unique Keys");
			// Unique Keys & Primary key
			var ukeys = SMOGetIndexs(t, Smo.IndexKeyType.DriPrimaryKey);
			ukeys.AddRange(SMOGetIndexs(t, Smo.IndexKeyType.DriUniqueKey));
			foreach (Smo.Index i in ukeys) {
				bool invalidCol = false;
				var uk = new MetaData.UniqueKey(table);
				int counter = 1;
				foreach (Smo.IndexedColumn smoIcol in i.IndexedColumns) {
					MetaData.Column col = SMOGetIndexColumn(table, smoIcol);
					if (col == null) {
						invalidCol = true;
						break;
					}
					col.ID = counter++;
					uk.Columns.Add(col.ColumnName, col);
				}
				if (invalidCol)
					continue;
				uk.UKName = i.Name;
				uk.KeyName = Helper.GetColumnListName(uk.Columns);
				uk.Properties = SMOGetExtendedProperties(i);
				uk.Description = GetDescription(uk.KeyName, uk.Properties);
				switch (i.IndexKeyType) {
					case Microsoft.SqlServer.Management.Smo.IndexKeyType.DriPrimaryKey:
						uk.Type = MetaData.MetaDataEnums.UniqueKeyType.Primary;
						break;
					case Microsoft.SqlServer.Management.Smo.IndexKeyType.DriUniqueKey:
						uk.Type = MetaData.MetaDataEnums.UniqueKeyType.UniqueKey;
						break;
				}

				uk.ID = index++;
				result.Add(uk.UKName, uk);
			}


			//Unique Identifier
			var uiCols = SMOGetUniqueIdentifierColumns(t);
			for (int i = 0; i < uiCols.Count; i++) {
				var uk = new MetaData.UniqueKey(table);
				MetaData.Column col = SMOGetColumn(table, uiCols[i]);
				col.ID = 1;
				uk.Columns.Add(col.ColumnName, col);
				uk.KeyName = Helper.GetColumnListName(uk.Columns);
				bool addedPast = false;
				foreach (MetaData.UniqueKey item in result.Values) 
					if (uk.KeyName == item.KeyName) {
						addedPast = true;
						break;
					}
				if (addedPast)
					continue;
				uk.UKName = string.Format("UK_{0}", col.ColumnName);
				uk.Properties = SMOGetExtendedProperties(uiCols[i]);
				uk.Description = GetDescription(uk.KeyName, uk.Properties);
				uk.Type = MetaData.MetaDataEnums.UniqueKeyType.UniqueIdentifier;
				uk.ID = index++;
				try {
					result.Add(uk.UKName, uk);
				} catch (Exception) { }
			}

			return result;
		}

		private Dictionary<string, MetaData.Column> GetColumns(MetaData.Table table, Smo.ColumnCollection columnCollection) {
			var result = new Dictionary<string, MetaData.Column>();
			this.AddMessage("* * * Columns");
			int index = 1;
			foreach (Smo.Column c in columnCollection) {
				var column = SMOGetColumn(table, c);
				column.ID = index++;
				result.Add(column.ColumnName, column);
			}
			return result;
		}

		private Dictionary<string, MetaData.ForeignKey> GetForeignKeys(Smo.Table table, ref DB metaData) {
			var result = new Dictionary<string, MetaData.ForeignKey>();
			this.AddMessage("* * * {0}", table.Name);
			int index = 1;
			foreach (Smo.ForeignKey f in table.ForeignKeys) {
				MetaData.ForeignKey fk = SMOGetForeignKey(f, ref metaData);
				if (fk == null)
					continue;
				fk.ID = index++;
				result.Add(fk.FkName, fk);
			}
			return result;
		}

		private void AddForeignKeys(ref DB metaData) {
			this.AddMessage("* * Foreign Keys");
			foreach (Smo.Table table in db.Tables) {
				//	try {
				if (!metaData.Schemas.ContainsKey(table.Schema))
					continue;
				metaData.Schemas[table.Schema].Tables[table.Name].ForeignKeys = GetForeignKeys(table, ref metaData);
				//    }
				//   catch (Exception ex) { }
			}
		}

		#region Helper:Smo

		public static MetaData.DataType SMOGetDataType(Smo.DataType smoDataType) {

			var result = new MetaData.DataType();
			result.Type = smoDataType.Name;
			switch (smoDataType.SqlDataType) {
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Image:
					result.CSharpType = "byte[]";
					result.CSharpNullableType = "byte?[]";
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.Image;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Text:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.String;
					result.SqlDbType = SqlDbType.Text; break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Binary:
					result.CSharpType = "byte[]";
					result.CSharpNullableType = "byte?[]";
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.Binary;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.TinyInt:
					result.CSharpType = "byte";
					result.CSharpNullableType = "byte?";
					result.DbType = DbType.Byte;
					result.SqlDbType = SqlDbType.TinyInt;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Date:
					result.CSharpType = "DateTime";
					result.CSharpNullableType = "DateTime?";
					result.DbType = DbType.Date;
					result.SqlDbType = SqlDbType.Date;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Time:
					result.CSharpType = "DateTime";
					result.CSharpNullableType = "DateTime?";
					result.DbType = DbType.Time;
					result.SqlDbType = SqlDbType.Time;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Bit:
					result.CSharpType = "bool";
					result.CSharpNullableType = "bool?";
					result.DbType = DbType.Boolean;
					result.SqlDbType = SqlDbType.Bit;

					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.SmallInt:
					result.CSharpType = "short";
					result.CSharpNullableType = "short?";
					result.DbType = DbType.Int16;
					result.SqlDbType = SqlDbType.SmallInt;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Decimal:
					result.CSharpType = "decimal";
					result.CSharpNullableType = "decimal?";
					result.DbType = DbType.Decimal;
					result.SqlDbType = SqlDbType.Decimal;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Int:
					result.CSharpType = "int";
					result.CSharpNullableType = "int?";
					result.DbType = DbType.Int32;
					result.SqlDbType = SqlDbType.Int;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.SmallDateTime:
					result.CSharpType = "DateTime";
					result.CSharpNullableType = "DateTime?";
					result.DbType = DbType.DateTime;
					result.SqlDbType = SqlDbType.SmallDateTime;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Real:
					result.CSharpType = "float";
					result.CSharpNullableType = "float?";
					result.DbType = DbType.Single;
					result.SqlDbType = SqlDbType.Real;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Money:
					result.CSharpType = "decimal";
					result.CSharpNullableType = "decimal?";
					result.DbType = DbType.Currency;
					result.SqlDbType = SqlDbType.Money;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime:
					result.CSharpType = "DateTime";
					result.CSharpNullableType = "DateTime?";
					result.DbType = DbType.DateTime;
					result.SqlDbType = SqlDbType.DateTime;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Float:
					result.CSharpType = "double";
					result.CSharpNullableType = "double?";
					result.DbType = DbType.Double;
					result.SqlDbType = SqlDbType.Float;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Numeric:
					result.CSharpType = "decimal";
					result.CSharpNullableType = "decimal?";
					result.DbType = DbType.Decimal;
					result.SqlDbType = SqlDbType.Decimal;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.SmallMoney:
					result.CSharpType = "decimal";
					result.CSharpNullableType = "decimal?";
					result.DbType = DbType.Currency;
					result.SqlDbType = SqlDbType.SmallMoney;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.DateTime2:
					result.CSharpType = "DateTime";
					result.CSharpNullableType = "DateTime?";
					result.DbType = DbType.DateTime2;
					result.SqlDbType = SqlDbType.DateTime2;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.BigInt:
					result.CSharpType = "long";
					result.CSharpNullableType = "long?";
					result.DbType = DbType.Int64;
					result.SqlDbType = SqlDbType.BigInt;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinary:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.VarBinaryMax:
					result.CSharpType = "byte[]";
					result.CSharpNullableType = "byte?[]";
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.VarBinary;
					result.Type = "VarBinary";
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Timestamp:
					result.CSharpType = "byte[]";
					result.CSharpNullableType = "byte?[]";
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.Timestamp;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.SysName:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.String;
					result.SqlDbType = SqlDbType.NVarChar;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.NVarChar:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.NVarCharMax:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.String;
					result.SqlDbType = SqlDbType.NVarChar;
					result.Type = "NVarChar";
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.VarChar:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.VarCharMax:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.AnsiString;
					result.SqlDbType = SqlDbType.VarChar;
					result.Type = "VarChar";
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.NText:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.String;
					result.SqlDbType = SqlDbType.NText;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.UniqueIdentifier:
					result.CSharpType = "Guid";
					result.CSharpNullableType = "Guid?";
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.UniqueIdentifier;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.DateTimeOffset:
					result.CSharpType = "DateTimeOffset";
					result.CSharpNullableType = "DateTimeOffset?";
					result.DbType = DbType.DateTimeOffset;
					result.SqlDbType = SqlDbType.DateTimeOffset;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Variant:
					result.CSharpType = "object";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.Binary;
					result.SqlDbType = SqlDbType.Variant;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Xml:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.Xml;
					result.SqlDbType = SqlDbType.Xml;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Char:
					result.CSharpType = smoDataType.MaximumLength == 1 ? "char" : "string";
					result.CSharpNullableType = smoDataType.MaximumLength == 1 ? "char?" : result.CSharpType;
					result.DbType = DbType.AnsiStringFixedLength;
					result.SqlDbType = SqlDbType.Char;
					break;

				case Microsoft.SqlServer.Management.Smo.SqlDataType.NChar:
					result.CSharpType = smoDataType.MaximumLength == 1 ? "char" : "string";
					result.CSharpNullableType = smoDataType.MaximumLength == 1 ? "char?" : result.CSharpType;
					result.DbType = DbType.StringFixedLength;
					result.SqlDbType = SqlDbType.NChar;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Geography:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.Geometry:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.HierarchyId:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.None:
					result.CSharpType = "string";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.String;
					result.SqlDbType = SqlDbType.NVarChar;
					break;
				case Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedTableType:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedDataType:
				case Microsoft.SqlServer.Management.Smo.SqlDataType.UserDefinedType:
					result.CSharpType = "object";
					result.CSharpNullableType = result.CSharpType;
					result.DbType = DbType.Object;
					result.SqlDbType = SqlDbType.Udt;
					break;
			}
			return result;
		}

		public static Smo.Column SMOGetIdentity(Smo.Table table) {
			for (int i = 0; i < table.Columns.Count; i++)
				if (table.Columns[i].Identity)
					return table.Columns[i];
			return null;
		}

		public static List<Smo.Column> SMOGetUniqueIdentifierColumns(Smo.Table table) {
			var result = new List<Smo.Column>();
			for (int i = 0; i < table.Columns.Count; i++) {
				if (table.Columns[i].DataType.SqlDataType == Smo.DataType.UniqueIdentifier.SqlDataType)
					result.Add(table.Columns[i]);
			}
			return result;
		}

		public static string SMOGetDefaultValue(Smo.DefaultConstraint defaultConstraint, Smo.DataType dataType) {
			if (defaultConstraint == null)
				return string.Empty;
			var result = defaultConstraint.Text;
			result = result.Replace("(N", "").Replace("N'", "\"").Replace("'", "\"").Replace("(", "").Replace(")", "").ToLower();
			if (String.IsNullOrEmpty(result))
				return string.Empty;

			switch (dataType.SqlDataType) {
				case Smo.SqlDataType.Bit:
					result = (result == "0") ? "false" : "true";
					break;

				case Smo.SqlDataType.SmallDateTime:
				case Smo.SqlDataType.DateTime:
				case Smo.SqlDataType.DateTime2:
				case Smo.SqlDataType.Date:
					result = result.Replace("\"", "");
					result = (result == "getdate")
								? "DateTime.Now"
								: (result == "getutcdate") ? "DateTime.UtcNow" : string.Format("DateTime.Parse(\"{0}\")", result);
					break;
				case Smo.SqlDataType.UniqueIdentifier:
					result = (result == "newid") ? "Guid.NewGuid()" : string.Format("Guid.Parse({0})", result);
					break;
				case Smo.SqlDataType.Char:
				case Smo.SqlDataType.NChar:
				case Smo.SqlDataType.VarChar:
				case Smo.SqlDataType.NVarChar:
				case Smo.SqlDataType.NVarCharMax:
				case Smo.SqlDataType.Text:
				case Smo.SqlDataType.NText:
				case Smo.SqlDataType.Xml:
					result = (result == "\"\"")
								? "string.Empty"
								: (result.IndexOf('"') < 0) ? string.Format("\"{0}\"", result) : result;
					break;
			}

			return result;
		}

		public static List<Smo.Index> SMOGetIndexs(Smo.Table table, Smo.IndexKeyType type) {
			var result = new List<Smo.Index>();
			for (int i = 0; i < table.Indexes.Count; i++)
				if (table.Indexes[i].IndexKeyType == type) {
					result.Add(table.Indexes[i]);

				}
			return result;
		}

		private MetaData.Column SMOGetColumn(MetaData.Table table, Smo.Column c) {
			var result = new MetaData.Column(table);
			result.ColumnName = c.Name;
			result.IsNullable = c.Nullable;
			result.IsIdentity = c.Identity;
			result.IsPrimaryKey = c.InPrimaryKey;
			result.DefaultValue = SMOGetDefaultValue(c.DefaultConstraint, c.DataType);
			result.Properties = SMOGetExtendedProperties(c);
			result.Description = GetDescription((table.IsView)? result.PropertyName:string.Empty, result.Properties);
			result.Type = SMOGetDataType(c.DataType);

			return result;
		}

		private MetaData.Column SMOGetIndexColumn(MetaData.Table table, Smo.IndexedColumn c) {
			return SMOGetColumn(table, SMOOrginalColumn(c));
		}

		private static Smo.Column SMOOrginalColumn(Smo.IndexedColumn iCol) {
			if (iCol.IsComputed)
				return null;
			Smo.Table tb = (Smo.Table)iCol.Parent.Parent;

			for (int i = 0; i < tb.Columns.Count; i++)
				if (tb.Columns[i].Name == iCol.Name)
					return tb.Columns[i];

			return null;
		}

		private Dictionary<string, string> SMOGetExtendedProperties(Smo.IExtendedProperties extendedProperties) {
			var result = new Dictionary<string, string>();
			foreach (Smo.ExtendedProperty e in extendedProperties.ExtendedProperties) {
				result.Add(e.Name, e.Value.ToString());
			}
			return result;
		}

		private MetaData.ForeignKey SMOGetForeignKey(Smo.ForeignKey f, ref DB metaData) {
			if (!metaData.Schemas.ContainsKey(f.ReferencedTableSchema))
				return null;
			if (!metaData.Schemas[f.ReferencedTableSchema].Tables.ContainsKey(f.ReferencedTable))
				return null;

			var thisTable = metaData.Schemas[f.Parent.Schema].Tables[f.Parent.Name];
			var otherTable = metaData.Schemas[f.ReferencedTableSchema].Tables[f.ReferencedTable];

			var result = new MetaData.ForeignKey(thisTable);

			result.FkName = f.Name;
			result.KeyName = result.KeyName.ToNormalPascal();
			result.Properties = SMOGetExtendedProperties(f);
			result.Description = GetDescription(result.MemberName, result.Properties);
			result.OtherTable = otherTable;
			for (int i = 0; i < f.Columns.Count; i++) {
				var thisColumn = thisTable.Columns[f.Columns[i].Name];
				var otherColumn = otherTable.Columns[f.Columns[i].ReferencedColumn];
				thisColumn.ID = otherColumn.ID = i + 1;
				result.ThisColumns.Add(thisColumn.ColumnName, thisColumn);
				result.OtherColumns.Add(otherColumn.ColumnName, otherColumn);
			}

			bool thisColumnsIsUnique = false;
			bool otherColumnsIsUnique = false;
			foreach (MetaData.UniqueKey uk in thisTable.UniqueKeys.Values)
				if (Helper.GetColumnListName(uk.Columns) == Helper.GetColumnListName(result.ThisColumns)) {
					thisColumnsIsUnique = true;
					break;
				}
			foreach (MetaData.UniqueKey uk in otherTable.UniqueKeys.Values)
				if (Helper.GetColumnListName(uk.Columns) == Helper.GetColumnListName(result.OtherColumns)) {
					otherColumnsIsUnique = true;
					break;
				}

			if (thisColumnsIsUnique & otherColumnsIsUnique)
				result.AssociationType = MetaDataEnums.AssociationType.OneToOne;
			else if (thisColumnsIsUnique & !otherColumnsIsUnique)
				result.AssociationType = MetaDataEnums.AssociationType.OneToMany;
			else if (!thisColumnsIsUnique & otherColumnsIsUnique)
				result.AssociationType = MetaDataEnums.AssociationType.ManyToOne;
			else if (!thisColumnsIsUnique & !otherColumnsIsUnique)
				result.AssociationType = MetaDataEnums.AssociationType.ManyToMany;

			return result;
		}

		#endregion

		#region Helper
		public static string GetDescription(string objName, Dictionary<string, string> properties) {
			try {
				return properties["MS_Description"].ToString();
			} catch {
				return objName.ToDescription();
			}
		}

		#endregion
	}

}

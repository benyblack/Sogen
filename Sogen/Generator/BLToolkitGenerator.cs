using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using Sogen.Common;
using Sogen.Data.DataProvider;
using Sogen.Data;
using Sogen.Data.MetaData;
using Sogen.Writer;


namespace Sogen.Generator {
	public class BLToolkitGenerator {

		#region Properties & Fileds
		public List<string> Messages { get; private set; }
		public List<Validation> Warnings { get; private set; }
		internal ProviderBase Provider { get; private set; }
		internal Configuration.BLToolkitConfiguration Config { get { return this.Provider.Config; } }

		#endregion

		#region Events
		public delegate void OnMessageAddHandler(string lastMessage);
		public event OnMessageAddHandler OnMessageAdd;
		#endregion

		#region Constructors
		private BLToolkitGenerator(ProviderBase provider) {
			this.Provider = provider;
			this.Messages = new List<string>();
			this.Warnings = new List<Validation>();
			this.Provider.OnMessageAdd += new OnMessageAddHandler(Provider_OnMessageAdd);
		}
		#endregion

		#region Private Methods

		private void Generate(Schema schema) {
			Validate(schema);
			this.AddMessage("* Schema {0}", schema.Namespace);
			string rootNamespace = string.Format("{0}.{1}", Config.RootNamespace, schema.Namespace);
			string dataMadoleName = Config.DataModelName;
			var writer = WriterBase.Create(Config.Language);
			writer.Add(GetHeader());
			writer.AddUsing("System", "System.Collections.Generic", "BLToolkit.Data", "BLToolkit.Data.Linq", "BLToolkit.DataAccess", "BLToolkit.Mapping");
			writer.AddUsing(Config.CustomUsings);
			writer.AddLine();

			writer.AddBeginNamespace(rootNamespace);
			writer.AddBeginClass(dataMadoleName, Config.DataModelAccessModifier, false, true, "DbManager");
			foreach (Table table in schema.Tables.Values) {
				writer.AddPropertyReadOnly(table.ClassName,
											string.Format("Table<{0}>", table.ClassName),
											string.Format("GetTable<{0}>()", table.ClassName),
											table.Description);
			}
			writer.AddLine();
			writer.AddFormatLine("public {0}() : base ( new BLToolkit.Data.DataProvider.{1}(), {2}.Configs.{3}ConnStr) {{ }}",
							dataMadoleName,
							Provider.BLToolkitDataProvider,
							Config.RootNamespace,
							schema.Namespace);
			writer.AddEndClass(dataMadoleName);

			foreach (Table table in schema.Tables.Values)
				Generate(table, ref writer);
			if (schema.Enums.Count > 0)
				Generate(schema.Enums,schema.Namespace, ref writer);
			writer.AddEndNamespace(rootNamespace);

			string filename = string.Format("{0}{1}.generated.cs", GetExportFolder(), schema.Namespace);
			if (File.Exists(filename))
				File.Delete(filename);
			File.WriteAllText(filename, writer.Text, System.Text.Encoding.UTF8);
			this.AddMessage("* * Create File {0}", filename);

			if (!Config.GenerateEmptyPartialClass)
				return;
			string schemaFolder = string.Format("{0}{1}\\", GetExportFolder(), schema.Namespace);
			if (!Directory.Exists(schemaFolder))
				Directory.CreateDirectory(schemaFolder);
			foreach (Table table in schema.Tables.Values) {
				string classFilename = string.Format("{0}{1}.cs", schemaFolder, table.ClassName);
				if (File.Exists(classFilename))
					continue;
				writer.clear();
				writer.AddUsing("System", "System.Collections.Generic", "BLToolkit.Data", "BLToolkit.Data.Linq", "BLToolkit.DataAccess", "BLToolkit.Mapping");
				writer.AddUsing(Config.CustomUsings);
				writer.AddLine();
				writer.AddBeginNamespace(rootNamespace);
				writer.AddXmlComment(table.Description);
				writer.AddBeginClass(table.ClassName, "public", false, true);
				writer.AddLine();
				writer.AddEndClass(table.ClassName);
				writer.AddEndNamespace(rootNamespace);
				File.WriteAllText(classFilename, writer.Text, System.Text.Encoding.UTF8);
				this.AddMessage("* * Create File {0}", classFilename);
			}

		}


		private void Generate(Table table, ref WriterBase writer) {
			Validate(table);
			this.AddMessage("* * * {0}", table.ClassName);
			List<string> attributes = new List<string>();
			attributes.Add(string.Format("TableName(Owner = \"{0}\", Name = \"{1}\")", table.Parent.SchemaName, table.TableName));

			foreach (string attr in table.Attributes) {
				attributes.Add(attr.Replace("{namespace}", Config.RootNamespace)
									.Replace("{schema}", table.Parent.Namespace)
									.Replace("{table}", table.ClassName)
									.Replace("{name}", table.ClassName)
									.Replace("{type}", table.ClassName)
									.Replace("{type?}", table.ClassName));
			}

			writer.AddLine();
			writer.AddBeginRegion(table.ClassName);
			writer.AddXmlComment(table.Description);
			writer.AddAttribute(attributes.ToArray());
			writer.AddBeginClass(table.ClassName, "public", false, true);

			Dictionary<string, Column> primary = new Dictionary<string, Column>();
			foreach (UniqueKey uk in table.UniqueKeys.Values) {
				if (uk.Type == MetaDataEnums.UniqueKeyType.Primary)
					primary = uk.Columns;
			}
			this.AddMessage("* * * * Columns");
			foreach (Column column in table.Columns.Values)
				Generate(column, primary, ref writer);
			this.AddMessage("* * * * Foreign Keys");
			foreach (ForeignKey fk in table.ForeignKeys.Values)
				Generate(fk, ref writer);
			if (Config.GenerateInsertUpdateDelete && !table.IsView)
				writer.Add(GenerateInsertUpdateDelete(table));
			if (Config.GenerateGetMethods && !table.IsView)
				writer.Add(GenerateGetMethods(table));

			writer.AddEndClass(table.ClassName);
			writer.AddEndRegion(table.ClassName);

		}

		private string GenerateGetMethods(Table table) {
			var writer = WriterBase.Create(Config.Language);
			// Get
			foreach (UniqueKey uk in table.UniqueKeys.Values) {
				Validate(uk);
				writer.AddLine();
				writer.AddXmlComment(string.Format("Get a {0} from db", table.ClassName));
				writer.AddFormatLine("public static {0}.{1} Get{2} (", table.Parent.Namespace, table.ClassName,
					(uk.Type == MetaDataEnums.UniqueKeyType.Primary) ? "" : string.Format("By{0}", uk.MemberName)
																		).pushIndent();
				writer.Add(Helper.GetColumnList(uk.Columns, "{type} {camelprop}", ", \r\n", false, false)).AddLine(" ) {");
				writer.AddFormatLine("var {0} = new {1}.{2}();", table.ClassName.ToCamel(), table.Parent.Namespace, table.ClassName);
				writer.Add(Helper.GetColumnList(uk.Columns,
					   string.Format("{0}.{{property}} = {{camelprop}};", table.ClassName.ToCamel()),
					  " \r\n", false, false));
				writer.AddLine();
				writer.AddFormatLine("using ({0}.{1} db = new {0}.{1}()) {{", table.Parent.SchemaName, Config.DataModelName);
				writer.pushIndent();
				writer.AddLine("return db.SetCommand(@\"").pushIndent();
				writer.AddFormatLine("Select * From [{0}].[{1}] ", table.Parent.SchemaName, table.TableName);
				writer.AddLine("Where (").pushIndent();
				writer.Add(Helper.GetColumnList(uk.Columns, "{column} = @{column}", " AND \r\n", false, false)).Add(" );").popIndent();
				writer.AddFormatLine("\", db.CreateParameters({0}))", table.ClassName.ToCamel());
				writer.AddFormatLine(".ExecuteObject<{0}.{1}>();", table.Parent.Namespace, table.ClassName);
				writer.popIndent();
				writer.AddLine("}");
				writer.popIndent();
				writer.popIndent();
				writer.AddFormatLine("}} // Get {0}", uk.MemberName);
			}
			return writer.Text;
		}

		private string GenerateInsertUpdateDelete(Table table) {
			var writer = WriterBase.Create(Config.Language);
			bool hasIdentity = (table.IdentityColumn != null);

			// Insert
			writer.AddLine();
			writer.AddXmlComment("Insert this instance to db");
			writer.AddLine("public void Insert() {");
			writer.pushIndent();
			writer.AddFormatLine("using ({0}.{1} db = new {0}.{1}()) {{", table.Parent.SchemaName, Config.DataModelName);
			writer.pushIndent();
			if (hasIdentity)
				writer.AddFormatLine("this.{0} = ", table.IdentityColumn.FiledName).pushIndent();
			writer.AddLine("db.SetCommand(@\"").pushIndent();
			writer.AddFormatLine("Insert into [{0}].[{1}] (", table.Parent.SchemaName, table.TableName).pushIndent();
			writer.Add(Helper.GetColumnList(table.Columns, "{column}", ", ", true, false)).AddLine(" )").popIndent();
			writer.AddLine("values (").pushIndent();
			writer.Add(Helper.GetColumnList(table.Columns, "@{column}", ", ", true, false)).Add(" );").popIndent();
			if (hasIdentity)
				writer.AddLine().AddFormatLine(" SELECT Cast(SCOPE_IDENTITY() as {0}) {1}", table.IdentityColumn.Type.Type, table.IdentityColumn.PropertyName);
			writer.AddLine("\", db.CreateParameters(this))");
			if (hasIdentity)
				writer.AddFormatLine(" .ExecuteScalar<{0}>();", table.IdentityColumn.Type.CSharpType);
			else
				writer.AddLine(".ExecuteNonQuery();");
			writer.popIndent();
			if (hasIdentity)
				writer.popIndent();
			writer.AddLine("}");
			writer.popIndent();
			writer.popIndent();
			writer.AddLine("} // Insert");


			if (table.PrimaryKey == null)
				return writer.Text;
			// Update
			writer.AddLine();
			writer.AddXmlComment("Update this instance in db");
			writer.AddLine("public void Update() {");
			writer.pushIndent();
			writer.AddFormatLine("using ({0}.{1} db = new {0}.{1}()) {{", table.Parent.SchemaName, Config.DataModelName);
			writer.pushIndent();
			writer.AddLine("db.SetCommand(@\"").pushIndent();
			writer.AddFormatLine("Update [{0}].[{1}]  set ", table.Parent.SchemaName, table.TableName).pushIndent();
			writer.Add(Helper.GetColumnList(table.Columns, "{column} = @{column}", ", \r\n", true, false)).AddLine().popIndent();
			writer.AddLine(" Where (").pushIndent();
			writer.Add(Helper.GetColumnList(table.PrimaryKey.Columns, "{column} = @{column}", " AND \r\n", false, false)).Add(" );").popIndent();
			writer.AddLine("\", db.CreateParameters(this))");
			writer.popIndent();
			writer.AddLine(".ExecuteNonQuery();");
			writer.AddLine("}");
			writer.popIndent();
			writer.popIndent();
			writer.AddLine("} // Update");

			// Delete
			writer.AddLine();
			writer.AddXmlComment("Delete this instance from db");
			writer.AddLine("public void Delete() {");
			writer.pushIndent();
			writer.AddFormatLine("using ({0}.{1} db = new {0}.{1}()) {{", table.Parent.SchemaName, Config.DataModelName);
			writer.pushIndent();
			writer.AddLine("db.SetCommand(@\"").pushIndent();
			writer.AddFormatLine("Delete [{0}].[{1}] ", table.Parent.SchemaName, table.TableName);
			writer.AddLine(" Where (").pushIndent();
			writer.Add(Helper.GetColumnList(table.PrimaryKey.Columns, "{column} = @{column}", " AND \r\n", false, false)).Add(" );").popIndent();
			writer.AddLine("\", db.CreateParameters(this))");
			writer.popIndent();
			writer.AddLine(".ExecuteNonQuery();");
			writer.AddLine("}");
			writer.popIndent();
			writer.popIndent();
			writer.AddLine("} // Delete");

			// Static Delete
			writer.AddLine();
			writer.AddXmlComment(string.Format("Delete a {0} from db", table.ClassName));
			writer.AddLine("public static void Delete (").pushIndent();
			writer.Add(Helper.GetColumnList(table.PrimaryKey.Columns, "{type} {camelprop}", ", \r\n", false, false)).AddLine(" ) {");
			writer.AddFormatLine("var {0} = new {1}.{2}();", table.ClassName.ToCamel(), table.Parent.Namespace, table.ClassName);
			writer.Add(Helper.GetColumnList(table.PrimaryKey.Columns,
				   string.Format("{0}.{{property}} = {{camelprop}};", table.ClassName.ToCamel()),
				  " \r\n", false, false));
			writer.AddLine();
			writer.AddFormatLine("using ({0}.{1} db = new {0}.{1}()) {{", table.Parent.SchemaName, Config.DataModelName);
			writer.pushIndent();
			writer.AddLine("db.SetCommand(@\"").pushIndent();
			writer.AddFormatLine("Delete [{0}].[{1}] ", table.Parent.SchemaName, table.TableName);
			writer.AddLine("Where (").pushIndent();
			writer.Add(Helper.GetColumnList(table.PrimaryKey.Columns, "{column} = @{column}", " AND \r\n", false, false)).Add(" );").popIndent();
			writer.AddFormatLine("\", db.CreateParameters({0}))", table.ClassName.ToCamel());
			writer.AddLine(".ExecuteNonQuery();");
			writer.popIndent();
			writer.AddLine("}");
			writer.popIndent();
			writer.popIndent();
			writer.AddLine("} // Static Delete");


			return writer.Text;
		}

		private void Generate(Column column, Dictionary<string, Column> primary, ref WriterBase writer) {
			Validate(column);
			// Make Attributes
			List<string> attributes = new List<string>();

			attributes.Add(string.Format("MapField(\"{0}\")", column.ColumnName));
			if (column.IsIdentity)
				attributes.Add("Identity");
			if (column.IsNullable)
				attributes.Add("Nullable");
			if (column.IsPrimaryKey)
				attributes.Add(string.Format("PrimaryKey({0})", primary[column.ColumnName].ID));

			foreach (string attr in column.Attributes) {
				attributes.Add(attr.Replace("{namespace}", Config.RootNamespace)
									.Replace("{schema}", column.Parent.Parent.Namespace)
									.Replace("{table}", column.Parent.ClassName)
									.Replace("{name}", column.PropertyName)
									.Replace("{type}", column.Type.CSharpType)
									.Replace("{type?}", column.GetType));
			}

			//
			writer.AddLine();
			writer.AddPropertyWithFiled(column.PropertyName,
										column.GetType,
										column.Description,
										column.DefaultValue,
										attributes.ToArray());
		}

		private void Generate(ForeignKey fk, ref WriterBase writer) {
			if (!fk.IsBackReference)
				Validate(fk);
			List<string> attributes = new List<string>();

			string thisKey = string.Empty;
			foreach (Column column in fk.ThisColumns.Values) {
				thisKey += string.Format("{0}, ", column.ColumnName);
			}
			thisKey = thisKey.Remove(thisKey.Length - 2, 2);

			string otherKey = string.Empty;
			foreach (Column column in fk.OtherColumns.Values) {
				otherKey += string.Format("{0}, ", column.ColumnName);
			}
			otherKey = otherKey.Remove(otherKey.Length - 2, 2);

			attributes.Add(string.Format("Association(ThisKey = \"{0}\", OtherKey = \"{1}\")", thisKey, otherKey));

			string type = string.Format("{0}.{1}.{2}", Config.RootNamespace, fk.OtherTable.Parent.Namespace, fk.OtherTable.ClassName);
			if (fk.AssociationType == MetaDataEnums.AssociationType.ManyToMany || fk.AssociationType == MetaDataEnums.AssociationType.OneToMany)
				type = string.Format("List<{0}>", type);

			foreach (string attr in fk.Attributes) {
				attributes.Add(attr.Replace("{namespace}", Config.RootNamespace)
									.Replace("{schema}", fk.Parent.Parent.Namespace)
									.Replace("{table}", fk.Parent.ClassName)
									.Replace("{name}", fk.MemberName)
									.Replace("{type}", type)
									.Replace("{type?}", type));
			}

			writer.AddXmlComment(string.Format("{0} {1}", fk.Description, fk.FkName));
			writer.AddAttribute(attributes.ToArray());


			writer.AddProperty(fk.MemberName, type);
		}

		private void Generate(Dictionary<string, Data.MetaData.Enum> enums, string schemaName, ref WriterBase writer) {
			writer.AddLine();
			writer.AddBeginRegion(string.Format("{0}Enums",schemaName));
			writer.AddBeginClass(string.Format("{0}Enums", schemaName), "public", false, true);
			
			foreach (Data.MetaData.Enum e in enums.Values) {
				writer.AddFormatLine("public enum {0} {{", e.Name).pushIndent();
				foreach (KeyValuePair<string, string> item in e.Values.Values) {
					writer.AddFormatLine("[MapValue({0})] {1},", item.Key, item.Value);
				}
				writer.popIndent().AddLine("}");
			}
			writer.AddEndClass(string.Format("{0}Enums", schemaName));
			writer.AddEndRegion(string.Format("{0}Enums", schemaName));
		}

		private void CreateConfigFile(DB db) {
			var writer = WriterBase.Create(Config.Language);
			writer.Add(GetHeader());
			writer.AddUsing("System", "System.Collections.Generic", "System.Text", "System.Configuration", "System.IO");
			writer.AddLine();
			writer.AddBeginNamespace(Config.RootNamespace);
			writer.AddXmlComment(string.Format("{0}.Configs", Config.RootNamespace));
			writer.AddBeginClass("Configs", "public", true, true);
			writer.AddLine();
			writer.AddBeginRegion("Connection Strings");

			foreach (Schema schema in db.Schemas.Values) {
				writer.AddXmlComment(string.Format("Connection String to {0}", schema.Namespace));
				//writer.AddFormatLine("public static string {0}ConnStr {{ get {{ return \"{1}\"; }} }}", schema.Namespace, Provider.ConnectionString);
				writer.AddFormatLine("public static string {0}ConnStr {{ get {{ return ConfigurationManager.ConnectionStrings[\"{0}ConnStr\"].ConnectionString; }} }}", schema.Namespace);
				writer.AddLine();
			}

			writer.AddEndRegion("Connection Strings");
			writer.AddLine();
			writer.AddXmlComment("a shortcut to AppSettings");
			writer.AddLine("/// <param name=\"name\">\"key\" name of the setting</param>");
			writer.AddLine("/// <returns>value of setting</returns>");

			writer.AddLine("public static string GetAppSetting(string name) {");
			writer.pushIndent();
			writer.AddLine("try {");
			writer.pushIndent();
			writer.AddLine("return ConfigurationManager.AppSettings[name].ToString();");
			writer.popIndent();
			writer.AddLine("} catch (Exception) {");
			writer.pushIndent();
			writer.AddLine("return null;");
			writer.popIndent();
			writer.AddLine("}");
			writer.popIndent();
			writer.AddLine("}");

			writer.AddEndClass("Configs");
			writer.AddEndNamespace(Config.RootNamespace);
			string filename = string.Format("{0}Configs.generated.cs", GetExportFolder());
			if (File.Exists(filename))
				File.Delete(filename);
			File.WriteAllText(filename, writer.Text, System.Text.Encoding.UTF8);
			this.AddMessage("* * Create File {0}", filename);
		}

		private void CreateWarningFile() {
			if (!Config.CheckObjectsValidation)
				return;
			StringBuilder sb = new StringBuilder();
			string filename = string.Format(string.Format("{0}Warning.txt", GetExportFolder()));
			if (File.Exists(filename))
				File.Delete(filename);
			for (int i = 0; i < this.Warnings.Count; i++) {
				string rules = string.Empty;
				foreach (MetaDataEnums.ValidationRules rule in this.Warnings[i].Rules) {
					rules += string.Format("{0}, ", rule.ToString());
				}
				if (rules.Length > 0)
					rules = rules.Remove(rules.Length - 2);
				sb.AppendLine(string.Format("{0}  * Must be {1} ", this.Warnings[i].Objectname, rules));
			}
			if (sb.Length > 0) {
				File.WriteAllText(filename, sb.ToString());
				this.AddMessage("* * Create File {0}", filename);
			}
		}

		private DB NormalizeForeignKeys(DB db) {

			List<ForeignKey> backReferenceKeys = new List<ForeignKey>();
			foreach (Schema schema in db.Schemas.Values) {
				foreach (Table table in schema.Tables.Values) {
					foreach (ForeignKey fk in table.ForeignKeys.Values) {

						// Normalize Name
						string newName = (fk.OtherTable.Parent.SchemaName == fk.Parent.Parent.SchemaName) ? fk.OtherTable.ClassName : fk.OtherTable.Parent.Namespace + fk.OtherTable.ClassName;
						if (fk.Parent.TableName == fk.OtherTable.TableName && fk.Parent.Parent.SchemaName == fk.OtherTable.Parent.SchemaName)
							newName = Helper.GetColumnListName(fk.ThisColumns) + fk.OtherTable.ClassName;
						switch (fk.AssociationType) {
							case MetaDataEnums.AssociationType.OneToOne:
							case MetaDataEnums.AssociationType.ManyToOne:
								newName = Plurals.ToSingular(newName);
								break;
							case MetaDataEnums.AssociationType.OneToMany:
							case MetaDataEnums.AssociationType.ManyToMany:
								newName = Plurals.ToPlural(newName);
								break;
						}
						bool isValid = true;
						bool newFkIsValid = true;
						foreach (Column col in fk.Parent.Columns.Values)
							if (col.PropertyName == newName) {
								isValid = false;
								break;
							}
						if (isValid)
							foreach (ForeignKey otherFk in fk.Parent.ForeignKeys.Values)
								if (otherFk.OtherTable.TableName == fk.OtherTable.TableName && otherFk.OtherTable.Parent.SchemaName == fk.OtherTable.Parent.SchemaName)
									if (otherFk.ID != fk.ID) {
										isValid = false;
										newFkIsValid = false;
									}
						if (newName == table.ClassName) {
							isValid = false;
							newFkIsValid = false;
						}


						if (!isValid)
							newName = string.Format("{0}By{1}", newName, Helper.GetColumnListName(fk.ThisColumns));
						fk.KeyName = newName;

						// Back Reference
						if (fk.Parent.TableName == fk.OtherTable.TableName &&
								fk.Parent.Parent.SchemaName == fk.OtherTable.Parent.SchemaName &&
								fk.AssociationType == MetaDataEnums.AssociationType.OneToOne)
							continue;
						ForeignKey newFk = new ForeignKey(fk.OtherTable);
						switch (fk.AssociationType) {
							case MetaDataEnums.AssociationType.OneToOne:
							case MetaDataEnums.AssociationType.ManyToMany:
								newFk.AssociationType = fk.AssociationType;
								break;
							case MetaDataEnums.AssociationType.OneToMany:
								newFk.AssociationType = MetaDataEnums.AssociationType.ManyToOne;
								break;
							case MetaDataEnums.AssociationType.ManyToOne:
								newFk.AssociationType = MetaDataEnums.AssociationType.OneToMany;
								break;
						}
						newFk.FkName = string.Format("{0}_BackReference", fk.FkName);
						newFk.Description = fk.Description;
						newFk.ID = fk.OtherTable.ForeignKeys.Count + 1;
						newFk.KeyName = (fk.OtherTable.Parent.SchemaName == fk.Parent.Parent.SchemaName) ? table.ClassName : table.Parent.Namespace + table.ClassName;
						newFk.OtherColumns = fk.ThisColumns;
						newFk.OtherTable = table;
						newFk.Properties = fk.Properties;
						newFk.ThisColumns = fk.OtherColumns;
						switch (newFk.AssociationType) {
							case MetaDataEnums.AssociationType.OneToOne:
							case MetaDataEnums.AssociationType.ManyToOne:
								newFk.KeyName = Plurals.ToSingular(newFk.KeyName);
								break;
							case MetaDataEnums.AssociationType.OneToMany:
							case MetaDataEnums.AssociationType.ManyToMany:
								newFk.KeyName = Plurals.ToPlural(newFk.KeyName);
								break;
						}


						foreach (Column col in newFk.Parent.Columns.Values)
							if (col.PropertyName == newFk.KeyName) {
								newFkIsValid = false;
								break;
							}
						if (newFkIsValid)
							foreach (ForeignKey otherFk in newFk.Parent.ForeignKeys.Values)
								if (otherFk.OtherTable.TableName == newFk.OtherTable.TableName && otherFk.OtherTable.Parent.SchemaName == newFk.OtherTable.Parent.SchemaName)
									if (otherFk.ID != fk.ID) {
										newFkIsValid = false;
									}
						if (!newFkIsValid)
							newFk.KeyName = string.Format("{0}By{1}", newFk.MemberName, Helper.GetColumnListName(newFk.OtherColumns));
						newFk.IsBackReference = true;
						//newFk.OtherTable.ForeignKeys.Add(newFk.FkName, newFk);
						backReferenceKeys.Add(newFk);

					}
				}
			}
			foreach (ForeignKey fk in backReferenceKeys) {
				fk.ID = fk.Parent.ForeignKeys.Count + 1;
				fk.Parent.ForeignKeys.Add(fk.FkName, fk);
			}
			return db;
		}

		private DB MakeEnums(DB db) {
			foreach (Schema schema in db.Schemas.Values) {
				foreach (Table table in schema.Tables.Values) {
					foreach (Column col in table.Columns.Values) {
						Data.MetaData.Enum e = Helper.GetMapValue(col);
						if (e == null)
							continue;
						col.IsEnum = true;
						col.EnumType = string.Format("{0}Enums.{1}",schema.Namespace, e.Name.ToNormalPascal());
						if (!schema.Enums.ContainsKey(e.Name))
							schema.Enums.Add(e.Name,e);
					}
				}
			}
			return db;
		}

		private void Validate(IValidatable obj) {
			Validation validation = new Validation();
			validation.Objectname = obj.SqlFullName;
			foreach (MetaDataEnums.ValidationRules rule in obj.ValidationRoles) {
				switch (rule) {
					case MetaDataEnums.ValidationRules.PascalCase:
						if (obj.SqlName != obj.SqlName.ToPascal())
							validation.Rules.Add(MetaDataEnums.ValidationRules.PascalCase);
						break;
					case MetaDataEnums.ValidationRules.CamelCase:
						if (obj.SqlName != obj.SqlName.ToCamel())
							validation.Rules.Add(MetaDataEnums.ValidationRules.CamelCase);
						break;
					case MetaDataEnums.ValidationRules.Singular:
						if (obj.SqlName != Plurals.ToSingular(obj.SqlName))
							validation.Rules.Add(MetaDataEnums.ValidationRules.Singular);
						break;
					case MetaDataEnums.ValidationRules.Plural:
						if (obj.SqlName != Plurals.ToPlural(obj.SqlName))
							validation.Rules.Add(MetaDataEnums.ValidationRules.Plural);
						break;
					case MetaDataEnums.ValidationRules.HasDescription:
						if (string.IsNullOrWhiteSpace(obj.Description))
							validation.Rules.Add(MetaDataEnums.ValidationRules.HasDescription);
						break;
				}
			}
			if (validation.Rules.Count > 0)
				this.Warnings.Add(validation);
		}

		private string GetHeader() {
			var writer = WriterBase.Create(Config.Language);
			writer.AddComment("----------------------------------------------------------------------------------------------------------");
			writer.AddComment(Helper.SogenTitle);
			writer.AddComment(string.Format("Date Created: {0}", DateTime.Now));
			writer.AddComment("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");
			writer.AddComment(" This code was generated for BLToolkit");
			writer.AddComment(" warnning!! Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.");
			writer.AddComment("----------------------------------------------------------------------------------------------------------");
			return writer.Text;
		}

		private void Provider_OnMessageAdd(string message) {
			this.AddMessage(message);
		}

		private void AddMessage(string format, params string[] args) {
			this.AddMessage(string.Format(format, args));
		}

		private void AddMessage(string message) {
			this.Messages.Add(message);
			if (OnMessageAdd != null)
				this.OnMessageAdd(message);
		}

		private string GetExportFolder() {
			var result = string.Format("{0}{1}",
									   Config.ExportPath,
									   (Config.ExportPath.EndsWith("\\")) ? "" : "\\");
			if (!Directory.Exists(result)) {
				Directory.CreateDirectory(result);
				this.AddMessage(string.Format("{0} Created Successfully", result));
			}
			return result;
		}

		#endregion

		#region Public Methods

		public void Execute() {
			this.AddMessage(Helper.SogenTitle);
			this.AddMessage("- Fetch Data :::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			DB db = Provider.GetMetaData();
			this.AddMessage("- Fetch Data : done:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			this.AddMessage("- Normalize ForegnKeys:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			db = NormalizeForeignKeys(db);
			this.AddMessage("- Generate Enums:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			db = MakeEnums(db);
			this.AddMessage("- Normalize ForegnKeys : done:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			this.AddMessage("- Generate Code:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));
			foreach (Schema schema in db.Schemas.Values)
				Generate(schema);

			CreateConfigFile(db);
			CreateWarningFile();

			this.AddMessage("- Generate Code : done:::: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss"));

			StringBuilder sb = new StringBuilder();
			sb = new StringBuilder();
			for (int i = 0; i < this.Messages.Count; i++) {
				sb.AppendLine(this.Messages[i]);
			}
			File.WriteAllText(string.Format("{0}Sogen.log", GetExportFolder()), sb.ToString());
		}





		#region Static Methods

		public static BLToolkitGenerator Create(Configuration.BLToolkitConfiguration config) {

			switch (config.Provider) {
				case DataProviderEnums.Providers.MSSqlServer:
					switch (config.Language) {
						case WriterEnums.Languages.CSharp:
							return new BLToolkitGenerator(new MSSqlSmoProvider(config));
						default:
							throw new Exception("Language does not supported!");
					}
				default:
					throw new Exception("Provider does not supported!");
			}

		}

		#endregion

		#endregion

	}
}

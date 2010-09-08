using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {

	public class Table {
		public int ID { get; set; }
		public string TableName { get; set; }
		public string ClassName {
			get { return Helper.GetName(Properties, this.TableName); }
		}
		public string Description { get; set; }
		public bool IsView { get; set; }

		public Column IdentityColumn {
			get {
				foreach (Column col in this.Columns.Values)
					if (col.IsIdentity)
						return col;
				return null;
			}
		}

		public UniqueKey PrimaryKey {
			get {
				foreach (UniqueKey uk in this.UniqueKeys.Values)
					if (uk.Type == MetaDataEnums.UniqueKeyType.Primary)
						return uk;
				return null;

			}
		}

		private Dictionary<string, string> _properties = new Dictionary<string, string>();
		public Dictionary<string, string> Properties {
			get { return this._properties; }
			set { this._properties = value; }
		}

		public List<string> Attributes {
			get { return Helper.GetAttributes(this.Properties); }
		}

		private Dictionary<string, Column> _columns = new Dictionary<string, Column>();
		public Dictionary<string, Column> Columns {
			get { return this._columns; }
			set { this._columns = value; }
		}

		private Dictionary<string, ForeignKey> _foreignKeys = new Dictionary<string, ForeignKey>();
		public Dictionary<string, ForeignKey> ForeignKeys {
			get { return this._foreignKeys; }
			set { this._foreignKeys = value; }
		}

		private Dictionary<string, UniqueKey> _uniqueKeys = new Dictionary<string, UniqueKey>();
		public Dictionary<string, UniqueKey> UniqueKeys {
			get { return this._uniqueKeys; }
			set { this._uniqueKeys = value; }
		}
		public override string ToString() {
			return this.ClassName;
		}
		public Schema Parent { get; private set; }

		public Table(Schema parent) {
			this.Parent = parent;
		}
	}

}

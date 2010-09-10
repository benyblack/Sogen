using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {

	internal class Column :IValidatable {
		public int ID { get; set; }
		public string ColumnName { get; set; }
		public string PropertyName {
			get { return Helper.GetName(Properties, this.ColumnName); }
		}
		public string FiledName { get { return this.ColumnName.ToNormalCamelWithUnderscore(); } }
		public bool IsNullable { get; set; }
		public bool IsIdentity { get; set; }
		public bool IsPrimaryKey { get; set; }
		public string DefaultValue { get; set; }
		public string Description { get; set; }
		public DataType Type { get; set; }

		private Dictionary<string, string> _properties = new Dictionary<string, string>();
		public Dictionary<string, string> Properties {
			get { return this._properties; }
			set { this._properties = value; }
		}
		public List<string> Attributes {
			get { return Helper.GetAttributes(this.Properties); }
		}
		public override string ToString() {
			return this.PropertyName;
		}
		public Table Parent { get; private set; }

		public Column(Table parent) {
			this.Parent = parent;
		}

		#region IValidatable Members
		public string SqlName {
			get { return this.ColumnName; }
		}

		public string SqlFullName {
			get { return string.Format("[{0}].[{1}].[{2}]", Parent.Parent.SqlName, Parent.SqlName, this.SqlName); }
		}

		MetaDataEnums.ValidationRules[] IValidatable.ValidationRoles {
			get { return new MetaDataEnums.ValidationRules[] { MetaDataEnums.ValidationRules.HasDescription }; }
		}

		#endregion
	}

}

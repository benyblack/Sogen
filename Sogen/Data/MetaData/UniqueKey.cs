using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {

	public class UniqueKey {
		public int ID { get; set; }
		public string UKName { get; set; }
		public string KeyName { get; set; }
		public string MemberName {
			get { return Helper.GetName(Properties, this.KeyName); }
		}
		public string Description { get; set; }
		public MetaDataEnums.UniqueKeyType Type { get; set; }

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
		public override string ToString() {
			return this.KeyName;
		}
		public Table Parent { get; private set; }

		public UniqueKey(Table parent) {
			this.Parent = parent;
		}
	}

}

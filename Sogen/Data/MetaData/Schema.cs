using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {
	public class Schema {
		public string SchemaName { get; set; }
		public string Namespace {
			get { return Helper.GetName(Properties, this.SchemaName); }
		}
		public string Description { get; set; }

		private Dictionary<string, string> _properties = new Dictionary<string, string>();
		public Dictionary<string, string> Properties {
			get { return this._properties; }
			set { this._properties = value; }
		}

		private Dictionary<string, Table> _tables = new Dictionary<string, Table>();
		public Dictionary<string, Table> Tables {
			get { return this._tables; }
			set { this._tables = value; }
		}
		public override string ToString() {
			return this.Namespace;
		}

	}
}

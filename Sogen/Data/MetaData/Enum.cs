using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sogen.Data.MetaData {
	internal class Enum {
		public string Name { get; set; }
		private Dictionary<string, KeyValuePair<string, string>> _values = new Dictionary<string, KeyValuePair<string, string>>();
		public Dictionary<string, KeyValuePair<string, string>> Values {
			get { return this._values; }
			set { this._values = value; }
		}
	}
}

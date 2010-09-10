using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Sogen.Common;

namespace Sogen.Data.MetaData {

	internal class DataType {
		public string Type { get; set; }
		public string CSharpType { get; set; }
		public string CSharpNullableType { get; set; }
		public DbType DbType { get; set; }
		public SqlDbType SqlDbType { get; set; }
		public override string ToString() {
			return this.CSharpType;
		}
	}

}

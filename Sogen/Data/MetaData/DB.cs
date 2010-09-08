using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;


namespace Sogen.Data.MetaData {

	public class DB {

		public string DBName { get; set; }

		private Dictionary<string, Schema> _schemas = new Dictionary<string, Schema>();
		public Dictionary<string, Schema> Schemas {
			get { return this._schemas; }
			set { this._schemas = value; }
		}
	}
}

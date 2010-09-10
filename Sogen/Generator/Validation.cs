using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sogen.Generator {
	public class Validation {
		private List<Data.MetaData.MetaDataEnums.ValidationRules> _rules = new List<Data.MetaData.MetaDataEnums.ValidationRules>();
		public List<Data.MetaData.MetaDataEnums.ValidationRules> Rules {
			get { return this._rules; }
			set { this._rules = value; }
		}

		public string Objectname { get; set; }
	}
}

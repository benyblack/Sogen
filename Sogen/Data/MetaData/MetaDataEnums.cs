using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {
	public class MetaDataEnums {
		public enum AssociationType {
			OneToOne,
			OneToMany,
			ManyToOne,
			ManyToMany
		}

		public enum UniqueKeyType {
			Primary,
			UniqueIdentifier,
			UniqueKey
		}
		public enum ValidationRules {
			PascalCase,
			CamelCase,
			Singular,
			Plural,
			HasDescription
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sogen.Data.MetaData {
	internal interface IValidatable {
		string SqlName { get; }
		string SqlFullName { get; }
		string Description { get; }
		MetaDataEnums.ValidationRules[] ValidationRoles { get; }


	}
}

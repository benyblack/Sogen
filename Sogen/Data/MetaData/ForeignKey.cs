using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;

namespace Sogen.Data.MetaData {

	public class ForeignKey {
		public int ID { get; set; }
		public string FkName { get; set; }
		public string KeyName { get; set; }
		public string MemberName {
			get {
				if (this.IsBackReference)
					return Helper.GetBackReferenceName(Properties, this.KeyName);
				else
					return Helper.GetName(Properties, this.KeyName);
			}
		}
		public Table OtherTable { get; set; }
		public string Description { get; set; }

		private bool _isBackReference = false;
		public bool IsBackReference {
			get { return this._isBackReference; }
			set { this._isBackReference = value; }
		}



		private Dictionary<string, string> _properties = new Dictionary<string, string>();
		public Dictionary<string, string> Properties {
			get { return this._properties; }
			set { this._properties = value; }
		}
		public List<string> Attributes {
			get { return Helper.GetAttributes(this.Properties); }
		}

		private Dictionary<string, Column> _thisColumns = new Dictionary<string, Column>();
		public Dictionary<string, Column> ThisColumns {
			get { return this._thisColumns; }
			set { this._thisColumns = value; }
		}

		private Dictionary<string, Column> _otherColumns = new Dictionary<string, Column>();
		public Dictionary<string, Column> OtherColumns {
			get { return this._otherColumns; }
			set { this._otherColumns = value; }
		}

		private MetaDataEnums.AssociationType _associationType = MetaDataEnums.AssociationType.ManyToOne;
		public MetaDataEnums.AssociationType AssociationType {
			get { return _associationType; }
			set { _associationType = value; }
		}
		public override string ToString() {
			return this.MemberName;
		}
		public Table Parent { get; private set; }

		public ForeignKey(Table parent) {
			this.Parent = parent;
		}
	}

}

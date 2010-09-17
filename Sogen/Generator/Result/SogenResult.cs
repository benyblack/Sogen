using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;
using Sogen.Data.MetaData;

namespace Sogen.Generator.Result {
	public class SogenResult {
		public string Version { get { return Helper.SogenVersion; } }
		public string Generator { get { return Helper.SogenTitle; } }
		public List<string> Messages { get; set; }
		public List<Validation> Warnings { get; set; }
		public string MessagesString {
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < this.Messages.Count; i++) {
					sb.AppendLine(this.Messages[i]);
				}
				return sb.ToString();
			}
		}
		public string WarningsString {
			get {
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < this.Warnings.Count; i++) {
					string rules = string.Empty;
					foreach (MetaDataEnums.ValidationRules rule in this.Warnings[i].Rules) {
						rules += string.Format("{0}, ", rule.ToString());
					}
					if (rules.Length > 0)
						rules = rules.Remove(rules.Length - 2);
					sb.AppendLine(string.Format("{0}  * Must be {1} ", this.Warnings[i].Objectname, rules));
				}
				return sb.ToString();
			}
		}
		public List<ResultFile> Files { get; set; }

		public SogenResult() {
			this.Messages = new List<string>();
			this.Warnings = new List<Validation>();
			this.Files = new List<ResultFile>();
		}
	}
}

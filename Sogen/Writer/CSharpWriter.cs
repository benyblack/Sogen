using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Sogen.Common;

namespace Sogen.Writer {
	internal class CSharpWriter : WriterBase {

		#region Constructor
		public CSharpWriter() : base(WriterEnums.Languages.CSharp) { }
		#endregion

		public override WriterBase AddComment(string s) {
			this.AddFormatLine("// {0}", s);
			return this;

		}

		public override WriterBase AddUsing(params string[] s) {
			if (s.Length == 0)
				return this;
			for (int i = 0; i < s.Length; i++) {
				if (!string.IsNullOrEmpty(s[i]))
					this.AddFormatLine("using {0};", s[i]);
			}
			return this;
		}

		public override WriterBase AddBeginNamespace(string s) {
			if (string.IsNullOrEmpty(s))
				return this;
			this.AddFormatLine("namespace {0} {{", s);
			this.pushIndent();
			return this;
		}

		public override WriterBase AddEndNamespace([Optional, DefaultParameterValue("")] string s) {
			this.popIndent();
			if (string.IsNullOrEmpty(s))
				this.AddLine("}");
			else
				this.AddFormatLine("}} // {0}", s);
			return this;
		}

		public override WriterBase AddBeginClass(string className, [Optional, DefaultParameterValue("public")] string accessModifier, [Optional, DefaultParameterValue(false)] bool isStatic, [Optional, DefaultParameterValue(true)] bool isPartial, [Optional] params string[] baseClass) {
			if (string.IsNullOrEmpty(className))
				return this;
			this.AddFormat("{0}{1}{2} class {3}",
								accessModifier,
								isStatic ? " static" : "",
								isPartial ? " partial" : "",
								className);
			if (baseClass.Length > 0) {
				this.Add(" :");
				foreach (string item in baseClass) {
					if (!string.IsNullOrEmpty(item))
						this.AddFormat(" {0},", item);
				}
				this.Remove(1);
			}
			this.AddLine(" {");
			this.pushIndent();
			return this;
		}

		public override WriterBase AddEndClass([Optional, DefaultParameterValue("")] string s) {
			this.popIndent();
			if (string.IsNullOrEmpty(s))
				this.AddLine("}");
			else
				this.AddFormatLine("}} // {0}", s);
			return this;
		}

		public override WriterBase AddGenericType(string genericType, string type) {
			this.AddFormat("{0}<{1}>", genericType, type);
			return this;
		}

		public override WriterBase AddTableProperty(Data.MetaData.Column column) {
			return AddPropertyWithFiled(column.PropertyName,
										column.Type.CSharpType,
										column.Description,
										column.DefaultValue);
		}

		public override WriterBase AddProperty(string name, string type, [Optional, DefaultParameterValue("")] string description) {
			this.AddXmlComment(description);
			this.AddFormatLine("public {0} {1} {{ get; set; }}", type, name.ToNormalPascal());
			return this;
		}

		public override WriterBase AddPropertyReadOnly(string name, string type, string value, [Optional, DefaultParameterValue("")] string description) {
			this.AddXmlComment(description);
			this.AddFormatLine("public {0} {1} {{ get {{return {2};}} }}", type, name.ToNormalPascal(), value);
			return this;

		}

		public override WriterBase AddPropertyWithFiled(string name, string type, [Optional, DefaultParameterValue("")] string description, [Optional, DefaultParameterValue("")] string defalutValue, [Optional] string[] attributes) {
			this.AddFormat("private {0} {1}",
								type,
								name.ToNormalCamelWithUnderscore());
			if (string.IsNullOrEmpty(defalutValue))
				this.AddLine(";");
			else
				this.AddFormatLine(" = {0};", defalutValue);

			this.AddXmlComment(description);
			this.AddAttribute(attributes);
			this.AddFormatLine("public {0} {1} {{",
								type,
								name.ToNormalPascal());
			this.pushIndent();
			this.AddFormatLine("get {{ return this.{0}; }}", name.ToNormalCamelWithUnderscore());
			this.AddFormatLine("set {{ this.{0} = value; }}", name.ToNormalCamelWithUnderscore());
			this.popIndent();
			this.AddLine("}");
			return this;
		}

		public override WriterBase AddXmlComment(string s) {
			if (string.IsNullOrEmpty(s))
				return this;
			this.AddLine("/// <summary>");
			this.AddFormatLine("/// {0}", s);
			this.AddLine("/// </summary>");
			return this;
		}

		public override WriterBase AddAttribute(string[] attributes) {
			if (attributes.Length == 0)
				return this;
			this.Add("[");
			foreach (string item in attributes) {
				if (!string.IsNullOrEmpty(item))
					this.AddFormat("{0}, ", item);
			}
			this.Remove(2).AddLine("]");
			return this;
		}

		public override WriterBase AddBeginRegion(string s = "") {
			this.AddFormatLine("#region {0}", s);
			this.pushIndent();
			return this;
		}

		public override WriterBase AddEndRegion(string s = "") {
			this.popIndent();
			this.AddFormatLine("#endregion {0}", s);
			return this;
		}
	}
}

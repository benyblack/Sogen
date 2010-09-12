using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;




namespace Sogen.Writer {


	internal abstract class WriterBase {

		#region Properties
		public string Indent {
			get {
				string value = string.Empty;
				for (int i = 0; i < _indent.Count; i++)
					value += (_indent[i]);
				return value;
			}
		}
		public int Length { get { return this._text.Length; } }
		public string Text { get { return this._text.ToString(); } }
		public WriterEnums.Languages Language { get; private set; }
		#endregion

		#region Private Methods
		public StringBuilder _text = new StringBuilder();
		public List<char> _indent = new List<char>();
		#endregion

		#region Public Methods
		public WriterBase pushIndent([Optional, DefaultParameterValue('\t')] char c) {
			if (this.Text.EndsWith(this.Indent))
				this._text.Append(c);
			_indent.Add(c);
			if (this._text.Length == 0)
				this._text.Append(this.Indent);
			return this;
		}
		public WriterBase popIndent() {
			try {
				if (this.Text.EndsWith(this.Indent))
					this._text = this._text.Remove(this._text.Length - 1, 1);
				_indent.RemoveAt(_indent.Count - 1);
			} catch (Exception) { }
			return this;
		}
		public override string ToString() {
			return this.Text;
		}
		public WriterBase clear() {
			this._text.Clear();
			this._indent = new List<char>();
			return this;
		}
		public WriterBase AddIndent() {
            return this.Add(this.Indent);
		}
		public WriterBase AddLine() {
            return this.Add("\r\n");
		}
		public WriterBase Add(string s) {
			this._text.Append(s.Replace("\n", "\n" + this.Indent));
			return this;
		}
		public WriterBase AddLine(string s) {
			return this.Add(s).AddLine();
		}
		public WriterBase AddFormat(string format, params string[] s) {
            StringBuilder tmp = new StringBuilder();
            tmp.AppendFormat(format, s);
            return this.Add(tmp.ToString());
		}
		public WriterBase AddFormatLine(string format, params string[] s) {
			return this.AddFormat(format, s).AddLine();
		}
		public WriterBase Remove(int count) {
			if (this.Length < count)
				count = this.Length;
			this._text.Remove(this.Length - count, count);
			return this;
		}
		#endregion

		#region Constructor
		public WriterBase(WriterEnums.Languages language) {
			this.Language = language;
		}
		#endregion

		#region Abstract Methods

		public abstract WriterBase AddComment(string s);
		public abstract WriterBase AddUsing(params string[] s);
		public abstract WriterBase AddBeginNamespace(string s);
		public abstract WriterBase AddEndNamespace([Optional, DefaultParameterValue("")] string s);
		public abstract WriterBase AddBeginClass(string className, [Optional, DefaultParameterValue("public")] string accessModifier, [Optional, DefaultParameterValue(false)] bool isStatic, [Optional, DefaultParameterValue(true)] bool isPartial, [Optional] params string[] baseClass);
		public abstract WriterBase AddEndClass([Optional, DefaultParameterValue("")] string s);
		public abstract WriterBase AddGenericType(string genericType, string type);
		public abstract WriterBase AddTableProperty(Data.MetaData.Column column);
		public abstract WriterBase AddProperty(string name, string type, [Optional, DefaultParameterValue("")] string description);
		public abstract WriterBase AddPropertyReadOnly(string name, string type, string value, [Optional, DefaultParameterValue("")] string description);
		public abstract WriterBase AddPropertyWithFiled(string name, string type, [Optional, DefaultParameterValue("")] string description, [Optional, DefaultParameterValue("")] string defalutValue, [Optional] string[] attributes);
		public abstract WriterBase AddXmlComment(string s);
		public abstract WriterBase AddAttribute(string[] attributes);
		public abstract WriterBase AddBeginRegion([Optional, DefaultParameterValue("")] string s);
		public abstract WriterBase AddEndRegion([Optional, DefaultParameterValue("")] string s);

		#endregion

		#region Static Methods
		public static WriterBase Create(WriterEnums.Languages language) {
			switch (language) {
				case WriterEnums.Languages.CSharp:
					return new CSharpWriter();
				default:
					return null;
			}

		}
		#endregion
	}
}

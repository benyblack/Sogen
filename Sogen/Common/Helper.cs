using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Reflection;


namespace Sogen.Common {
	public static class Helper {
		public static string SogenVersion {
			get {
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}
		public static string SogenTitle {
			get {

				var description = ((AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(
						Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute))).Description;
				var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
				return string.Format("{0} - {1} version {2}", name, description, SogenVersion);
			}
		}

		private static List<string> ReserverdWords {
			get {
				return new List<string>() { "abstract", "as", "base", "bool", "break", "by", "byte", "case",
				"catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
				"double", "descending", "explicit", "event", "extern", "else", "enum", "false", "finally", "fixed",
				"float", "for", "foreach", "from", "goto", "group", "if", "implicit", "in", "int", "interface", 
				"internal", "into", "is", "lock", "long", "new", "null", "namespace", "object", "operator", "out", 
				"override", "orderby", "params","private", "protected", "public", "readonly", "ref", "return", 
				"switch", "struct", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "select", 
				"this", "throw", "true", "try", "typeof","uint", "ulong", "unchecked", "unsafe", "ushort", "using", 
				"var", "virtual", "volatile", "void", "while", "where", "yield" };
			}
		}




		#region Constant
		public const string NameKey = "Sogen_Name";
		public const string BackReferenceNameKey = "Sogen_BackReferenceName";
		public const string AttributeKey = "Sogen_Attribute";
		public const string MapValueKey = "Sogen_MapValue";
		#endregion

		#region  Extended Methods

		internal static string ToCamel(this string s) {
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			if (s.ToUpper() == "ID")
				return "id";

			return s[0].ToString().ToLower() + s.Remove(0, 1);
		}

		internal static string ToPascal(this string s) {
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			//if (s.ToUpper() == "ID")
			//return "ID";


			return s[0].ToString().ToUpper() + s.Remove(0, 1);
		}

		internal static string AppendLine(this string s, string appendString) {
			return (s + appendString).AppendLine();
		}

		internal static string AppendLine(this string s) {
			return s + "\r\n";
		}

		public static string ToXml(this object instance) {
			XmlSerializer serializer = new XmlSerializer(instance.GetType());

			MemoryStream stream = new MemoryStream();

			serializer.Serialize(stream, instance);

			string xml = Encoding.UTF8.GetString(stream.ToArray());

			stream.Close();

			return xml;
		}

		public static object ToObject(this string xml, System.Type type) {
			XmlSerializer serializer = new XmlSerializer(type);

			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

			object instance = serializer.Deserialize(stream);

			stream.Close();

			return instance;
		}

		internal static string ToNormalPascal(this string s) {
			return s.ToPascal().ToNormal();
		}

		internal static string ToNormalCamel(this string s) {
			return s.ToCamel().ToNormal();
		}

		internal static string ToNormalCamelWithUnderscore(this string s) {
			return "_" + s.ToNormalCamel();
		}

		internal static string ToDescription(this string s) {
			// must be complete
			return s;
		}

		internal static string ToNormal(this string s) {
			//if (s.ToUpper() == "ID")
			//return "ID";
			s = s.Replace("_", "").Replace(" ", "");
			if (ReserverdWords.Contains(s.ToLower()))
				s = s + "1";
			return s;
		}
		#endregion

		#region Methods


		internal static string GetColumnListName(Dictionary<string, Data.MetaData.Column> cols) {
			var result = string.Empty;
			foreach (Data.MetaData.Column col in cols.Values)
				result += col.PropertyName + "And";
			if (result.Length > 0)
				result = result.Remove(result.Length - 3, 3);
			return result;
		}

		internal static List<string> GetAttributes(Dictionary<string, string> properties) {
			var result = new List<string>();
			if (!properties.ContainsKey(AttributeKey))
				return result;

			var attributes = properties[AttributeKey].ToString();
			attributes.Replace("\r", "");
			var attr = attributes.Split('\n');
			for (int i = 0; i < attr.Length; i++) {
				result.Add(attr[i].Replace("\r", "").Replace("\n", ""));
			}
			return result;
		}

		internal static string GetName(Dictionary<string, string> properties, string def) {
			if (properties.ContainsKey(NameKey))
				return properties[NameKey];
			return def.ToNormalPascal();
		}

		internal static Data.MetaData.Enum GetMapValue(Data.MetaData.Column col) {
			if (!col.Properties.ContainsKey(MapValueKey))
				return null;

			try {
				Data.MetaData.Enum e = new Data.MetaData.Enum();
				string propValue = col.Properties[MapValueKey].ToString();
				propValue.Replace("\r", "");
				var lines = propValue.Split('\n');
				e.Name = lines[0].Replace("\r", "").Replace("\n", "").ToNormalPascal();
				for (int i = 1; i < lines.Length; i++) {
					var value = lines[i].Replace("\r", "").Replace("\n", "").Split(':');
					if (value.Length == 2)
						e.Values.Add(value[0], new KeyValuePair<string, string>(value[0], value[1].ToNormalPascal()));
				}
				return e;
			} catch (Exception) {
				return null;
			}
		}


		internal static string GetBackReferenceName(Dictionary<string, string> properties, string def) {
			if (properties.ContainsKey(BackReferenceNameKey))
				return properties[BackReferenceNameKey];
			return def.ToNormalPascal();
		}

		internal static string GetColumnList(Dictionary<string, Data.MetaData.Column> cols, string format, string delimiter, bool hideIdentities, bool hidePrimaries) {
			var result = string.Empty;
			foreach (Data.MetaData.Column col in cols.Values) {
				if (hideIdentities && col.IsIdentity)
					continue;

				if (hidePrimaries && col.IsPrimaryKey)
					continue;
				string tmp = format.Replace("{column}", col.ColumnName)
											.Replace("{property}", col.PropertyName)
											.Replace("{filed}", col.FiledName)
											.Replace("{type}", col.GetType)
											.Replace("{value}", (col.IsEnum) ? "" : (col.IsNullable) ? (col.Type.CSharpNullableType == col.Type.CSharpType ? "" : ".Value") : "")
											.Replace("{camelprop}", col.PropertyName.ToNormalCamel());

				result += tmp + delimiter;
			}

			if (result.Length > 0)
				result = result.Remove(result.Length - delimiter.Length, delimiter.Length);
			return result;
		}

		#endregion


	}
}

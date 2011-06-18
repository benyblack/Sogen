using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Serialization;
using Sogen.Writer;
using Sogen.Data.MetaData;
using Sogen.Data.DataProvider;

namespace Sogen.Generator.Configuration {


	[Serializable()]
	[XmlRoot("sogenBLToolkitGeneratorConfiguration")]
	public class BLToolkitConfiguration {

		#region Properties

		[XmlElement("rootNamespace")]
		public string RootNamespace { get; set; }
		[XmlElement("dataModelNamePostfix")]
		public string DataModelNamePostfix { get; set; }
		[XmlElement("dataModelAccessModifier")]
		public string DataModelAccessModifier { get; set; }
		[XmlElement("dataAccessMethodsAccessModifier")]
		public string DataAccessMethodsAccessModifier { get; set; }
		[XmlIgnore()]
		private string _exportPath;
		[XmlElement("exportPath")]
		public string ExportPath {
			get { return this._exportPath; }
			set {
				if (!value.EndsWith("\\"))
					value += "\\";
				this._exportPath = value;
			}
		}
		[XmlElement("generateEmptyPartialClass")]
		public bool GenerateEmptyPartialClass { get; set; }
		[XmlElement("checkObjectsValidation")]
		public bool CheckObjectsValidation { get; set; }
		[XmlArray("customUsings"), XmlArrayItem("using")]
		public string[] CustomUsings { get; set; }
		[XmlElement("provider")]
		public DataProviderEnums.Providers Provider { get; set; }
		[XmlElement("language")]
		public WriterEnums.Languages Language { get; set; }
		[XmlElement("MSSqlConfig")]
		public MSSqlConfiguration MSSqlConfig { get; set; }

		#endregion

		#region Constructors
		public BLToolkitConfiguration() {
			this.RootNamespace = "Sogen.DB";
			this.DataModelNamePostfix = "DataModel";
			this.DataModelAccessModifier = "public";
			this.DataAccessMethodsAccessModifier = "public";
			this.ExportPath = "C:\\SogenGenerated\\";
			this.GenerateEmptyPartialClass =
				this.CheckObjectsValidation = true;
			this.Provider = DataProviderEnums.Providers.MSSqlServer;
			this.Language = WriterEnums.Languages.CSharp;
			this.MSSqlConfig = new MSSqlConfiguration();
			this.CustomUsings = new string[] { "" };

		}
		#endregion


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
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
		[XmlElement("dataModelName")]
		public string DataModelName { get; set; }
		[XmlElement("dataModelAccessModifier")]
		public string DataModelAccessModifier { get; set; }
		[XmlElement("exportPath")]
		public string ExportPath { get; set; }
		[XmlElement("generateEmptyPartialClass")]
		public bool GenerateEmptyPartialClass { get; set; }
		[XmlElement("generateInsertUpdateDeleteMethods")]
		public bool GenerateInsertUpdateDelete { get; set; }
		[XmlElement("generateGetMethods")]
		public bool GenerateGetMethods { get; set; }
		[XmlElement("generateGetByForeignKeys")]
		public bool GenerateGetByForeignKeys { get; set; }
		[XmlElement("generateBreadcrumbs")]
		public bool GenerateBreadcrumbs { get; set; }
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
			this.DataModelName = "DataModel";
			this.DataModelAccessModifier = "public";
			this.ExportPath = "C:\\SogenGenerated\\";
			this.GenerateEmptyPartialClass =
				this.GenerateInsertUpdateDelete =
				this.GenerateGetMethods =
				this.GenerateGetByForeignKeys =
				this.GenerateBreadcrumbs =
				this.CheckObjectsValidation = true;
			this.Provider = DataProviderEnums.Providers.MSSqlServer;
			this.Language = WriterEnums.Languages.CSharp;
			this.MSSqlConfig = new MSSqlConfiguration();
			this.CustomUsings = new string[] { "Library Name" };

		}
		#endregion


	}
}

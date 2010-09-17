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
	[XmlRoot("MSSqlConfig")]
	public class MSSqlConfiguration {
		[XmlElement("server")]
		public string Server { get; set; }

		[XmlElement("database")]
		public string Database { get; set; }

		[XmlElement("username")]
		public string Username { get; set; }

		[XmlElement("password")]
		public string Password { get; set; }

		[XmlElement("useWindowsAuthentication")]
		public bool WindowsAuthentication { get; set; }

		[XmlArray("schemas"), XmlArrayItem("schema")]
		public string[] Schemas { get; set; }

		[XmlElement("generateObjectForViews")]
		public bool GenerateObjectForViews { get; set; }


		public MSSqlConfiguration() {
			this.Server = "erver";
			this.Database = "db";
			this.Username = "user";
			this.Password = "pass";
			this.WindowsAuthentication = false;
			this.Schemas = new string[] { "" };
			this.GenerateObjectForViews = true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sogen.Generator.Result {
	public class ResultFile {
		public string Filename { get; set; }
		public string FileExtention { get; set; }
		public string RelativePath { get; set; }
		public bool Overwrite { get; set; }
		public string FileContent { get; set; }
		

		public ResultFile(){}

		public ResultFile(
				string filename,
				string fileExtention,
				string relativePath,
				bool overWrite,
				string fileContent) {
			this.Filename = filename;
			this.FileExtention = fileExtention;
			this.RelativePath = relativePath;
			this.Overwrite = overWrite;
			this.FileContent = fileContent;
			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Generator;
using Sogen.Generator.Configuration;
using Sogen.Common;
using System.IO;

namespace Sogen.ConsoleApp {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine(Sogen.Common.Helper.SogenTitle);

			string filename = (args.Length > 0) ? args[0] : "SogenBLToolkitconfig.xml";

			if (!File.Exists(filename)) {
				FileNotFound(filename);
				return;
			}

			string	xml = File.ReadAllText(filename);
			GeneratorBL(xml);
		}
		private static void GeneratorBL(string xml) {
			BLToolkitConfiguration config = new BLToolkitConfiguration();

			try {
				config = (BLToolkitConfiguration)xml.ToObject(typeof(BLToolkitConfiguration));
			} catch (Exception) {
				Console.WriteLine(string.Format("Configuration file isn't in valid format"));
				Console.ReadKey();
				return;
			}

			BLToolkitGenerator generator = BLToolkitGenerator.Create(config);
			if (generator == null) {
				Console.WriteLine("Can't Connect to db");
				Console.ReadKey();
				return;
			}
			
			generator.OnMessageAdd += new BLToolkitGenerator.OnMessageAddHandler(generator_OnMessageAdd);
			Console.WriteLine("Connected to db Successfully");

			generator.Execute();

			System.Diagnostics.Process.Start(config.ExportPath);
			//	Console.ReadKey();
		}

		private static void FileNotFound(string filename) {
			Console.WriteLine(string.Format("Can't Find File '{0}'", filename));
			Console.Write("Create Template Config File (Y,N)? ");
			var key =  Console.ReadKey();
			if (key.KeyChar.ToString().ToLower() == "y") {
				BLToolkitConfiguration config = new BLToolkitConfiguration();
				File.WriteAllText(filename, config.ToXml());
				Console.WriteLine();
				Console.Write("'{0}' Created.", filename);
				Console.ReadKey();
			}
		}

		private static void ShowHelp() {
			Console.WriteLine(" * * * Help * * *");
			Console.WriteLine(" Under Construction ");
			Console.ReadKey();

		}

		private static void generator_OnMessageAdd(string message) {
			Console.WriteLine(message);
		}


	}
}

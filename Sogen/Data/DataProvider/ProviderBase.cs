using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sogen.Common;
using Sogen.Data.MetaData;
using Sogen.Generator.Configuration;
using Sogen.Generator;

namespace Sogen.Data.DataProvider {



	internal abstract class ProviderBase {

		public string BLToolkitDataProvider {
			get {
				switch (this.ProviderType) {
					case DataProviderEnums.Providers.MSSqlServer:
						return "SqlDataProvider";
					default:
						return "SqlDataProvider";
				}
			}
		}

		public BLToolkitConfiguration Config { get; private set; }

		public DataProviderEnums.Providers ProviderType { get; private set; }

		public List<string> Messages { get; protected set; }

		public ProviderBase(BLToolkitConfiguration config) {
			this.Config = config;
			this.ProviderType = config.Provider;
			this.Messages = new List<string>();
		}

		public abstract string ConnectionString { get; }

		public abstract DB GetMetaData();

		protected void AddMessage(string format, params string[] args) {
			this.AddMessage(string.Format(format, args));
		}

		protected void AddMessage(string message) {
			this.Messages.Add(message);
			if (OnMessageAdd != null)
				this.OnMessageAdd(message);
		}

		public event BLToolkitGenerator.OnMessageAddHandler OnMessageAdd;


	}
}

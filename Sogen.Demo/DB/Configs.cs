// ----------------------------------------------------------------------------------------------------------
// Sogen - Code Generator for BLToolkit version 1.4.3.0
// Date Created: 2010-09-17 7:12:13 PM
// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
//  This code was generated for BLToolkit
//  warnning!! Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// ----------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;

namespace Sogen.Demo.DB {
	/// <summary>
	/// Sogen.Demo.DB.Configs
	/// </summary>
	public static partial class Configs {
		
		#region Connection Strings
			/// <summary>
			/// Connection String to Members
			/// </summary>
			public static string MembersConnStr { get { return ConfigurationManager.ConnectionStrings["MembersConnStr"].ConnectionString; } }
			
			/// <summary>
			/// Connection String to Common
			/// </summary>
			public static string CommonConnStr { get { return ConfigurationManager.ConnectionStrings["CommonConnStr"].ConnectionString; } }
			
			/// <summary>
			/// Connection String to Contacts
			/// </summary>
			public static string ContactsConnStr { get { return ConfigurationManager.ConnectionStrings["ContactsConnStr"].ConnectionString; } }
			
			/// <summary>
			/// Connection String to Messages
			/// </summary>
			public static string MessagesConnStr { get { return ConfigurationManager.ConnectionStrings["MessagesConnStr"].ConnectionString; } }
			
		#endregion Connection Strings
		
		/// <summary>
		/// a shortcut to AppSettings
		/// </summary>
		/// <param name="name">"key" name of the setting</param>
		/// <returns>value of setting</returns>
		public static string GetAppSetting(string name) {
			try {
				return ConfigurationManager.AppSettings[name].ToString();
			} catch (Exception) {
				return null;
			}
		}
	} // Configs
} // Sogen.Demo.DB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Sogen.Demo {
	class Program {
		static void Main(string[] args) {
			Console.WriteLine("Sogen Generator Demo");

			Console.WriteLine();
			InsertUpdateDetele();
			Console.WriteLine();
			Enamurations();
			Console.WriteLine();
			LazyLoadingAndAssociations();
			Console.WriteLine();
			LinqAndLazyLoading();
		}

		private static void InsertUpdateDetele() {
			Console.WriteLine("* * Insert Update Detele Test");
			var firstCountry = DB.Common.Country.GetByISOCode("USA"); // or fisrtCountry = DB.Common.Country.GetByID(1);
			Console.WriteLine("Country '{0}' get from db", firstCountry.Title);

			firstCountry.Description = "United State of America";
			firstCountry.Update();
			Console.WriteLine("Description added to '{0}'", firstCountry.Title);

			var secondCountry = new DB.Common.Country();
			secondCountry.Title = "Australia";
			secondCountry.ISOCode = "AUS";
			// secondCountry.Description = ""; do not need to this line, Because this column has default value in data base and generated class has too. :)
			secondCountry.Insert();

			Console.WriteLine("Country '{0}' added by code '{1}'", secondCountry.Title, secondCountry.ID);

			secondCountry.Delete();
			Console.WriteLine("Country '{0}' delete from", secondCountry.Title);

			Console.ReadKey();
		}

		private static void Enamurations() {
			Console.WriteLine("* * Enamuration Test");
			var messenger = new DB.Contacts.Messenger();
			messenger.MemberID = 1;
			messenger.Username = "test";
			messenger.Service = DB.Contacts.ContactsEnums.MessengerService.MSN; // Base on Extended property of this filed
			messenger.Insert();
			messenger.Delete();
			Console.WriteLine("messenger added & deleted from db ");
			Console.ReadKey();
		}

		private static void LazyLoadingAndAssociations() {
			Console.WriteLine("* * Lazy loading & Associations");
			var memeber = DB.Members.Member.GetByID(1); // or GetByOfficeAndPesonnelCode or GetByUsername base on unique keys
			foreach (DB.Contacts.Address address in memeber.ContactsAddresses) {
				Console.WriteLine("Country:{0} City:{1} Zipcode:{2} Address:{3}",
									address.CommonCity.Country.Title,
									address.CommonCity.Title,
									address.ZipCode,
									address.LocalAddress);
			}
			Console.ReadKey();
		}

		private static void LinqAndLazyLoading() {
			Console.WriteLine("* * Linq & Lazy loading");
			using (DB.Contacts.ContactsDataModel db = new DB.Contacts.ContactsDataModel()) {
				var query =
					from q in db.Messenger
					where
						q.Service == DB.Contacts.ContactsEnums.MessengerService.GTalk
					select q;

				foreach (DB.Contacts.Messenger messenger in query) {
					Console.WriteLine("{0}@gmail.com from {1} {2}", messenger.Username, messenger.MembersMember.Firstname, messenger.MembersMember.Lastname);
				}
			}
			Console.ReadKey();
		}

	}
}

// ----------------------------------------------------------------------------------------------------------
// Sogen - Code Generator for BLToolkit version 1.4.3.0
// Date Created: 2010-09-17 7:12:13 PM
// * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
//  This code was generated for BLToolkit
//  warning!! Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// ----------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace Sogen.Demo.DB.Contacts {
	public partial class ContactsDataModel : DbManager {
		/// <summary>
		/// Address
		/// </summary>
		public Table<Address> Address { get {return GetTable<Address>();} }
		/// <summary>
		/// Email
		/// </summary>
		public Table<Email> Email { get {return GetTable<Email>();} }
		/// <summary>
		/// Messenger
		/// </summary>
		public Table<Messenger> Messenger { get {return GetTable<Messenger>();} }
		
		public ContactsDataModel() : base ( new BLToolkit.Data.DataProvider.SqlDataProvider(), Sogen.Demo.DB.Configs.ContactsConnStr) { }
	} // ContactsDataModel
	
	#region Address
		/// <summary>
		/// Address
		/// </summary>
		[TableName(Owner = "Contacts", Name = "Address")]
		public partial class Address {
			
			private long _id;
			/// <summary>
			/// Address ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private long _memberID;
			/// <summary>
			/// Member
			/// </summary>
			[MapField("MemberID")]
			public long MemberID {
				get { return this._memberID; }
				set { this._memberID = value; }
			}
			
			private long _cityID;
			/// <summary>
			/// City
			/// </summary>
			[MapField("CityID")]
			public long CityID {
				get { return this._cityID; }
				set { this._cityID = value; }
			}
			
			private string _zipCode = string.Empty;
			/// <summary>
			/// Zip Code
			/// </summary>
			[MapField("ZipCode")]
			public string ZipCode {
				get { return this._zipCode; }
				set { this._zipCode = value; }
			}
			
			private string _localAddress = string.Empty;
			/// <summary>
			/// Local Address
			/// </summary>
			[MapField("LocalAddress")]
			public string LocalAddress {
				get { return this._localAddress; }
				set { this._localAddress = value; }
			}
			
			private Sogen.Demo.DB.Common.City _commonCity;
			/// <summary>
			///  FK_Address_City
			/// </summary>
			[Association(ThisKey = "CityID", OtherKey = "ID")]
			public Sogen.Demo.DB.Common.City CommonCity {
				get {
					if (this._commonCity == null)
						if (this._cityID != null)
							this._commonCity = Sogen.Demo.DB.Common.City.GetByID(this._cityID);
					return this._commonCity;
				}
				set { this._commonCity = value; }
			}
			
			private Sogen.Demo.DB.Members.Member _membersMember;
			/// <summary>
			///  FK_Address_Member
			/// </summary>
			[Association(ThisKey = "MemberID", OtherKey = "ID")]
			public Sogen.Demo.DB.Members.Member MembersMember {
				get {
					if (this._membersMember == null)
						if (this._memberID != null)
							this._membersMember = Sogen.Demo.DB.Members.Member.GetByID(this._memberID);
					return this._membersMember;
				}
				set { this._membersMember = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Contacts].[Address] (
								MemberID, CityID, ZipCode, LocalAddress )
							values (
								@MemberID, @CityID, @ZipCode, @LocalAddress );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Update [Contacts].[Address]  set 
							MemberID = @MemberID, 
							CityID = @CityID, 
							ZipCode = @ZipCode, 
							LocalAddress = @LocalAddress
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Address] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Address from db
			/// </summary>
			public static void Delete (
				long id ) {
				var address = new Address();
				address.ID = id;
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Address] 
						Where (
							ID = @ID );", db.CreateParameters(address))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Address from db
			/// </summary>
			public static Address GetByID (
				long id ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Address
						where
							q.ID == id
						select q;
					return query.Single<Address>();
				}
			} // GetByID
		} // Address
	#endregion Address
	
	#region Email
		/// <summary>
		/// Email
		/// </summary>
		[TableName(Owner = "Contacts", Name = "Email")]
		public partial class Email {
			
			private long _id;
			/// <summary>
			/// Email ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private long _memberID;
			/// <summary>
			/// Member
			/// </summary>
			[MapField("MemberID")]
			public long MemberID {
				get { return this._memberID; }
				set { this._memberID = value; }
			}
			
			private string _emailAddress = string.Empty;
			/// <summary>
			/// Email Address
			/// </summary>
			[MapField("EmailAddress")]
			public string EmailAddress {
				get { return this._emailAddress; }
				set { this._emailAddress = value; }
			}
			
			private bool _isApproved = false;
			/// <summary>
			/// Approved?
			/// </summary>
			[MapField("IsApproved")]
			public bool IsApproved {
				get { return this._isApproved; }
				set { this._isApproved = value; }
			}
			
			private Sogen.Demo.DB.Members.Member _membersMember;
			/// <summary>
			///  FK_Email_Member
			/// </summary>
			[Association(ThisKey = "MemberID", OtherKey = "ID")]
			public Sogen.Demo.DB.Members.Member MembersMember {
				get {
					if (this._membersMember == null)
						if (this._memberID != null)
							this._membersMember = Sogen.Demo.DB.Members.Member.GetByID(this._memberID);
					return this._membersMember;
				}
				set { this._membersMember = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Contacts].[Email] (
								MemberID, EmailAddress, IsApproved )
							values (
								@MemberID, @EmailAddress, @IsApproved );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Update [Contacts].[Email]  set 
							MemberID = @MemberID, 
							EmailAddress = @EmailAddress, 
							IsApproved = @IsApproved
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Email] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Email from db
			/// </summary>
			public static void Delete (
				long id ) {
				var email = new Email();
				email.ID = id;
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Email] 
						Where (
							ID = @ID );", db.CreateParameters(email))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Email from db
			/// </summary>
			public static Email GetByID (
				long id ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Email
						where
							q.ID == id
						select q;
					return query.Single<Email>();
				}
			} // GetByID
			
			/// <summary>
			/// Get a Email from db
			/// </summary>
			public static Email GetByEmailAddress (
				string emailAddress ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Email
						where
							q.EmailAddress == emailAddress
						select q;
					return query.Single<Email>();
				}
			} // GetByEmailAddress
		} // Email
	#endregion Email
	
	#region Messenger
		/// <summary>
		/// Messenger
		/// </summary>
		[TableName(Owner = "Contacts", Name = "Messenger")]
		public partial class Messenger {
			
			private long _id;
			/// <summary>
			/// Messenger ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private long _memberID;
			/// <summary>
			/// Member ID
			/// </summary>
			[MapField("MemberID")]
			public long MemberID {
				get { return this._memberID; }
				set { this._memberID = value; }
			}
			
			private string _username = string.Empty;
			/// <summary>
			/// Username in Service
			/// </summary>
			[MapField("Username")]
			public string Username {
				get { return this._username; }
				set { this._username = value; }
			}
			
			private ContactsEnums.MessengerService _service;
			/// <summary>
			/// Service Type
			/// </summary>
			[MapField("Service")]
			public ContactsEnums.MessengerService Service {
				get { return this._service; }
				set { this._service = value; }
			}
			
			private Sogen.Demo.DB.Members.Member _membersMember;
			/// <summary>
			///  FK_Messenger_Member
			/// </summary>
			[Association(ThisKey = "MemberID", OtherKey = "ID")]
			public Sogen.Demo.DB.Members.Member MembersMember {
				get {
					if (this._membersMember == null)
						if (this._memberID != null)
							this._membersMember = Sogen.Demo.DB.Members.Member.GetByID(this._memberID);
					return this._membersMember;
				}
				set { this._membersMember = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Contacts].[Messenger] (
								MemberID, Username, Service )
							values (
								@MemberID, @Username, @Service );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Update [Contacts].[Messenger]  set 
							MemberID = @MemberID, 
							Username = @Username, 
							Service = @Service
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Messenger] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Messenger from db
			/// </summary>
			public static void Delete (
				long id ) {
				var messenger = new Messenger();
				messenger.ID = id;
				using (ContactsDataModel db = new ContactsDataModel()) {
					db.SetCommand(@"
						Delete [Contacts].[Messenger] 
						Where (
							ID = @ID );", db.CreateParameters(messenger))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Messenger from db
			/// </summary>
			public static Messenger GetByID (
				long id ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Messenger
						where
							q.ID == id
						select q;
					return query.Single<Messenger>();
				}
			} // GetByID
			
			/// <summary>
			/// Get a Messenger from db
			/// </summary>
			public static Messenger GetByMemberIDAndService (
				long memberID, 
				ContactsEnums.MessengerService service ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Messenger
						where
							q.MemberID == memberID && q.Service == service
						select q;
					return query.Single<Messenger>();
				}
			} // GetByMemberIDAndService
			
			/// <summary>
			/// Get a Messenger from db
			/// </summary>
			public static Messenger GetByUsernameAndService (
				string username, 
				ContactsEnums.MessengerService service ) {
				using (ContactsDataModel db = new ContactsDataModel()) {
					var query =
						from q in db.Messenger
						where
							q.Username == username && q.Service == service
						select q;
					return query.Single<Messenger>();
				}
			} // GetByUsernameAndService
		} // Messenger
	#endregion Messenger
	
	#region ContactsEnums
		public partial class ContactsEnums {
			public enum MessengerService {
				[MapValue(1)] GTalk,
				[MapValue(2)] Yahoo,
				[MapValue(3)] MSN,
				[MapValue(4)] ICQ,
			}
		} // ContactsEnums
	#endregion ContactsEnums
} // Sogen.Demo.DB.Contacts

// ----------------------------------------------------------------------------------------------------------
// Sogen - Code Generator for BLToolkit version 1.4.4.0
// Date Created: 2011-10-12 5:03:52 PM
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

namespace Sogen.Demo.DB.Members {
	public partial class MembersDataModel : DbManager {
		/// <summary>
		/// Member
		/// </summary>
		public Table<Member> Member { get {return GetTable<Member>();} }
		
		public MembersDataModel() : base ( new BLToolkit.Data.DataProvider.SqlDataProvider(), Sogen.Demo.DB.Configs.MembersConnStr) { }
	} // MembersDataModel
	
	#region Member
		/// <summary>
		/// Member
		/// </summary>
		[TableName(Owner = "Members", Name = "Member")]
		public partial class Member {
			
			private long _id;
			/// <summary>
			/// Member ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private string _firstname = string.Empty;
			/// <summary>
			/// First name
			/// </summary>
			[MapField("Firstname")]
			public string Firstname {
				get { return this._firstname; }
				set { this._firstname = value; }
			}
			
			private string _lastname = string.Empty;
			/// <summary>
			/// Last name
			/// </summary>
			[MapField("Lastname")]
			public string Lastname {
				get { return this._lastname; }
				set { this._lastname = value; }
			}
			
			private DateTime _membershipDate = DateTime.Now;
			/// <summary>
			/// Membership Date
			/// </summary>
			[MapField("MembershipDate")]
			public DateTime MembershipDate {
				get { return this._membershipDate; }
				set { this._membershipDate = value; }
			}
			
			private string _username;
			/// <summary>
			/// Username
			/// </summary>
			[MapField("Username")]
			public string Username {
				get { return this._username; }
				set { this._username = value; }
			}
			
			private MembersEnums.Gender _gender = MembersEnums.Gender.Unknown;
			/// <summary>
			/// Gender
			/// </summary>
			[MapField("Gender")]
			public MembersEnums.Gender Gender {
				get { return this._gender; }
				set { this._gender = value; }
			}
			
			private string _pesonnelCode;
			/// <summary>
			/// Personal Code
			/// </summary>
			[MapField("PesonnelCode")]
			public string PesonnelCode {
				get { return this._pesonnelCode; }
				set { this._pesonnelCode = value; }
			}
			
			private MembersEnums.Office _office = MembersEnums.Office.Center;
			/// <summary>
			/// Office Code
			/// </summary>
			[MapField("Office")]
			public MembersEnums.Office Office {
				get { return this._office; }
				set { this._office = value; }
			}
			
			private DateTime? _birthday;
			/// <summary>
			/// Birthday
			/// </summary>
			[MapField("Birthday"), Nullable]
			public DateTime? Birthday {
				get { return this._birthday; }
				set { this._birthday = value; }
			}
			
			private long? _industryID;
			/// <summary>
			/// Industry
			/// </summary>
			[MapField("IndustryID"), Nullable]
			public long? IndustryID {
				get { return this._industryID; }
				set { this._industryID = value; }
			}
			
			private string _jobTitle = string.Empty;
			/// <summary>
			/// Job title
			/// </summary>
			[MapField("JobTitle")]
			public string JobTitle {
				get { return this._jobTitle; }
				set { this._jobTitle = value; }
			}
			
			private Sogen.Demo.DB.Common.Industry _commonIndustry;
			/// <summary>
			///  FK_Member_Industry
			/// </summary>
			[Association(ThisKey = "IndustryID", OtherKey = "ID")]
			public Sogen.Demo.DB.Common.Industry CommonIndustry {
				get {
					if (this._commonIndustry == null)
						if (this._industryID != null)
							this._commonIndustry = Sogen.Demo.DB.Common.Industry.GetByID(this._industryID.Value);
					return this._commonIndustry;
				}
				set { this._commonIndustry = value; }
			}
			
			private List<Sogen.Demo.DB.Contacts.Address> _contactsAddresses;
			/// <summary>
			///  FK_Address_Member_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "MemberID")]
			public List<Sogen.Demo.DB.Contacts.Address> ContactsAddresses {
				get {
					if (this._contactsAddresses == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Contacts.ContactsDataModel db = new Sogen.Demo.DB.Contacts.ContactsDataModel()) {
								var query =
									from q in db.Address
									where
										q.MemberID == this._id 
									select q;
								this._contactsAddresses = query.ToList<Sogen.Demo.DB.Contacts.Address>();
							}
					return this._contactsAddresses;
				}
				set { this._contactsAddresses = value; }
			}
			
			private List<Sogen.Demo.DB.Contacts.Email> _contactsEmails;
			/// <summary>
			///  FK_Email_Member_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "MemberID")]
			public List<Sogen.Demo.DB.Contacts.Email> ContactsEmails {
				get {
					if (this._contactsEmails == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Contacts.ContactsDataModel db = new Sogen.Demo.DB.Contacts.ContactsDataModel()) {
								var query =
									from q in db.Email
									where
										q.MemberID == this._id 
									select q;
								this._contactsEmails = query.ToList<Sogen.Demo.DB.Contacts.Email>();
							}
					return this._contactsEmails;
				}
				set { this._contactsEmails = value; }
			}
			
			private List<Sogen.Demo.DB.Contacts.Messenger> _contactsMessengers;
			/// <summary>
			///  FK_Messenger_Member_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "MemberID")]
			public List<Sogen.Demo.DB.Contacts.Messenger> ContactsMessengers {
				get {
					if (this._contactsMessengers == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Contacts.ContactsDataModel db = new Sogen.Demo.DB.Contacts.ContactsDataModel()) {
								var query =
									from q in db.Messenger
									where
										q.MemberID == this._id 
									select q;
								this._contactsMessengers = query.ToList<Sogen.Demo.DB.Contacts.Messenger>();
							}
					return this._contactsMessengers;
				}
				set { this._contactsMessengers = value; }
			}
			
			private List<Sogen.Demo.DB.Messages.Message> _messagesMessagesByFromMember;
			/// <summary>
			///  FK_Message_MemberFrom_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "FromMember")]
			public List<Sogen.Demo.DB.Messages.Message> MessagesMessagesByFromMember {
				get {
					if (this._messagesMessagesByFromMember == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Messages.MessagesDataModel db = new Sogen.Demo.DB.Messages.MessagesDataModel()) {
								var query =
									from q in db.Message
									where
										q.FromMember == this._id 
									select q;
								this._messagesMessagesByFromMember = query.ToList<Sogen.Demo.DB.Messages.Message>();
							}
					return this._messagesMessagesByFromMember;
				}
				set { this._messagesMessagesByFromMember = value; }
			}
			
			private List<Sogen.Demo.DB.Messages.Message> _messagesMessagesByToMember;
			/// <summary>
			///  FK_Message_MemberTo_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "ToMember")]
			public List<Sogen.Demo.DB.Messages.Message> MessagesMessagesByToMember {
				get {
					if (this._messagesMessagesByToMember == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Messages.MessagesDataModel db = new Sogen.Demo.DB.Messages.MessagesDataModel()) {
								var query =
									from q in db.Message
									where
										q.ToMember == this._id 
									select q;
								this._messagesMessagesByToMember = query.ToList<Sogen.Demo.DB.Messages.Message>();
							}
					return this._messagesMessagesByToMember;
				}
				set { this._messagesMessagesByToMember = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			internal virtual void Insert() {
				using (MembersDataModel db = new MembersDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Members].[Member] (
								Firstname, Lastname, MembershipDate, Username, Gender, PesonnelCode, Office, Birthday, IndustryID, JobTitle )
							values (
								@Firstname, @Lastname, @MembershipDate, @Username, @Gender, @PesonnelCode, @Office, @Birthday, @IndustryID, @JobTitle );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			internal virtual void Update() {
				using (MembersDataModel db = new MembersDataModel()) {
					db.SetCommand(@"
						Update [Members].[Member]  set 
							Firstname = @Firstname, 
							Lastname = @Lastname, 
							MembershipDate = @MembershipDate, 
							Username = @Username, 
							Gender = @Gender, 
							PesonnelCode = @PesonnelCode, 
							Office = @Office, 
							Birthday = @Birthday, 
							IndustryID = @IndustryID, 
							JobTitle = @JobTitle
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			internal virtual void Delete() {
				using (MembersDataModel db = new MembersDataModel()) {
					db.SetCommand(@"
						Delete [Members].[Member] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Member from db
			/// </summary>
			internal static void Delete (
				long id ) {
				var member = new Member();
				member.ID = id;
				using (MembersDataModel db = new MembersDataModel()) {
					db.SetCommand(@"
						Delete [Members].[Member] 
						Where (
							ID = @ID );", db.CreateParameters(member))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Member from db
			/// </summary>
			internal static Member GetByID (
				long id ) {
				using (MembersDataModel db = new MembersDataModel()) {
					var query =
						from q in db.Member
						where
							q.ID == id
						select q;
					try {
						return query.Single<Member>();
					}
					catch (Exception) {
						return null;
					}
				}
			} // GetByID
			
			/// <summary>
			/// Get a Member from db
			/// </summary>
			internal static Member GetByOfficeAndPesonnelCode (
				MembersEnums.Office office, 
				string pesonnelCode ) {
				using (MembersDataModel db = new MembersDataModel()) {
					var query =
						from q in db.Member
						where
							q.Office == office && q.PesonnelCode == pesonnelCode
						select q;
					try {
						return query.Single<Member>();
					}
					catch (Exception) {
						return null;
					}
				}
			} // GetByOfficeAndPesonnelCode
			
			/// <summary>
			/// Get a Member from db
			/// </summary>
			internal static Member GetByUsername (
				string username ) {
				using (MembersDataModel db = new MembersDataModel()) {
					var query =
						from q in db.Member
						where
							q.Username == username
						select q;
					try {
						return query.Single<Member>();
					}
					catch (Exception) {
						return null;
					}
				}
			} // GetByUsername
		} // Member
	#endregion Member
	
	#region MembersEnums
		public partial class MembersEnums {
			public enum Gender {
				[MapValue(0)] Unknown = 0,
				[MapValue(1)] Male = 1,
				[MapValue(2)] Female = 2,
			}
			public enum Office {
				[MapValue(1)] Center = 1,
				[MapValue(2)] SecondOffice = 2,
			}
		} // MembersEnums
	#endregion MembersEnums
} // Sogen.Demo.DB.Members

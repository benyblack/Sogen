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

namespace Sogen.Demo.DB.Common {
	public partial class CommonDataModel : DbManager {
		/// <summary>
		/// City
		/// </summary>
		public Table<City> City { get {return GetTable<City>();} }
		/// <summary>
		/// Country
		/// </summary>
		public Table<Country> Country { get {return GetTable<Country>();} }
		/// <summary>
		/// Industry
		/// </summary>
		public Table<Industry> Industry { get {return GetTable<Industry>();} }
		
		public CommonDataModel() : base ( new BLToolkit.Data.DataProvider.SqlDataProvider(), Sogen.Demo.DB.Configs.CommonConnStr) { }
	} // CommonDataModel
	
	#region City
		/// <summary>
		/// City
		/// </summary>
		[TableName(Owner = "Common", Name = "City")]
		public partial class City {
			
			private long _id;
			/// <summary>
			/// City ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private string _title = string.Empty;
			/// <summary>
			/// City name
			/// </summary>
			[MapField("Title")]
			public string Title {
				get { return this._title; }
				set { this._title = value; }
			}
			
			private long _countyID;
			/// <summary>
			/// Country
			/// </summary>
			[MapField("CountyID")]
			public long CountyID {
				get { return this._countyID; }
				set { this._countyID = value; }
			}
			
			private Country _country;
			/// <summary>
			///  FK_City_Country
			/// </summary>
			[Association(ThisKey = "CountyID", OtherKey = "ID")]
			public Country Country {
				get {
					if (this._country == null)
						if (this._countyID != null)
							this._country = Sogen.Demo.DB.Common.Country.GetByID(this._countyID);
					return this._country;
				}
				set { this._country = value; }
			}
			
			private List<Sogen.Demo.DB.Contacts.Address> _contactsAddresses;
			/// <summary>
			///  FK_Address_City_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "CityID")]
			public List<Sogen.Demo.DB.Contacts.Address> ContactsAddresses {
				get {
					if (this._contactsAddresses == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Contacts.ContactsDataModel db = new Sogen.Demo.DB.Contacts.ContactsDataModel()) {
								var query =
									from q in db.Address
									where
										q.CityID == this._id 
									select q;
								this._contactsAddresses = query.ToList<Sogen.Demo.DB.Contacts.Address>();
							}
					return this._contactsAddresses;
				}
				set { this._contactsAddresses = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (CommonDataModel db = new CommonDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Common].[City] (
								Title, CountyID )
							values (
								@Title, @CountyID );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Update [Common].[City]  set 
							Title = @Title, 
							CountyID = @CountyID
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[City] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a City from db
			/// </summary>
			public static void Delete (
				long id ) {
				var city = new City();
				city.ID = id;
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[City] 
						Where (
							ID = @ID );", db.CreateParameters(city))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a City from db
			/// </summary>
			public static City GetByID (
				long id ) {
				using (CommonDataModel db = new CommonDataModel()) {
					var query =
						from q in db.City
						where
							q.ID == id
						select q;
					return query.Single<City>();
				}
			} // GetByID
		} // City
	#endregion City
	
	#region Country
		/// <summary>
		/// Country
		/// </summary>
		[TableName(Owner = "Common", Name = "Country")]
		public partial class Country {
			
			private long _id;
			/// <summary>
			/// Country ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private string _title = string.Empty;
			/// <summary>
			/// Country name
			/// </summary>
			[MapField("Title")]
			public string Title {
				get { return this._title; }
				set { this._title = value; }
			}
			
			private string _iSOCode;
			/// <summary>
			/// Standard ISO 3 Code
			/// </summary>
			[MapField("ISOCode")]
			public string ISOCode {
				get { return this._iSOCode; }
				set { this._iSOCode = value; }
			}
			
			private string _description = string.Empty;
			/// <summary>
			/// Description
			/// </summary>
			[MapField("Description")]
			public string Description {
				get { return this._description; }
				set { this._description = value; }
			}
			
			private List<City> _cities;
			/// <summary>
			///  FK_City_Country_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "CountyID")]
			public List<City> Cities {
				get {
					if (this._cities == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Common.CommonDataModel db = new Sogen.Demo.DB.Common.CommonDataModel()) {
								var query =
									from q in db.City
									where
										q.CountyID == this._id 
									select q;
								this._cities = query.ToList<City>();
							}
					return this._cities;
				}
				set { this._cities = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (CommonDataModel db = new CommonDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Common].[Country] (
								Title, ISOCode, Description )
							values (
								@Title, @ISOCode, @Description );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Update [Common].[Country]  set 
							Title = @Title, 
							ISOCode = @ISOCode, 
							Description = @Description
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[Country] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Country from db
			/// </summary>
			public static void Delete (
				long id ) {
				var country = new Country();
				country.ID = id;
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[Country] 
						Where (
							ID = @ID );", db.CreateParameters(country))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Country from db
			/// </summary>
			public static Country GetByID (
				long id ) {
				using (CommonDataModel db = new CommonDataModel()) {
					var query =
						from q in db.Country
						where
							q.ID == id
						select q;
					return query.Single<Country>();
				}
			} // GetByID
			
			/// <summary>
			/// Get a Country from db
			/// </summary>
			public static Country GetByISOCode (
				string iSOCode ) {
				using (CommonDataModel db = new CommonDataModel()) {
					var query =
						from q in db.Country
						where
							q.ISOCode == iSOCode
						select q;
					return query.Single<Country>();
				}
			} // GetByISOCode
		} // Country
	#endregion Country
	
	#region Industry
		/// <summary>
		/// Industry
		/// </summary>
		[TableName(Owner = "Common", Name = "Industry")]
		public partial class Industry {
			
			private long _id;
			/// <summary>
			/// Industry ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private string _title = string.Empty;
			/// <summary>
			/// Industry Title
			/// </summary>
			[MapField("Title")]
			public string Title {
				get { return this._title; }
				set { this._title = value; }
			}
			
			private bool _isVisible = true;
			/// <summary>
			/// Is Visible?
			/// </summary>
			[MapField("IsVisible")]
			public bool IsVisible {
				get { return this._isVisible; }
				set { this._isVisible = value; }
			}
			
			private List<Sogen.Demo.DB.Members.Member> _membersMembers;
			/// <summary>
			///  FK_Member_Industry_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "IndustryID")]
			public List<Sogen.Demo.DB.Members.Member> MembersMembers {
				get {
					if (this._membersMembers == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Members.MembersDataModel db = new Sogen.Demo.DB.Members.MembersDataModel()) {
								var query =
									from q in db.Member
									where
										q.IndustryID == this._id 
									select q;
								this._membersMembers = query.ToList<Sogen.Demo.DB.Members.Member>();
							}
					return this._membersMembers;
				}
				set { this._membersMembers = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			public virtual void Insert() {
				using (CommonDataModel db = new CommonDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Common].[Industry] (
								Title, IsVisible )
							values (
								@Title, @IsVisible );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			public virtual void Update() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Update [Common].[Industry]  set 
							Title = @Title, 
							IsVisible = @IsVisible
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			public virtual void Delete() {
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[Industry] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Industry from db
			/// </summary>
			public static void Delete (
				long id ) {
				var industry = new Industry();
				industry.ID = id;
				using (CommonDataModel db = new CommonDataModel()) {
					db.SetCommand(@"
						Delete [Common].[Industry] 
						Where (
							ID = @ID );", db.CreateParameters(industry))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Industry from db
			/// </summary>
			public static Industry GetByID (
				long id ) {
				using (CommonDataModel db = new CommonDataModel()) {
					var query =
						from q in db.Industry
						where
							q.ID == id
						select q;
					return query.Single<Industry>();
				}
			} // GetByID
		} // Industry
	#endregion Industry
} // Sogen.Demo.DB.Common

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

namespace Sogen.Demo.DB.Messages {
	public partial class MessagesDataModel : DbManager {
		/// <summary>
		/// Message
		/// </summary>
		public Table<Message> Message { get {return GetTable<Message>();} }
		/// <summary>
		/// Type
		/// </summary>
		public Table<Type> Type { get {return GetTable<Type>();} }
		
		public MessagesDataModel() : base ( new BLToolkit.Data.DataProvider.SqlDataProvider(), Sogen.Demo.DB.Configs.MessagesConnStr) { }
	} // MessagesDataModel
	
	#region Message
		/// <summary>
		/// Message
		/// </summary>
		[TableName(Owner = "Messages", Name = "Message")]
		public partial class Message {
			
			private long _id;
			/// <summary>
			/// Mesaage ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private long _fromMember;
			/// <summary>
			/// Sender
			/// </summary>
			[MapField("FromMember")]
			public long FromMember {
				get { return this._fromMember; }
				set { this._fromMember = value; }
			}
			
			private long _toMember;
			/// <summary>
			/// Reciever
			/// </summary>
			[MapField("ToMember")]
			public long ToMember {
				get { return this._toMember; }
				set { this._toMember = value; }
			}
			
			private string _messageBody = string.Empty;
			/// <summary>
			/// Message Text
			/// </summary>
			[MapField("MessageBody")]
			public string MessageBody {
				get { return this._messageBody; }
				set { this._messageBody = value; }
			}
			
			private DateTime _sendDate = DateTime.Now;
			/// <summary>
			/// Send date
			/// </summary>
			[MapField("SendDate")]
			public DateTime SendDate {
				get { return this._sendDate; }
				set { this._sendDate = value; }
			}
			
			private long? _parentMessage;
			/// <summary>
			/// parent Message
			/// </summary>
			[MapField("ParentMessage"), Nullable]
			public long? ParentMessage {
				get { return this._parentMessage; }
				set { this._parentMessage = value; }
			}
			
			private long _messageType = 0;
			/// <summary>
			/// Type of message
			/// </summary>
			[MapField("MessageType")]
			public long MessageType {
				get { return this._messageType; }
				set { this._messageType = value; }
			}
			
			private int _replyCount = 0;
			/// <summary>
			/// Count of Reply
			/// </summary>
			[MapField("ReplyCount")]
			public int ReplyCount {
				get { return this._replyCount; }
				set { this._replyCount = value; }
			}
			
			private Sogen.Demo.DB.Members.Member _membersMemberByFromMember;
			/// <summary>
			///  FK_Message_MemberFrom
			/// </summary>
			[Association(ThisKey = "FromMember", OtherKey = "ID")]
			public Sogen.Demo.DB.Members.Member MembersMemberByFromMember {
				get {
					if (this._membersMemberByFromMember == null)
						if (this._fromMember != null)
							this._membersMemberByFromMember = Sogen.Demo.DB.Members.Member.GetByID(this._fromMember);
					return this._membersMemberByFromMember;
				}
				set { this._membersMemberByFromMember = value; }
			}
			
			private Sogen.Demo.DB.Members.Member _membersMemberByToMember;
			/// <summary>
			///  FK_Message_MemberTo
			/// </summary>
			[Association(ThisKey = "ToMember", OtherKey = "ID")]
			public Sogen.Demo.DB.Members.Member MembersMemberByToMember {
				get {
					if (this._membersMemberByToMember == null)
						if (this._toMember != null)
							this._membersMemberByToMember = Sogen.Demo.DB.Members.Member.GetByID(this._toMember);
					return this._membersMemberByToMember;
				}
				set { this._membersMemberByToMember = value; }
			}
			
			private Type _type;
			/// <summary>
			///  FK_Message_Type
			/// </summary>
			[Association(ThisKey = "MessageType", OtherKey = "ID")]
			public Type Type {
				get {
					if (this._type == null)
						if (this._messageType != null)
							this._type = Sogen.Demo.DB.Messages.Type.GetByID(this._messageType);
					return this._type;
				}
				set { this._type = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			internal virtual void Insert() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Messages].[Message] (
								FromMember, ToMember, MessageBody, SendDate, ParentMessage, MessageType, ReplyCount )
							values (
								@FromMember, @ToMember, @MessageBody, @SendDate, @ParentMessage, @MessageType, @ReplyCount );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			internal virtual void Update() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Update [Messages].[Message]  set 
							FromMember = @FromMember, 
							ToMember = @ToMember, 
							MessageBody = @MessageBody, 
							SendDate = @SendDate, 
							ParentMessage = @ParentMessage, 
							MessageType = @MessageType, 
							ReplyCount = @ReplyCount
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			internal virtual void Delete() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Delete [Messages].[Message] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Message from db
			/// </summary>
			internal static void Delete (
				long id ) {
				var message = new Message();
				message.ID = id;
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Delete [Messages].[Message] 
						Where (
							ID = @ID );", db.CreateParameters(message))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Message from db
			/// </summary>
			internal static Message GetByID (
				long id ) {
				using (MessagesDataModel db = new MessagesDataModel()) {
					var query =
						from q in db.Message
						where
							q.ID == id
						select q;
					try {
						return query.Single<Message>();
					}
					catch (Exception) {
						return null;
					}
				}
			} // GetByID
		} // Message
	#endregion Message
	
	#region Type
		/// <summary>
		/// Type
		/// </summary>
		[TableName(Owner = "Messages", Name = "Type")]
		public partial class Type {
			
			private long _id;
			/// <summary>
			/// Message Type ID
			/// </summary>
			[MapField("ID"), Identity, PrimaryKey(1)]
			public long ID {
				get { return this._id; }
				set { this._id = value; }
			}
			
			private string _title;
			/// <summary>
			/// Message Type Title
			/// </summary>
			[MapField("Title")]
			public string Title {
				get { return this._title; }
				set { this._title = value; }
			}
			
			private bool _isActive = true;
			/// <summary>
			/// Is Active?
			/// </summary>
			[MapField("IsActive")]
			public bool IsActive {
				get { return this._isActive; }
				set { this._isActive = value; }
			}
			
			private List<Message> _messages;
			/// <summary>
			///  FK_Message_Type_BackReference
			/// </summary>
			[Association(ThisKey = "ID", OtherKey = "MessageType")]
			public List<Message> Messages {
				get {
					if (this._messages == null)
						if (this._id != null)
							using (Sogen.Demo.DB.Messages.MessagesDataModel db = new Sogen.Demo.DB.Messages.MessagesDataModel()) {
								var query =
									from q in db.Message
									where
										q.MessageType == this._id 
									select q;
								this._messages = query.ToList<Message>();
							}
					return this._messages;
				}
				set { this._messages = value; }
			}
			
			/// <summary>
			/// Insert this instance to db
			/// </summary>
			internal virtual void Insert() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					this._id = 
						db.SetCommand(@"
							Insert into [Messages].[Type] (
								Title, IsActive )
							values (
								@Title, @IsActive );
							 SELECT Cast(SCOPE_IDENTITY() as bigint) ID
							", db.CreateParameters(this))
							 .ExecuteScalar<long>();
					}
			} // Insert
			
			/// <summary>
			/// Update this instance in db
			/// </summary>
			internal virtual void Update() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Update [Messages].[Type]  set 
							Title = @Title, 
							IsActive = @IsActive
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Update
			
			/// <summary>
			/// Delete this instance from db
			/// </summary>
			internal virtual void Delete() {
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Delete [Messages].[Type] 
						 Where (
							ID = @ID );", db.CreateParameters(this))
					.ExecuteNonQuery();
					}
			} // Delete
			
			/// <summary>
			/// Delete a Type from db
			/// </summary>
			internal static void Delete (
				long id ) {
				var type = new Type();
				type.ID = id;
				using (MessagesDataModel db = new MessagesDataModel()) {
					db.SetCommand(@"
						Delete [Messages].[Type] 
						Where (
							ID = @ID );", db.CreateParameters(type))
						.ExecuteNonQuery();
					}
			} // Static Delete
			
			/// <summary>
			/// Get a Type from db
			/// </summary>
			internal static Type GetByID (
				long id ) {
				using (MessagesDataModel db = new MessagesDataModel()) {
					var query =
						from q in db.Type
						where
							q.ID == id
						select q;
					try {
						return query.Single<Type>();
					}
					catch (Exception) {
						return null;
					}
				}
			} // GetByID
		} // Type
	#endregion Type
} // Sogen.Demo.DB.Messages

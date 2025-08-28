using Data;
using System.Data;
namespace Web_Service
{
    public class MessageRepository : Repository, IRepository<Message>
    {
        public MessageRepository(DBContext dbContext) : base(dbContext)
        {

        }

        //public string CipherTextBase64 { get; set; }
        //public string IVBase64 { get; set; }
        //public string HmacBase64 { get; set; }
        //public string SignatureBase64 { get; set; }
        //public string SenderPublicKeyBase64 { get; set; }
        //public string SenderSigningKeyBase64 { get; set; }

        //public string? ChatID { get; set; }
        //public string? Attachments { get; set; }
        //public string? SenderID { get; set; }
        //public DateTime? SentAt { get; set; }

        public bool Create(Message message)
        {
            this.dbContext.ClearParameters();
            string sql = @$"INSERT INTO Message(MessageID, SenderID, SentAt, CipherTextBase64, Attachments, ChatID, IVBase64,
                                        HmacBase64, SignatureBase64, SenderPublicKeyBase64, SenderSigningKeyBase64)
                   VALUES(@MessageID, @SenderID, #{message.SentAt}#, @CipherTextBase64, @Attachments, 
                          @ChatID, @IVBase64, @HmacBase64, @SignatureBase64, @SenderPublicKeyBase64, @SenderSigningKeyBase64)";

            base.dbContext.AddParameters("@MessageID", message.ID);
            base.dbContext.AddParameters("@SenderID", message.SenderID);
            base.dbContext.AddParameters("@CipherTextBase64", message.CipherTextBase64);
            base.dbContext.AddParameters("@Attachments", message.Attachments);
            base.dbContext.AddParameters("@ChatID", message.ChatID);
            base.dbContext.AddParameters("@IVBase64", message.IVBase64);
            base.dbContext.AddParameters("@HmacBase64", message.HmacBase64);
            base.dbContext.AddParameters("@SignatureBase64", message.SignatureBase64);
            base.dbContext.AddParameters("@SenderPublicKeyBase64", message.SenderPublicKeyBase64);
            base.dbContext.AddParameters("@SenderSigningKeyBase64", message.SenderSigningKeyBase64);

            return base.dbContext.Create(sql) > 0;
        }

        public bool Update(Message message)
        {
            string sql = $@"UPDATE Message SET SenderID = @SenderID, 
                                               SentAt = @SentAt, 
                                               CipherTextBase64 = @CipherTextBase64, 
                                               Attachments = @Attachments
                                            WHERE MessageID = @MessageID";
            base.dbContext.AddParameters("@MessageID", message.ID);
            base.dbContext.AddParameters("@SenderID", message.SenderID);
            base.dbContext.AddParameters("@SentAt", message.SentAt.ToString());
            base.dbContext.AddParameters("@CipherTextBase64", message.CipherTextBase64);
            base.dbContext.AddParameters("@Attachments", message.Attachments);
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(Message message)
        {
            string sql = $@"DELETE FROM Message WHERE MessageID = @MessageID";
            base.dbContext.AddParameters("@MessageID", message.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Message WHERE MessageID = @MessageID";
            base.dbContext.AddParameters("@MessageID", ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public Message GetByID(string ID)
        {
            string sql = $@"SELECT * FROM Message WHERE MessageID = @MessageID";
            base.dbContext.AddParameters("@MessageID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.MessageCreator.CreateModel(reader);
            }
        }

        public List<Message> GetAll()
        {
            string sql = $@"SELECT * FROM Message";
            List<Message> messages = new List<Message>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    messages.Add(this.modelFactory.MessageCreator.CreateModel(reader));
                }
                return messages;
            }
        }

        public List<Message> Conversation(string ChatID)
        {
            List<Message> messages = new List<Message>();
            string sql = $@"SELECT * FROM Message WHERE ChatID = '{ChatID}' ORDER BY SentAt ASC";
            //base.dbContext.AddParameters("@ChatID", ChatID);
            Console.WriteLine("sql -> " + sql);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    messages.Add(this.modelFactory.MessageCreator.CreateModel(reader));
                }
                return messages;
            }
        }

        public List<string> AttachmentsCheck(List<Message> messages)
        {
            List<string> strings = new List<string>();
            foreach (Message message in messages)
            {
                if (!message.Attachments.Equals("NONE"))
                {
                    strings.Add(message.Attachments);
                }
            }
            return strings;
        }

        public List<Message> GroupConversation(string ChatID)
        {
            string sql = $@"SELECT  Message.SenderID, 
                                    Message.SentAt, 
                                    Message.CipherTextBase64, 
                                    Message.ChatID, 
                                    Message.Attachments 
                            FROM Message LEFT JOIN GroupChat ON Message.ChatID = GroupChat.ChatID
                            WHERE GroupChat.GroupChatID = @ChatID";
            List<Message> messages = new List<Message>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    messages.Add(this.modelFactory.MessageCreator.CreateModel(reader));
                }
                return messages;
            }
        }

        public Message GetLastMessages(string UserID,string ChatID)
        {
            string sql = $@"SELECT TOP 1 Message.* FROM Message LEFT JOIN Chat
                            ON Message.ChatID = Chat.ChatID WHERE 
                            (Message.FirstUserID = '{UserID}' OR 
                            Message.SecondUserID = '{UserID}') AND 
                            Chat.ChatID = '{ChatID}' ORDER BY SentAt DESC";
            //base.dbContext.AddParameters("@UserID", UserID);
            //base.dbContext.AddParameters("@ChatID", ChatID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.MessageCreator.CreateModel(reader);
            }
        }           
        public Message GetLastGroupMessages(string UserID, string ChatID)
        {
            string sql = $@"SELECT TOP 1 * FROM Message LEFT JOIN GroupChat
                            ON Message.ChatID = GroupChat.ChatID WHERE 
                            (Message.FirstUserID = @UserID OR 
                            Message.SecondUserID = @UserID) AND 
                            Chat.GroupChatID = @ChatID ORDER BY SentAt DESC";
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.MessageCreator.CreateModel(reader);
            }
        }

        
    }
}

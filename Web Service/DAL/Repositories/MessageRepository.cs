using Data;
using System.Data;
namespace Web_Service
{
    public class MessageRepository : Repository, IRepository<Message>
    {
        public MessageRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Message message)
        {
            string sql = $@"INSERT INTO Message(MessageID,SenderID ,SentAt ,Content ,Attachments)
                                        VALUES(@MessageID,@SenderID,@SentAt,@Content,@Attachments)";
            base.dbContext.AddParameters("@MessageID", message.ID);
            base.dbContext.AddParameters("@SenderID", message.SenderID);
            base.dbContext.AddParameters("@SentAt", message.SentAt.ToString());
            base.dbContext.AddParameters("@Content", message.Content);
            base.dbContext.AddParameters("@Attachments", message.Attachments);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Message message)
        {
            string sql = $@"UPDATE Message SET SenderID = @SenderID, 
                                               SentAt = @SentAt, 
                                               Content = @Content, 
                                               Attachments = @Attachments
                                            WHERE MessageID = @MessageID";
            base.dbContext.AddParameters("@MessageID", message.ID);
            base.dbContext.AddParameters("@SenderID", message.SenderID);
            base.dbContext.AddParameters("@SentAt", message.SentAt.ToString());
            base.dbContext.AddParameters("@Content", message.Content);
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
            string sql = $@"SELECT * FROM Message WHERE ChatID = '{ChatID}'";
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
                                    Message.Content, 
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
            string sql = $@"SELECT TOP 1 * FROM Message LEFT JOIN Chat
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

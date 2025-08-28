using Data;
using System;
using System.Collections.Generic;
using System.Data;
namespace Web_Service
{
    public class ChatRepository : Repository, IRepository<Chat>
    {
        public ChatRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Chat chat)
        {
            string sql = $@"INSERT INTO Chat(ChatID, ChatName, Creator, CreationDate,ChatIMG, ChatDescription, FirstUserID, SecondUserID)
                                       VALUES(@ChatID, @ChatName, @Creator, @CreationDate, @ChatIMG, @ChatDescription, @FirstUserID, @SecondUserID)";
            base.dbContext.AddParameters("@ChatID", chat.ID);
            base.dbContext.AddParameters("@ChatName", chat.ChatName);
            base.dbContext.AddParameters("@Creator", chat.Creator);
            base.dbContext.AddParameters("@CreationDate", chat.CreationDate.ToString());
            return base.dbContext.Create(sql) > 0;
        }
        public Chat GetByID(string ID)
        {
            string sql = $@"SELECT * FROM Chat WHERE ChatID = @ChatID";
            base.dbContext.AddParameters("@ChatID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.ChatCreator.CreateModel(reader);
            }
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Chat WHERE ChatID = @ChatID";
            base.dbContext.AddParameters("@ChatID", ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool Update(Chat chat)
        {
            string sql = $@"UPDATE Chat SET ChatName = @ChatName, 
                                            Creator = @Creator, 
                                            CreationDate = @CreationDate,
                                            ChatIMG = @ChatIMG,
                                            ChatDescription = @ChatDescription,
                                            FirstUserID = @FirstUserID,
                                            SecondUserID = @SecondUserID
                                        WHERE ChatID = @ChatID";
            base.dbContext.AddParameters("@ChatID", chat.ID);
            base.dbContext.AddParameters("@ChatName", chat.ChatName);
            base.dbContext.AddParameters("@Creator", chat.Creator);
            base.dbContext.AddParameters("@CreationDate", chat.CreationDate.ToString());
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(Chat chat)
        {
            string sql = $@"DELETE FROM Chat WHERE ChatID = @ChatID";
            base.dbContext.AddParameters("@ChatID", chat.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public List<Chat> GetAll()
        {
            List<Chat> Chats = new List<Chat>();
            string sql = $@"SELECT * FROM Chat";
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Chats.Add(this.modelFactory.ChatCreator.CreateModel(reader));
                }
            }
            return Chats;
        }

        public List<Chat> GetBySearch(string ChatName, string UserID)
        {
            string sql = $@"SELECT * FROM Chat 
                                               WHERE Chat.ChatName LIKE @ChatName 
                                               AND (Chat.FirstUserID = @UserID OR Chat.SecondUserID = @UserID)";
            base.dbContext.AddParameters("@ChatName", '%' + ChatName + '%');
            base.dbContext.AddParameters("@UserID", UserID);
            Console.WriteLine(sql);
            List<Chat> Chats = new List<Chat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Chats.Add(this.modelFactory.ChatCreator.CreateModel(reader));
                }
            }
            return Chats;
        }
        public List<Chat> GetChatsByUser(string ID)
        {
            string sql = $@"SELECT * FROM Chat WHERE FirstUserID = {ID} OR SecondUserID = {ID}";
            base.dbContext.AddParameters("@UserID", ID);
            List<Chat> Chats = new List<Chat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Chats.Add(this.modelFactory.ChatCreator.CreateModel(reader));
                }
            }
            return Chats;
        }

        public List<Chat> GetArchived(string UserID)
        {
            string sql = $@"SELECT Chat.* FROM Chat 
                            LEFT JOIN Archive ON Chat.ChatID = Archive.ChatID
                            WHERE Archive.UserID = {UserID}";
            base.dbContext.AddParameters("@UserID", UserID);
            List<Chat> Chats = new List<Chat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Chats.Add(this.modelFactory.ChatCreator.CreateModel(reader));
                }
            }
            return Chats;
        }

        public List<Chat> GetByUnread(string UserID)
        {
            string sql = $@"SELECT DISTINCT Chat.* FROM Chat
                            JOIN Message ON Chat.ChatID = Message.ChatID
                            LEFT JOIN ReadMessage ON ReadMessage.MessageID = Message.MessageID AND ReadMessage.UserID = '{UserID}'
                            WHERE ReadMessage.ReadAt IS NULL
                            AND (Chat.FirstUserID = '{UserID}' OR Chat.SecondUserID = '{UserID}')";
            List<Chat> chats = new List<Chat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    chats.Add(this.modelFactory.ChatCreator.CreateModel(reader));
                }
                return chats;
            }

        }
    }
}

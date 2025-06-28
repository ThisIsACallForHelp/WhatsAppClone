using Data;
using System.Data;
namespace Web_Service
{
    public class GroupChatRepository : Repository, IRepository<GroupChat>
    {
        public GroupChatRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(GroupChat groupChat)
        {
            string sql = $@"INSERT INTO GroupChat(GroupChatID, ChatID, Users,GroupChatName)
                                        VALUES(@GroupChatID,@ChatID,@Users, @GroupChatName)";
            base.dbContext.AddParameters("@GroupChatID", groupChat.ID);
            base.dbContext.AddParameters("@ChatID", groupChat.ChatID);
            base.dbContext.AddParameters("@Users", groupChat.Users);
            base.dbContext.AddParameters("@GroupChatName", groupChat.GroupChatName);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Delete(GroupChat groupChat)
        {
            string sql = $@"DELETE FROM GroupChat WHERE GroupChatID = @GroupChatID";
            base.dbContext.AddParameters("@GroupChatID", groupChat.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM GroupChat WHERE GroupChatID = @GroupChatID";
            base.dbContext.AddParameters("@GroupChatID", ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public GroupChat GetByID(string ID)
        {
            string sql = $@"SELECT * FROM GroupChat WHERE GroupChatID = @GroupChatID";
            base.dbContext.AddParameters("@GroupChatID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.GroupChatCreator.CreateModel(reader);  
            }
        }
        public List<GroupChat> GetAll()
        {
            List<GroupChat> groupChats = new List<GroupChat>();
            string sql = $@"SELECT * FROM GroupChat";
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    groupChats.Add(this.modelFactory.GroupChatCreator.CreateModel(reader));
                }
                return groupChats;
            }
        }
        public bool Update(GroupChat groupChat)
        {
            string sql = $@"UPDATE GroupChat SET ChatID = @ChatID,
                                                 Users = @Users,
                                                 GroupChatName = @GroupChatName
                                             WHERE GroupChatID = @GroupChatID";
            base.dbContext.AddParameters("@GroupChatID", groupChat.ID);
            base.dbContext.AddParameters("@ChatID", groupChat.ChatID);
            base.dbContext.AddParameters("@Users", groupChat.Users);
            base.dbContext.AddParameters("@GroupChatName", groupChat.GroupChatName);
            return base.dbContext.Update(sql) > 0;
        }

        public List<GroupChat> GetAllByUserID(string ID)
        {
            List<GroupChat> groupChats = new List<GroupChat>();
            string sql = $@"SELECT * FROM GroupChat WHERE Users LIKE {ID}";
            base.dbContext.AddParameters("@UserID", ID);
            Console.WriteLine("Query -> " + sql);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    groupChats.Add(this.modelFactory.GroupChatCreator.CreateModel(reader));
                }
                return groupChats;
            }
        }

        public List<GroupChat> GetGroupBySearch(string Request)
        {
            string sql = $@"SELECT * FROM GroupChat WHERE GroupChatName LIKE @Request";
            base.dbContext.AddParameters("@Request", Request);
            List<GroupChat> groupChats = new List<GroupChat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    groupChats.Add(this.modelFactory.GroupChatCreator.CreateModel(reader));
                }
                return groupChats;
            }
        }

        public List<GroupChat> GetArchived(string ID)
        {
            string sql = $@"SELECT GroupChat.ChatID, GroupChat.Users, GroupChat.GroupChatName, GroupChat.GroupChatID FROM GroupChat LEFT JOIN Archive
                                   ON GroupChat.ChatID = Archive.ChatID WHERE GroupChat.Users LIKE %@UserID%";
            base.dbContext.AddParameters("@UserID", ID);
            List<GroupChat> Chats = new List<GroupChat>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Chats.Add(this.modelFactory.GroupChatCreator.CreateModel(reader));
                }
            }
            return Chats;
        }
    }
}

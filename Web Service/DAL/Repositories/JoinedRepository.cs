using Data;
using System.Data;
namespace Web_Service
{
    public class JoinedRepository : Repository, IRepository<Joined>
    {
        public JoinedRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Joined joined)
        {
            string sql = $@"INSERT INTO Joined(UserID, ChatID, JoinedAt, GroupChatID)
                                        VALUES(@UserID,@ChatID,@JoinedAt,@GroupChatID)";
            base.dbContext.AddParameters("@UserID", joined.UserID);
            base.dbContext.AddParameters("@ChatID", joined.ChatID);
            base.dbContext.AddParameters("@JoinedAt", joined.JoinedAt.ToString());
            base.dbContext.AddParameters("@GroupChatID", joined.GroupChatID);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Joined joined)
        {
            string sql = $@"UPDATE Joined SET UserID = @UserID,
                                              ChatID = @ChatID,
                                              JoinedAt = @JoinedAt,
                                              GroupChatID = @GroupChatID
                                            WHERE JoinID = @JoinID";
            base.dbContext.AddParameters("@JoinedAt", joined.JoinedAt.ToString());
            base.dbContext.AddParameters("@UserID", joined.UserID);
            base.dbContext.AddParameters("@ChatID", joined.ChatID);
            base.dbContext.AddParameters("@GroupChatID", joined.GroupChatID);
            base.dbContext.AddParameters("@JoinID", joined.JoinID.ToString());
            return base.dbContext.Update(sql) > 0;
        }

        public bool Delete(Joined joined)
        {
            string sql = $@"DELETE FROM Joined WHERE JoinID = @JoinID";
            base.dbContext.AddParameters("@JoinID", joined.JoinID.ToString());
            return base.dbContext.Delete(sql) > 0;
        }

        public bool DeleteByID(string ID)
        {
            int JoinID = Convert.ToInt32(ID);
            string sql = $@"DELETE FROM Joined WHERE JoinID = @JoinID";
            base.dbContext.AddParameters("@JoinID", JoinID.ToString());
            return base.dbContext.Delete(sql) > 0;
        }

        public List<Joined> GetAll()
        {
            string sql = $@"SELECT * FROM Joined";
            List<Joined> joined = new List<Joined>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    joined.Add(this.modelFactory.JoinedCreator.CreateModel(reader));
                }
                return joined;
            }
        }

        public Joined GetByID(string ID)
        {
            int JoinID = Convert.ToInt32(ID);
            string sql = $@"SELECT * FROM Joined WHERE JoinID = @JoinID";
            base.dbContext.AddParameters("@JoinID", JoinID.ToString());
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.JoinedCreator.CreateModel(reader);
            }
        }
    }
}

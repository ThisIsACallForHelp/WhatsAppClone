using Data;
using System.Data;
namespace Web_Service
{
    public class ArchiveRepository : Repository, IRepository<Archive>
    {
        public ArchiveRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Archive archive)
        {
            string sql = $@"INSERT INTO Archive(ArchiveID, UserID, ChatID)
                                        VALUES(@ArchiveID,@UserID,@ChatID)";
            base.dbContext.AddParameters("@ArchiveID", Guid.NewGuid().ToString());
            base.dbContext.AddParameters("@UserID", archive.UserID);
            base.dbContext.AddParameters("@ChatID", archive.ChatID);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Archive archive)
        {
            string sql = $@"UPDATE Archive SET UserID = @UserID,
                                               ChatID = @ChatID
                                            WHERE ArchiveID = @ArchiveID";
            base.dbContext.AddParameters("@ArchiveID", archive.ID);
            base.dbContext.AddParameters("@UserID", archive.UserID);
            base.dbContext.AddParameters("@ChatID", archive.ChatID);
            return base.dbContext.Update(sql) > 0;
        }

        public bool Delete(Archive archive)
        {
            string sql = $@"DELETE FROM Archive WHERE ArchiveID = @ArchiveID";
            base.dbContext.AddParameters("@ArchiveID", archive.ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Archive WHERE ArchiveID = @ArchiveID";
            base.dbContext.AddParameters("@ArchiveID", ID);
            return base.dbContext.Delete(sql) > 0;
        }

        public List<Archive> GetAll()
        {
            string sql = $@"SELECT * FROM Archive";
            List<Archive> archives = new List<Archive>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    archives.Add(this.modelFactory.ArchiveCreator.CreateModel(reader));
                }
                return archives;
            }
        }

        public Archive GetByID(string ID)
        {
            string sql = $@"SELECT * FROM Archive WHERE ArchiveID = @ArchiveID";
            base.dbContext.AddParameters("@ArchiveID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.ArchiveCreator.CreateModel(reader);
            }
        }
    }
}

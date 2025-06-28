using Data;
using System.Data;
namespace Web_Service
{
    public class ReadMessageRepository : Repository, IRepository<ReadMessage>
    {
        public ReadMessageRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(ReadMessage ReadMessage)
        {
            string sql = $@"INSERT INTO ReadMessage(ReadID ,MessageID ,ReadAt ,UserID)
                                        VALUES(@ReadID,@MessageID,@ReadAt,@UserID)";
            base.dbContext.AddParameters("@ReadID", ReadMessage.ID);
            base.dbContext.AddParameters("@MessageID", ReadMessage.MessageID);
            base.dbContext.AddParameters("@ReadAt", ReadMessage.ReadAt.ToString());
            base.dbContext.AddParameters("@UserID", ReadMessage.UserID);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(ReadMessage ReadMessage)
        {
            string sql = $@"UPDATE ReadMessage SET MessageID = @MessageID, 
                                                   ReadAt = @ReadAt, 
                                                   UserID =@UserID
                                               WHERE ReadID = @ReadID";
            base.dbContext.AddParameters("@ReadID", ReadMessage.ID);
            base.dbContext.AddParameters("@MessageID", ReadMessage.MessageID);
            base.dbContext.AddParameters("@ReadAt", ReadMessage.ReadAt.ToString());
            base.dbContext.AddParameters("@UserID", ReadMessage.UserID);
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(ReadMessage ReadMessage)
        {
            string sql = $@"DELETE FROM ReadMessage WHERE ReadID = @ReadID";
            base.dbContext.AddParameters("@ReadID", ReadMessage.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM ReadMessage WHERE ReadID = @ReadID";
            base.dbContext.AddParameters("@ReadID", ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public ReadMessage GetByID(string ID)
        {
            string sql = $@"SELECT * FROM ReadMessage WHERE ReadID = @ReadID";
            base.dbContext.AddParameters("@ReadID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.ReadMessageCreator.CreateModel(reader);
            }
        }
        public List<ReadMessage> GetAll()
        {
            string sql = $@"SELECT * FROM ReadMessage";
            List<ReadMessage> Messages = new List<ReadMessage>();
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    Messages.Add(this.modelFactory.ReadMessageCreator.CreateModel(reader));
                }
                return Messages;
            }
        }
    }
}

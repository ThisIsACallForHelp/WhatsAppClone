using Data;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;

namespace Web_Service
{
    public class AttachmentRepository : Repository, IRepository<Attachments>
    {
        public AttachmentRepository(DBContext dbContext) : base(dbContext)
        {

        }
        public bool Create(Attachments attachments)
        {
            string sql = $@"INSERT INTO Attachment(AttachmentID, FilePath, MessageID)
                                        VALUES(@AttachmentID, @FilePath, @MessageID)";
            base.dbContext.AddParameters("@AttachmentID", attachments.ID);
            base.dbContext.AddParameters("@FilePath", attachments.FilePath);
            base.dbContext.AddParameters("@MessageID", attachments.MessageID);
            return base.dbContext.Create(sql) > 0;
        }
        public bool Update(Attachments attachments)
        {
            string sql = $@"UPDATE Attachment SET FilePath = @FilePath, 
                                                  MessageID = @MessageID 
                                            WHERE AttachmentID = @AttachmentID";
            base.dbContext.AddParameters("@AttachmentID", attachments.ID);
            base.dbContext.AddParameters("@FilePath", attachments.FilePath);
            base.dbContext.AddParameters("@MessageID", attachments.MessageID);
            return base.dbContext.Update(sql) > 0;
        }
        public bool Delete(Attachments attachment)
        {
            string sql = $@"DELETE FROM Attachment WHERE AttachmentID = @AttachmentID";
            base.dbContext.AddParameters("@AttachmentID", attachment.ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public bool DeleteByID(string ID)
        {
            string sql = $@"DELETE FROM Attachment WHERE AttachmentID = @ID";
            base.dbContext.AddParameters("@AttachmentID", ID);
            return base.dbContext.Delete(sql) > 0;
        }
        public Attachments GetByID(string ID)
        {
            string sql = $@"SELECT * FROM Attachment WHERE AttachmentID = @ID";
            base.dbContext.AddParameters("@AttachmentID", ID);
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                reader.Read();
                return this.modelFactory.AttachmentsCreator.CreateModel(reader);
            }
        }

        public List<Attachments> GetAll()
        {
            List<Attachments> attachments = new List<Attachments>();
            string sql = $@"SELECT * FROM Attachment";
            using (IDataReader reader = base.dbContext.Read(sql))
            {
                while (reader.Read())
                {
                    attachments.Add(this.modelFactory.AttachmentsCreator.CreateModel(reader));
                }
                return attachments;
            }
        }


    }
}

using Data;
using System.Data;

namespace Web_Service
{
    public class AttachmentsCreator : IModelCreator<Attachments>
    {
        public Attachments CreateModel(IDataReader src)
        {
            return new Attachments
            {
                FilePath = "https://localhost:7189/Attachments/" + Convert.ToString(src["FilePath"]),
                ID = Convert.ToString(src["AttachmentID"]),
                MessageID = Convert.ToString(src["MessageID"])
            };
        }
    }
}

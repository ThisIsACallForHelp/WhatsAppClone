using Data;
using System.Data;

namespace Web_Service
{
    public class ArchiveCreator : IModelCreator<Archive>
    {
        public Archive CreateModel(IDataReader src)
        {
            return new Archive
            {
                ID = Convert.ToString(src["ArchiveID"]),
                ChatID = Convert.ToString(src["ChatID"]),
                UserID = Convert.ToString(src["UserID"])
            };
        }
    }
}

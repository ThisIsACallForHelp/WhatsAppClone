using Data;
using System.Data;
namespace Web_Service
{
    public class ReadMessageCreator : IModelCreator<ReadMessage>
    {
        public ReadMessage CreateModel(IDataReader src)
        {
            return new ReadMessage
            {
                MessageID = Convert.ToString(src["MessageID"]),
                ReadAt = Convert.ToDateTime(src["ReadAt"]),
                ID = Convert.ToString(src["ReadID"]),
                UserID = Convert.ToString(src["UserID"])
            };
        }
    }
}

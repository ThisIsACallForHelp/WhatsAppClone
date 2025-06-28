using Data;
using System.Data;
namespace Web_Service
{
    public class JoinedCreator : IModelCreator<Joined>
    {
        public Joined CreateModel(IDataReader src)
        {
            return new Joined
            {
                ChatID = Convert.ToString(src["ChatID"]),
                JoinID = Convert.ToInt32(src["JoinID"]),
                UserID = Convert.ToString(src["UserID"]),
                GroupChatID = Convert.ToString(src["GroupChatID"]),
                JoinedAt = Convert.ToDateTime(src["JoinedAt"])
            };
        }
    }
}

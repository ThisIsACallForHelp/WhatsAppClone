using Data;
using System.Data;

namespace Web_Service
{
    public class ChatCreator : IModelCreator<Chat>
    {
        public Chat CreateModel(IDataReader src)
        {
            return new Chat
            {
                ChatName = Convert.ToString(src["ChatName"]),
                CreationDate = Convert.ToDateTime(src["CreationDate"]),
                Creator = Convert.ToString(src["Creator"]),
                ID = Convert.ToString(src["ChatID"]),
                ChatDescription = Convert.ToString(src["ChatDescription"]),
                ChatIMG = "https://localhost:7189/Chats/" + Convert.ToString(src["ChatIMG"]),
                FirstUserID = Convert.ToString(src["FirstUserID"]),
                SecondUserID = Convert.ToString(src["SecondUserID"])
            };
        }
    }
}

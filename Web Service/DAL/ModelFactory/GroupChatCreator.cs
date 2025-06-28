using Data;
using System.Data;
using System.Reflection.Metadata.Ecma335;
namespace Web_Service
{
    public class GroupChatCreator : IModelCreator<GroupChat>
    {
        public GroupChat CreateModel(IDataReader src)
        {
            return new GroupChat
            {
                ChatID = Convert.ToString(src["ChatID"]),
                ID = Convert.ToString(src["GroupChatID"]),
                GroupChatName = Convert.ToString(src["GroupChatName"]),
                Users = Convert.ToString(src["Users"])
            };
        }
    }
}

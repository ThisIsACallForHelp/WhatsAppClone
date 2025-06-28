using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MainPage
    {
        public List<Message>? Convo {  get; set; }
        public List<Message>? Messages {  get; set; }
        public User? User { get; set; }
        public List<GroupChat>? GroupChats { get; set; }
        public List<Chat>? Chats { get; set; }
    }
}

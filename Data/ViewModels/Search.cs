using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Search
    {
        public List<Chat> Chats { get; set; }   
        public List<GroupChat> GroupChats { get; set; }
        public List<Message> LatestMessages { get; set; }
    }
}

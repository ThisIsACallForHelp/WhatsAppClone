using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Joined 
    {
        public int? JoinID { get; set; }
        public string? UserID { get; set; }
        public string? ChatID { get; set; }
        public DateTime? JoinedAt { get; set; }
        public string? GroupChatID { get; set; }
    }
}

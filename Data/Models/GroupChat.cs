using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class GroupChat : Model
    {
        public string? ChatID { get; set; }
        public string? Users {  get; set; }
        public string? GroupChatName { get; set; }

    }
}

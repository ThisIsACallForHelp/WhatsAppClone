
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Message : Model
    {
        public string? SenderID { get; set; }
        public DateTime? SentAt { get; set; }
        public string? Content { get; set; }
        public string? Attachments { get; set; }
    }
    //use attachments as a long string and then get it from the db using the ID
    //each attachment will have a fixed length of characters.  
    //divide the length of the string by the fixed length of each id and you will get the
    //ID's and the amount of the attachments
}

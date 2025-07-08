
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
        public string CipherTextBase64 { get; set; } 
        public string IVBase64 { get; set; }              
        public string HmacBase64 { get; set; }            
        public string SignatureBase64 { get; set; }       
        public string SenderPublicKeyBase64 { get; set; } 
        public string SenderSigningKeyBase64 { get; set; }
        
        public string? ChatID {  get; set; }
        public string? Attachments { get; set; }
        public string? SenderID { get; set; }
        public DateTime? SentAt { get; set; }
    }
    //use attachments as a long string and then get it from the db using the ID
    //each attachment will have a fixed length of characters.  
    //divide the length of the string by the fixed length of each id and you will get the
    //ID's and the amount of the attachments
}

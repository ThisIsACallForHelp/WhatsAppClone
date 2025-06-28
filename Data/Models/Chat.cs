using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Chat : Model
    {
        public string? ChatName { get; set; }
        public string? Creator {  get; set; }
        public DateTime? CreationDate { get; set; }
        public string? ChatIMG { get; set; }
        public string? ChatDescription { get; set; }
        public string? FirstUserID { get; set; }    
        public string? SecondUserID { get; set; }
    }
}

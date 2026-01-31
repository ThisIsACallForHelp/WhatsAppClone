using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Token
    {
        public  string? TokenStr { get; set; }
        public  DateTime? CreatedAt { get; set; }
        public  DateTime? ExpiresAt { get; set; }
        public  string? AuthUserID {get; set; }
        public  bool? HasBeenUsed { get; set; }
        public  string? RedirectURL { get; set; }
        public string? BrowserConnectID { get; set; }
    }
}

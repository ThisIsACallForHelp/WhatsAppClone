using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ReadMessage : Model
    {
        public string? MessageID { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? UserID { get; set; }
    }
}

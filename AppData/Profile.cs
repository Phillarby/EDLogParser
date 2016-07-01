using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    class Profile
    {
        public int id { get; set; }
        public string Commander { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
    }
}

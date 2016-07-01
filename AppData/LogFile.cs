using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    class LogFile
    {
        public int id { get; set; }
        public string LocalFile { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastParsed { get; set; }
        public int LinesParsed { get; set; }
    }
}

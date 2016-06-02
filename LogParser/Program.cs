using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Locator L = new Locator();
            Parser P = new Parser();

            //Enable netlogging if not already enabled
            Console.Write("Checking netlog is enabled: ");
            if (!L.IsLoggingEnabled())
            {
                Console.WriteLine("Not enabled.  Turning on... ");
                L.EnableVerboseNetlog();
            }
            else
            {
                Console.WriteLine("OK.");
            }

            //Get netlog files
            var logs = L.getNetlogs();

            //Parse Logfiles
            foreach (System.IO.FileInfo fi in logs)
            {
                var s = P.ParseFile(fi);
                if (s != string.Empty) Console.Write(s);
            }

            Console.ReadLine();

        }
    }
}

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

            Console.ReadLine();

        }
    }
}

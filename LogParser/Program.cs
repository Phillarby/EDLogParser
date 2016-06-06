using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LogParser
{
    class Program
    {
        public static XmlDocument x;

        static void Main(string[] args)
        {
            Locator L = new Locator();
            Parser P = new Parser();

            P.ScreenshotPath = L.ScreenshotFolder;

            //Create XML Output 
            x = new XmlDocument();
            XmlDeclaration xmlDeclaration = x.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = x.CreateElement("EDData");
            x.AppendChild(root);
            x.InsertBefore(xmlDeclaration, root);

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
                XmlElement f = P.ParseFile(fi, x);
                root.AppendChild(f);
            }

            //Write to console
            XmlTextWriter writer = new XmlTextWriter(Console.Out);
            writer.Formatting = Formatting.Indented;
            x.WriteTo(writer);
            writer.Flush();
            Console.WriteLine();

            Console.ReadLine();

        }
    }
}

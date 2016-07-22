using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace LogParser
{
    public delegate void NewLineProcessedEventHandler(object sender, NewLineEventArgs e);
    public delegate void StopPublishEventHandler(object sender, EventArgs e);

    class Program
    {
        private XmlDocument xDocNetlog;
        private XmlDocument xDocCliLog;

        private Locator L;
        private Parser P;
        private DataSet FileData;

        public static event NewLineProcessedEventHandler NewLineProcessed;
        public static event StopPublishEventHandler StopPublish;
        
        static void Main(string[] args)
        {
            new Program().go();
        }

        private void CheckNetlog()
        {
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
        }

        /// <summary>
        /// Get all data front eh launcher log files
        /// </summary>
        /// <returns></returns>
        public XmlDocument getLauncherLogData(XmlDocument x)
        {
            //Get XML root
            var root = x.DocumentElement;

            //Get xml elements to append
            var log = L.getLauncherLog();
            var c = P.ParseLauncherLog(log, x);

            root.AppendChild(c);

            return x;

        }

        /// <summary>
        /// Get all data from the current netlog files
        /// </summary>
        /// <param name="x">The document to be populated</param>
        /// <param name="LastState">The last known state as unchanged files can be excluded</param>
        /// <returns></returns>
        public XmlDocument getNetLogData(XmlDocument x, DataSet LastState)
        {
            //Get netlog files
            FileInfo[] logs; 

            //Get the document root
            var root = x.DocumentElement;

            //Get a list of all previously encoutered and processed files so they can be ignored unless subsequently changed
            Dictionary<string, DateTime> ParsedFiles = new Dictionary<string, DateTime>();

            //Get current netlog files
            logs = L.getNetlogs();

            //Ignore if no last state data exists
            if (LastState.Tables.Count != 0)
            {
                var PastFiles = LastState.Tables["File"].Select();

                foreach (DataRow d in PastFiles)
                {
                    ParsedFiles.Add(d["Filename"].ToString(), DateTime.ParseExact(d["Modified"].ToString(), "yyyyMMddHHmmss", null));
                }

                //Add any netlog files that have been updated since the last state was established
                logs = L.getNetlogs().Where(f => ParsedFiles.Keys.Contains(f.Name) && ParsedFiles[f.Name] != f.LastWriteTimeUtc.TrimMilliseconds()).ToArray<FileInfo>();
            }


            //Parse each log file
            foreach (System.IO.FileInfo fi in logs)
            {
                XmlElement f = P.ParseNetlogFile(fi, x);
                root.AppendChild(f);
            }

            return x;
        }

        //Sets all columns of the specified name to be the primary key of all tables in the dataset
        private void setPrimaryKey(DataSet d, string ColumnName)
        {
            foreach (DataTable dt in d.Tables)
            {
                if (dt.Columns.Contains(ColumnName))
                {
                    dt.PrimaryKey = new DataColumn[] { dt.Columns[ColumnName] };
                }
            }
        }

        private XmlDocument InstantiateXML()
        {
            //Create XML file with declaration and root
            var x = new XmlDocument();
            XmlDeclaration xmlDeclaration = x.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement xRoot = x.CreateElement("EDLogs");
            x.AppendChild(xRoot);
            x.InsertBefore(xmlDeclaration, xRoot);

            return x;
        }

        public void go()
        {
            //Instantiate file locater and file parser objects
            L = new Locator();  //File locator
            P = new Parser();   //Netlog parser

            //Get output paths
            var Appfolder = System.IO.Directory.CreateDirectory(Path.Combine(L.AppDataFolderPath, "EDNetlogParser")); //Output foler
            string XmlPath = Path.Combine(Appfolder.FullName, "NetlogData.xml"); //XML data 
            var XsdPath = Path.Combine(Appfolder.FullName, "NetlogData.xsd");    //XML Schema

            //Check netlogging in enabled and enable if not
            CheckNetlog();

            //Get the local save path for screenshots and give to the parser
            P.ScreenshotPath = L.ScreenshotFolder;

            //Load the last saved state into XML file and ADO.Net dataset
            //This includes all previously encountered netlog lines that have been recorded locally
            var xLastState = new XmlDocument();
            var dLastState = new DataSet();
            if (File.Exists(XmlPath)) { xLastState.Load(XmlPath); } //If no previous file exists then assume this is the first execution so don't try to load previous state
            var rLastState = new XmlNodeReader(xLastState); //get XML reader
            dLastState.ReadXml(rLastState);  //Populate ADO.net dataset from XML info
            setPrimaryKey(dLastState, "Id"); //Set Id as primary key across all dataset tables.  This allows dataset merging in later steps

            //Create XML documents for parsing current log files
            xDocNetlog = InstantiateXML(); //For netlog files
            xDocCliLog = InstantiateXML(); //For client log files

            //Load data from logfiles into XML
            xDocNetlog = getNetLogData(xDocNetlog, dLastState);
            //xDocNetlog = getLauncherLogData(xDocCliLog);

            //Create Dataset from XML log data
            FileData = new DataSet();
            var xRdr = new XmlNodeReader(xDocNetlog);
            FileData.ReadXml(xRdr);
            setPrimaryKey(FileData, "Id");

            var x = FileData.Tables["System"].Select();
            var y = dLastState.Tables["System"].Select();

            //Merge log file dataset into last state datasets.  This automatically dedupes.
            dLastState.Merge(FileData);
            dLastState.AcceptChanges();

            //Write netlog content to console
            XmlTextWriter writer = new XmlTextWriter(Console.Out);
            writer.Formatting = Formatting.Indented;

            dLastState.WriteXml(writer);
            writer.Flush();
            Console.WriteLine();

            //Delete last state files
            File.Delete(XmlPath);
            File.Delete(XsdPath);

            //Save dataset to filesystem - XML
            dLastState.WriteXml(XmlPath);
            dLastState.WriteXmlSchema(XsdPath);

            //Wait to close console
            Console.ReadLine();
        }

        //Publisheh live data from netlog files via chunked http output for consumption by whatever wants it
        //public void CreateHttpPublisher()
        //{
        //    //Create a publisher in a new thread
        //    string[] endpoints = new string[] { "http://localhost/EDData/" };
        //    Thread pubThread = new Thread(() => new Publisher(endpoints, 80));
        //    pubThread.Start();

        //    int i = 0;

        //    while (i < 10)
        //    {
        //        Console.ReadLine();

        //        i++;
        //        Console.WriteLine(i);

        //        var e = new NewLineEventArgs();
        //        e.Data = i.ToString();

        //        if (NewLineProcessed != null)
        //            NewLineProcessed(this, e);
        //    }

        //    //Close http stream
        //    if (StopPublish != null)
        //        StopPublish(this, new EventArgs());
        //}
    }

    

    public class NewLineEventArgs : EventArgs
    {
        public string Data { get; set; }
    }

}

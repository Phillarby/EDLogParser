using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LogParser
{
    public class Locator
    {
        /// <summary>
        /// Path of current active game installation folder
        /// </summary>
        private string AppFolder
        {
            get; set;
        }

        /// <summary>
        /// Path of the folder containing the launcher logs
        /// </summary>
        private string ClientLogFolder
        {
            get; set;
        }

        /// <summary>
        /// Screenshot save folder
        /// </summary>
        public string ScreenshotFolder
        {
            get { return getScreenshotFolder(); }
        }
        
        /// <summary>
        

        /// <summary>
        /// Accessor for environment variable
        /// </summary>
        public string AppDataFolderPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); }
        }

        /// <summary>
        /// Standard constructor intiaition searhc for applicaiton folders
        /// </summary>
        public Locator()
        {
            AppFolder = FindProductFolder();
            ClientLogFolder = FindClientLogFolder();
        }

        /// <summary>
        /// Force a specific game app folder path during construction
        /// </summary>
        /// <param name="AppFolder">Injected application folder path</param>
        public Locator(string AppFolder)
        {
            this.AppFolder = AppFolder;
            ClientLogFolder = FindClientLogFolder();
        }

        private string NetLogFolder
        {
            get { return AppFolder + "\\Logs"; }
        }

        private string AppConfig()
        {
            return AppFolder + "\\AppConfig.xml";
        }

        public FileInfo getLauncherLog()
        {
            var dir = new DirectoryInfo(ClientLogFolder);
            var f = dir.GetFiles().Where(x => x.Name == "Client.log").FirstOrDefault();
            return f;
        }

        public FileInfo[] getNetlogs()
        {
            var dir = new DirectoryInfo(NetLogFolder);
            FileInfo[] files = dir.GetFiles();

            //Ensure only netlog files are being processed
            return files.Where(x => x.Name.Substring(0, 6) == "netLog").ToArray();
        }

        public bool IsLoggingEnabled()
        {
            bool ret = true;

            XDocument doc = XDocument.Load(AppConfig());

            //Check for verbose logging
            var network = doc.Root.Elements().Where(x => x.Name == "Network").FirstOrDefault();
            var verbose = network.Attributes("VerboseLogging").FirstOrDefault();

            //No verbose logging enabled
            if (verbose == null || verbose.Value != "1") ret = false;

            return ret;

        }

        public void EnableVerboseNetlog()
        {
            XDocument doc = XDocument.Load(AppConfig());

            //Check for verbose logging
            var network = doc.Root.Elements().Where(x => x.Name == "Network").FirstOrDefault();
            var verbose = network.Attributes("VerboseLogging").FirstOrDefault();

            //No verbose logging enabled
            if (verbose == null)
            {
                network.Add(new XAttribute("VerboseLogging", "1"));
                verbose = network.Attributes("VerboseLogging").FirstOrDefault();
            }

            //Verbose logging disabled
            if (verbose.Value != "1") verbose.Value = "1";

            doc.Save(AppConfig());

        }
        private string getScreenshotFolder()
        {
            var userPath = Environment.GetEnvironmentVariable("USERPROFILE");
            return userPath + @"\Pictures\Frontier Developments\Elite Dangerous\";
        }

        public string FindClientLogFolder()
        {
            return Directory.GetParent(AppFolder).Parent.FullName + "\\Logs";
        }

        /// <summary>
        /// Find the best match for the game folder
        /// </summary>
        /// <returns></returns>
        public string FindProductFolder()
        {
            var steamPath = "C:\\program files (x86)\\Steam\\steamapps\\common\\Elite Dangerous\\Products\\";
            var frontierPath = AppDataFolderPath + "\\Frontier_Developments\\Products\\";
            var progFilesPath = "C:\\Program Files (x86)\\Frontier\\Products\\";
            

            //Check which game install paths exist
            bool steam = System.IO.Directory.Exists(steamPath);
            bool frontier = System.IO.Directory.Exists(frontierPath);
            bool progFiles = System.IO.Directory.Exists(progFilesPath);

            //Get details for all non-beta game folders.
            //Current Folder Names: elite-dangerous-64 or FORC-FDEV-D-1010
            List<string> gameFolders = new List<string>();
            if (steam)
            {
                gameFolders.AddRange(
                    System.IO.Directory.GetDirectories(steamPath)
                                       .Where(x => x == steamPath + "FORC-FDEV-D-1010" || x == steamPath + "elite-dangerous-64")
                                       .ToList()
                );
            }

            if (frontier)
            {
                gameFolders.AddRange(
                    System.IO.Directory.GetDirectories(frontierPath)
                                       .Where(x => x == frontierPath + "FORC-FDEV-D-1010" || x == frontierPath + "elite-dangerous-64")
                                       .ToList()
                );
            }

            if (progFiles)
            {
                gameFolders.AddRange(
                    System.IO.Directory.GetDirectories(progFilesPath)
                                       .Where(x => x == progFilesPath + "FORC-FDEV-D-1010" || x == progFilesPath + "elite-dangerous-64")
                                       .ToList()
                );
            }

            //Find the most liekly candidate for the currently used installation
            if (gameFolders.Count == 0)
                return string.Empty;
            else
                return PickBestFolder(gameFolders);

        }

        /// <summary>
        /// Determine the best installation by finding the most recently created log file.  Assume this folder is the current install
        /// that is being used
        /// </summary>
        /// <param name="Folders"></param>
        /// <returns></returns>
        private string PickBestFolder(List<string> Folders)
        {
            //Find the most recently used of all the game folders
            String MostRecentFolder = Folders.FirstOrDefault();
            DateTime MostRecentDate = DateTime.MinValue;

            foreach (String f in Folders)
            {
                var d = new System.IO.DirectoryInfo(f + "\\Logs\\");
                var files = d.GetFiles();

                foreach(System.IO.FileInfo fi in files)
                {
                    var last = fi.LastWriteTime;
                    if  (last > MostRecentDate)
                    {
                        MostRecentDate = last;
                        MostRecentFolder = f;
                    }
                }
            }

            return MostRecentFolder;
        }
    }
}

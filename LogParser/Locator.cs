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
        public string AppFolderPath;
        
        public Locator()
        {
            AppFolderPath = FindProductFolder();
        }

        public string NetLogFolder
        {
            get { return AppFolderPath + "\\Logs"; }
        }

        public string AppConfig()
        {
            return AppFolderPath + "\\AppConfig.xml";
        }

        public FileInfo[] getNetlogs()
        {
            var files = Directory.GetFiles(NetLogFolder).Where(x => x.Contains("netLog.")).ToArray();
            FileInfo[] fi = new FileInfo[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                fi[i] = new FileInfo(files[i]);
            }

            return fi;
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

        /// <summary>
        /// Find the best match for the game folder
        /// </summary>
        /// <returns></returns>
        public string FindProductFolder()
        {
            
            var userPath = Environment.GetEnvironmentVariable("USERPROFILE");

            var steamPath = @"C:\program files (x86)\Steam\steamapps\common\Elite Dangerous\Products\";
            var frontierPath = userPath + @"\AppData\Local\Frontier_Developments\Products\";
            var progFilesPath = @"C:\Program Files (x86)\Frontier\Products\";

            //Check if steam path exists
            bool steam = System.IO.Directory.Exists(steamPath);
            bool frontier = System.IO.Directory.Exists(frontierPath);
            bool progFiles = System.IO.Directory.Exists(progFilesPath);

            //Identify all non-beta game folders.
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

            if (gameFolders.Count == 0)
                return string.Empty;
            else
                return PickBestFolder(gameFolders);

        }

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

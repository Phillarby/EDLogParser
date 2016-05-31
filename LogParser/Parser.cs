using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Drawing;

namespace LogParser
{
    public class Parser
    {
        public enum LineType
        {
            Header,
            SystemHit,
            Screenshot,
            NoInterest
        }

        DateTime _fileDate;
        DateTime _gMTDate;
        string _filename;
        int _linesParsed;
        string _currentSystem;
        string _logFolder;

        public Parser() : this(string.Empty)
        {
 
        }

        public Parser(string LogFolder)
        {
            FileInfo[] files = getFiles(LogFolder);

            foreach (FileInfo f in files)
                ParseFile(f);

        }

        public FileInfo[] getFiles(string LogFolder)
        {
            var dir = new DirectoryInfo(LogFolder);
            FileInfo[] files = dir.GetFiles();

            //Ensure only netlog files are being processed
            return files.Where(x => x.Name.Substring(0,6) == "netLog").ToArray();
        }

        public void ParseFile(FileInfo f)
        {
            int line = 0; // Line counter

            //Open file for reading
            var file = f.OpenRead();
            StreamReader reader = new StreamReader(file);

            //Read the file line by line
            while (!reader.EndOfStream)
            {
                line++;
                ProcessLine(reader.ReadLine(), line);
            }

            //Clean up and close
            reader.Close();
            file.Close();
        }

        private void ProcessLine(string line, int LineNumber)
        {
            LineType lineType;

            //identify line type
            if (LineNumber == 1)
                lineType = LineType.Header;
            else if (line.Contains("System:") && line.Contains("StarPos:"))
                lineType = LineType.SystemHit;
            else if (line.Contains("SCREENSHOT: Saved"))
                lineType = LineType.Screenshot;
            else
                lineType = LineType.NoInterest;

            //Process line
            if (lineType != LineType.NoInterest)
                ExtractData(line, lineType);

            _linesParsed = LineNumber;

        }

        private void ExtractData(string Line, LineType Type)
        {
            if (Type == LineType.Header)
            {
                _fileDate = ReadHeaderLocalDateTime(Line);
            }

            if (Type == LineType.SystemHit)
            {
                var SystemName = ReadSystem(Line);
                var Pos = ReadStarPos(Line);
                var Time = ReadLineTime(Line);
            }

            if (Type == LineType.Screenshot)
            {
                var Screenshot = ReadScreenshotFilename(Line);
            }
        }

        private DateTime ReadHeaderLocalDateTime(string Line)
        {
            string Pattern = @"([0-9]*\-){2}[0-9]{2}-[0-9]{2}:[0-9]{2}";

            //Run the regex and get matches
            Regex r = new Regex(Pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(Line);

            //Check the correct number of regex matches are found
            if (m.Captures.Count != 1)
            {
                throw new ApplicationException(
                    String.Format("Incorect number of regex matches for local date time.  Found {0}, expecting 1", m.Captures.Count)
                );
            }

            //Parse into DateTime object
            var vals = m.Value.Replace(':', '-').Split('-');
            var intVals = vals.Select(x => int.Parse(x)).ToArray();
            intVals[0] += 2000; //Update years to include second millenium
            DateTime ret = new DateTime(intVals[0], intVals[1], intVals[2], intVals[3], intVals[4], 0);

            return ret;
        }

        private string ReadSystem(string Line)
        {
            string SystemPattern = "\\\"(.*?)\\\"";

            //Run the regex and get matches
            Regex r = new Regex(SystemPattern, RegexOptions.IgnoreCase);
            Match m = r.Match(Line);

            //Check the correct number of regex matches are found
            if (m.Captures.Count != 1)
            {
                throw new ApplicationException(
                    String.Format("Incorect number of regex matches for system name.  Found {0}, expecting 1", m.Captures.Count)
                );
            }

            return m.Value.Replace("\"", "");
        }

        private DateTime ReadLineTime(string Line)
        {
            string SystemPattern = "([0-9]{2}:)([0-9]{2}):[0-9]{2}";

            //Run the regex and get matches
            Regex r = new Regex(SystemPattern, RegexOptions.IgnoreCase);
            Match m = r.Match(Line);

            //Check the correct number of regex matches are found
            if (m.Captures.Count != 1)
            {
                throw new ApplicationException(
                    String.Format("Incorect number of regex matches for system name.  Found {0}, expecting 1", m.Captures.Count)
                );
            }

            return DateTime.Parse(m.Value);
        }

        private Vector3D ReadStarPos(string Line)
        {
            string SystemPattern = @"StarPos:\(.[0-9\,\.\\-]*\)";

            //Run the regex and get matches
            Regex r = new Regex(SystemPattern, RegexOptions.IgnoreCase);
            Match m = r.Match(Line);

            //Check the correct number of regex matches are found
            if (m.Captures.Count != 1)
            {
                throw new ApplicationException(
                    String.Format("Incorect number of regex matches for system name.  Found {0}, expecting 1", m.Captures.Count)
                );
            }

            //Parse out unnescessary guff to just keep numeric data
            var extract = m.Value;
            var right = extract.Substring(9);
            var left = right.Substring(0, right.Length - 1);
            string[] coords = left.Split(',');

            //Check we have 3 coordinates
            if (coords.Length != 3)
            {
                throw new ApplicationException(
                    String.Format("Incorect number system coordinates identified.  Found {0}, expecting 3", m.Captures.Count)
                );
            }

            //Create a 3D vector for the star location
            Vector3D vec = new Vector3D(Double.Parse(coords[0]), Double.Parse(coords[1]), Double.Parse(coords[2]));

            return vec;       
        }

        private string ReadScreenshotFilename(string Line)
        {
            //Regex to identify file path on local machine
            string Pattern = "\\\\(.*)(.bmp)";

            //Run the regex and get matches
            Regex r = new Regex(Pattern, RegexOptions.IgnoreCase);
            Match m = r.Match(Line);

            //Check the correct number of regex matches are found
            if (m.Captures.Count != 1)
            {
                throw new ApplicationException(
                    String.Format("Incorect number of regex matches for screenshot line.  Found {0}, expecting 1", m.Captures.Count)
                );
            }

            return m.Value;
        }
    }
}

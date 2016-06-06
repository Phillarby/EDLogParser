using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Xml;

namespace LogParser
{
    public class Parser
    {
        public enum LineType
        {
            Header,
            System,
            Screenshot,
            NoInterest
        }

        public string ScreenshotPath
        {
            private get; set;
        }

        DateTime _fileDate;
        int _linesParsed;
        string _currentSystem;
        XmlDocument _doc;
        

        public XmlElement ParseFile(FileInfo f, XmlDocument x)
        {
            return ParseFile(f, x, 0);
        }

        public XmlElement ParseFile(FileInfo f, XmlDocument x, int readLines)
        {
            int line = 0; // Line counter
            _doc = x;

            //Create XML file node
            XmlElement ret = _doc.CreateElement(string.Empty, "File", string.Empty);
            XmlAttribute filename = _doc.CreateAttribute("Filename");

            filename.Value = f.Name;
            ret.Attributes.Append(filename);

            //Open file for reading
            var file = f.OpenRead();
            StreamReader reader = new StreamReader(file);

            //Navigate to start line
            for (int i = 0; i < readLines; i++)
            {
                reader.ReadLine();
                line = i;
            }

            //Process the file line by line until done
            while (!reader.EndOfStream)
            {
                //Add a new XML element if the line contains useful data
                var lineContent = ProcessLine(reader.ReadLine(), line);
                if (lineContent != null)
                    ret.AppendChild(lineContent);


                //Increment line counter
                line++;
            }

            //Clean up and close
            reader.Close();
            file.Close();


            //All done
            return ret;
        }

        private XmlElement ProcessLine(string line, int LineNumber)
        {
            XmlElement ret;
            LineType lineType;

            //identify line type
            if (LineNumber == 0)
            {
                //Header is always the first line in the log file
                lineType = LineType.Header;
                ret = _doc.CreateElement("Header");
            }
            else if (line.Contains("System:") && line.Contains("StarPos:"))
            {
                lineType = LineType.System;
                ret = _doc.CreateElement("System");
            }
            else if (line.Contains("SCREENSHOT: Saved"))
            {
                lineType = LineType.Screenshot;
                ret = _doc.CreateElement("Screenshot");
            }
            else
            {
                lineType = LineType.NoInterest;
                ret = null;
            }

            //Process line
            if (lineType != LineType.NoInterest)
            {
                ret = ExtractData(line, lineType);
            }

            _linesParsed = LineNumber;

            return ret;
        }

        private XmlElement ExtractData(string Line, LineType Type)
        {
            XmlElement ret = null;

            if (Type == LineType.Header)
            {
                _fileDate = ReadHeaderLocalDateTime(Line);
                ret = _doc.CreateElement("Header");

                //Add element attributes
                XmlAttribute ts = _doc.CreateAttribute("TS");
                ts.Value = _fileDate.ToString("yyyy-MM-ddTHH:mm:ss");

                ret.Attributes.Append(ts);
            }

            if (Type == LineType.System)
            {
                var SystemName = ReadSystem(Line);

                //Only capture if this is a move to a different system
                if (SystemName != _currentSystem)
                {
                    _currentSystem = SystemName;
                    var Pos = ReadStarPos(Line);
                    var Time = ReadLineTime(Line);

                    ret = _doc.CreateElement("System");
                    ret.InnerText = SystemName;

                    //Add element attributes
                    XmlAttribute ts = _doc.CreateAttribute("TS");
                    XmlAttribute x = _doc.CreateAttribute("X");
                    XmlAttribute y = _doc.CreateAttribute("Y");
                    XmlAttribute z = _doc.CreateAttribute("Z");

                    ts.Value = Time.ToString("yyyy-MM-ddTHH:mm:ss");
                    x.Value = Pos.X.ToString();
                    y.Value = Pos.Y.ToString();
                    z.Value = Pos.Z.ToString();

                    ret.Attributes.Append(ts);
                    ret.Attributes.Append(x);
                    ret.Attributes.Append(y);
                    ret.Attributes.Append(z);
                }
            }

            if (Type == LineType.Screenshot)
            {
                var Screenshot = ReadScreenshotFilename(Line);
                var Time = ReadLineTime(Line);

                ret = _doc.CreateElement("screenshot");
                ret.InnerText = Screenshot.Replace("\\ED_Pictures\\", ScreenshotPath);

                //Add element attributes
                XmlAttribute ts = _doc.CreateAttribute("TS");
                ts.Value = Time.ToString("yyyy-MM-ddTHH:mm:ss");
                ret.Attributes.Append(ts);
            }

            return ret;
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

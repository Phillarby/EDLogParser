using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Collections.Generic;

namespace LogParser
{
    public class Parser
    {
        private string dateFormat = "yyyyMMddHHmmss";

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
        
        public XmlElement ParseLauncherLog(FileInfo f, XmlDocument x)
        {
            List<string> emails = new List<string>();

            //Open file for reading
            var file = f.OpenRead();
            StreamReader reader = new StreamReader(file);

            //Create XML file node
            XmlElement ret = _doc.CreateElement(string.Empty, "Commander", string.Empty);

            //Read lines
            List<string> cmdrs = new List<string>();

            //Regex for identifying email addresses from log file
            string Pattern =
                @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";

            //Run the regex and get matches
            Regex r = new Regex(Pattern, RegexOptions.IgnoreCase);
            

            //Process the file line by line until done
            while (!reader.EndOfStream)
            {
                //Add a new XML element if the line contains useful data
                var l = reader.ReadLine();

                //If the line is a successful authentication line then process
                if (l.Contains("\"action\" : \"Authenticated\", \"user\" :"))
                {
                    //Extract any email addresses 
                    Match m = r.Match(l);
                    if (m.Captures.Count == 1)
                    {
                        //capture email if it has not been seen before
                        var e = m.Captures[0].Value;
                        if (!emails.Contains(e))
                            emails.Add(e);
                    }
                }
            }

            foreach(string s in emails)
            {
                XmlElement cmdr = x.CreateElement("Account");
                XmlAttribute a = x.CreateAttribute("Username", s);
                cmdr.Attributes.Append(a);
                ret.AppendChild(cmdr);
            }


            return ret;
        }

        public XmlElement ParseNetlogFile(FileInfo f, XmlDocument x)
        {
            return ParseNetlogFile(f, x, 0);
        }

        public XmlElement ParseNetlogFile(FileInfo f, XmlDocument x, int readLines)
        {
            int line = 0; // Line counter
            _doc = x;

            //Create XML file node
            XmlElement ret = _doc.CreateElement(string.Empty, "File", string.Empty);

            //Create attribute for filename, number of lines and last modified date
            XmlAttribute filename = _doc.CreateAttribute("Filename");
            XmlAttribute lines = _doc.CreateAttribute("Lines");
            XmlAttribute modified = _doc.CreateAttribute("Modified");

            //Populate filename attribute
            filename.Value = f.Name;
            ret.Attributes.Append(filename);

            //Populate modified date attribute
            modified.Value = f.LastWriteTimeUtc.ToString(dateFormat);
            ret.Attributes.Append(modified);

            //Open file for reading
            var file = f.OpenRead();
            StreamReader reader = new StreamReader(file);

            //Process lines from the start of the file to the specified end point
            for (int i = 0; i < readLines; i++)
            {
                reader.ReadLine();
                line = i;
            }

            //Process the file line by line until done
            while (!reader.EndOfStream)
            {
                //Add a new XML element if the line contains useful data
                var lineContent = ProcessNetlogLine(reader.ReadLine(), line, f.Name);
                if (lineContent != null)
                    ret.AppendChild(lineContent);


                //Increment line counter
                line++;
            }

            //Add line count
            lines.Value = line.ToString();
            ret.Attributes.Append(lines);

            //Clean up and close
            reader.Close();
            file.Close();


            //All done
            return ret;
        }

        private XmlElement ProcessNetlogLine(string line, int LineNumber, string Filename)
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

            if (ret != null)
            {
                //Apply a unique identifer to use a primary key for the data row
                XmlAttribute Id = _doc.CreateAttribute("Id");
                Id.Value = Filename.Substring(7, 12) + Filename.Substring(20, 2) + LineNumber.ToString();
                ret.Attributes.Append(Id);
            }

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
                ts.Value = _fileDate.ToString(dateFormat);

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

                    ts.Value = Time.ToString(dateFormat);
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
                ts.Value = Time.ToString(dateFormat);
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

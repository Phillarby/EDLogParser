using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EDDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EDData" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EDData.svc or EDData.svc.cs at the Solution Explorer and start debugging.
    public class EDData : IEDData
    {
        public string SubmitSystem(string Name, float X, float Y, float Z, DateTime Visited)
        {
            return string.Format("You visited the system {0} on {1}, at coordinates ({2},{3},{4})", Name, Visited.ToShortDateString(), X, Y, Z);
        }
    }
}

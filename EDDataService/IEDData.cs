using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EDDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEDData" in both code and config file together.
    [ServiceContract]
    public interface IEDData
    {
        [OperationContract]
        string SubmitSystem(string Name, float X, float Y, float Z, DateTime Visited);
    }
}

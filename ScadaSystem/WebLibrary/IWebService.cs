using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DatabaseLibrary;

namespace WebLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWebService" in both code and config file together.
    [ServiceContract]
    public interface IWebService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/CreateTables")]
        string CreateTables();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/DropTables")]
        string DropTables();

        [OperationContract]
        [WebInvoke(Method ="GET",ResponseFormat =WebMessageFormat.Json,UriTemplate ="/sensors")]
        List<Sensor> GetAllSensors();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/sensors/{id}")]
        Sensor GetSensorById(string id);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/sensors/{period}/{id}")]
        List<Measurement> GetMeasurements(string id,string period);

        [OperationContract]
        [WebInvoke(Method = "POST",RequestFormat=WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/TestPost/{id}")]
        Limit TestPost(string id);
    }
}

using DatabaseLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WebService : IWebService
    {
        private ScadaDatabase database;

        public WebService(ScadaDatabase database)
        {
            this.database = database;
        }

        string IWebService.CreateTables()
        {
            database.CreateTables();
            return "Successfully created tables";
        }

        string IWebService.DropTables()
        {
            database.DropTables();
            return "Successfully dropped tables";
        }

        List<Sensor> IWebService.GetAllSensors()
        {
            return database.GetAllSensors();
        }

        Sensor IWebService.GetSensorById(string sensorId)
        {
            return database.GetSensorById(sensorId);
        }

        List<Measurement> IWebService.GetMeasurements(string sensorId,string period)
        {
            const string dateFormat = "yyyy-MM-dd-HH-mm-ss";
            string[] dateStrings = period.Split('&');
            DateTime start = DateTime.ParseExact(dateStrings[0], dateFormat,CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(dateStrings[1], dateFormat,CultureInfo.InvariantCulture);
            return database.GetMeasurements(sensorId, start, end);
        }

        Limit IWebService.TestPost(string id)
        {
            return new Limit
            {
                id = id,
                minVal = -100,
                maxVal = 0
            };
        }
    }
}

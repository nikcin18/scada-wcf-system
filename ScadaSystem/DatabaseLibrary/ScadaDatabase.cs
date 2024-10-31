using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary
{
    public class ScadaDatabase : Database
    {
        private LockHandle lockHandle=new LockHandle();
        public ScadaDatabase(string host, int port, string name, string username, string password)
            : base(host, port, name, username, password){}

        public void CreateTables()
        {
            string commandString = File.ReadAllText(@"db scripts\createTablesScript.sql");
            NpgsqlCommand command = new NpgsqlCommand(commandString, conn);
            command.ExecuteNonQuery();
        }

        public void DropTables()
        {
            string commandString = File.ReadAllText(@"db scripts\dropTablesScript.sql");
            NpgsqlCommand command = new NpgsqlCommand(commandString, conn);
            command.ExecuteNonQuery();
        }

        public List<Sensor> GetAllSensors()
        {
            List<Sensor> result=new List<Sensor>();
            lock (lockHandle)
            {
                string queryString = "SELECT s.sensor_id,s.sensor_name,s.active,l.limit_id,l.min_val,l.max_val,"
                    + "lm.measurement_id,lm.date_time,lm.measured_value,lm.measurement_unit"
                    + " FROM sensors s LEFT JOIN limits l ON (s.limit_id=l.limit_id)"
                    + " LEFT JOIN measurements lm ON (s.last_measurement_id=lm.measurement_id)";
                NpgsqlCommand cmd = new NpgsqlCommand(queryString,conn);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    Sensor sensor = new Sensor
                    {
                        id = reader.GetString(reader.GetOrdinal("s.sensor_id")),
                        name = reader.GetString(reader.GetOrdinal("s.sensor_name")),
                        active = reader.GetBoolean(reader.GetOrdinal("s.active")),
                        limit = new Limit
                        {
                            id = reader.GetString(reader.GetOrdinal("l.limit_id")),
                            minVal = reader.GetDouble(reader.GetOrdinal("l.min_val")),
                            maxVal = reader.GetDouble(reader.GetOrdinal("l.max_val"))
                        }
                    };
                    if (!reader.IsDBNull(reader.GetOrdinal("measurement_unit")))
                    {
                        sensor.lastMeasurement = new Measurement
                        {
                            id = reader.GetString(reader.GetOrdinal("lm.measurement_id")),
                            dateTime = reader.GetDateTime(reader.GetOrdinal("lm.date_time")),
                            unit = reader.GetString(reader.GetOrdinal("lm.measurement_unit")),
                            value = reader.GetDouble(reader.GetOrdinal("lm.measured_value"))
                        };
                    }
                    result.Add(sensor);
                }
                reader.Close();
            }
            return result;
        }

        public Sensor GetSensorById(string sensorId)
        {
            string queryString = "SELECT s.sensor_id,s.sensor_name,s.active,l.limit_id,l.min_val,l.max_val,"
                    + "lm.measurement_id,lm.date_time,lm.measured_value,lm.measurement_unit"
                    + " FROM sensors s LEFT JOIN limits l ON (s.limit_id=l.limit_id)"
                    + " LEFT JOIN measurements lm ON (s.last_measurement_id=lm.measurement_id)"
                    + $" WHERE s.sensor_id='{sensorId}'";
            NpgsqlCommand cmd = new NpgsqlCommand(queryString, conn);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            Sensor sensor = null;
            if (reader.Read())
            {
                sensor = new Sensor
                {
                    id = reader.GetString(reader.GetOrdinal("sensor_id")),
                    name = reader.GetString(reader.GetOrdinal("sensor_name")),
                    active = reader.GetBoolean(reader.GetOrdinal("active")),
                    limit = new Limit
                    {
                        id = reader.GetString(reader.GetOrdinal("limit_id")),
                        minVal = reader.GetDouble(reader.GetOrdinal("min_val")),
                        maxVal = reader.GetDouble(reader.GetOrdinal("max_val"))
                    }
                };
                if (!reader.IsDBNull(reader.GetOrdinal("measurement_unit")))
                {
                    sensor.lastMeasurement = new Measurement
                    {
                        id = reader.GetString(reader.GetOrdinal("measurement_id")),
                        dateTime = reader.GetDateTime(reader.GetOrdinal("date_time")),
                        unit = reader.GetString(reader.GetOrdinal("measurement_unit")),
                        value = reader.GetDouble(reader.GetOrdinal("measured_value"))
                    };
                }
            }
            reader.Close();
            return sensor;
        }

        public List<Measurement> GetMeasurements(string sensorId,DateTime start,DateTime end)
        {
            List<Measurement> result = new List<Measurement>();
            return result;
        }

        public Sensor AddSensor(Sensor sensor)
        {
            Console.WriteLine("AddSensor ...");
            return null;
        }

        public Sensor UpdateSensor(Sensor sensor)
        {
            Console.WriteLine("UpdateSensor ...");
            return null;
        }
        public Sensor RemoveSensor(string sensorId)
        {
            Console.WriteLine("RemoveSensor ...");
            return null;
        }

        public string ControlSensors(List<string>actList,List<string>deactList)
        {
            Console.WriteLine("ControlSensors ...");
            return "";
        }

        private string NextId(string table, string column, string prefix)
        {
            string sqlQuery = $"SELECT {column} FROM {table} WHERE {column} ~ '{prefix}[0-9]+'";
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, conn);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader == null) return prefix + "0";
            int maxId = 0;
            while (reader.Read())
            {
                int id = int.Parse(reader.GetString(reader.GetOrdinal(column)).Substring(prefix.Length));
                maxId = (maxId > id) ? maxId : id;
            }
            reader.Close();
            return prefix + (maxId + 1).ToString();
        }
    }

    [DataContract]
    public class Limit
    {
        [DataMember]
        public string id {  get; set; }
        [DataMember]
        public double minVal { get; set; }
        [DataMember]
        public double maxVal { get; set; }
    }

    public class Measurement
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string unit { get; set; }
        [DataMember]
        public double value { get; set; }
        [DataMember]
        public DateTime dateTime { get; set; }
    }

    public class Sensor
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public bool active { get; set; }
        [DataMember]
        public Limit limit { get; set; }
        [DataMember]
        public Measurement lastMeasurement { get; set; }
    }

    public class ControlCommand
    {
        [DataMember]
        public List<string> actList { get; set; }
        [DataMember]
        public List<string> deactList { get; set; }
    }

    public class LockHandle { }
}

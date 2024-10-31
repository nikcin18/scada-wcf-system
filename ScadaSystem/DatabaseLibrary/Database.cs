using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DatabaseLibrary
{
    public class Database:IDisposable
    {
        protected NpgsqlConnection conn = null;

        protected Database(string host, int port, string name, string username, string password)
        {
            string connString = $"Host={host};"
                + $"Port={port};"
                + $"Database={name};"
                + $"Username={username};"
                + $"Password={password};";
            conn = new NpgsqlConnection(connString);
            conn.Open();
            if(conn==null || conn.State != ConnectionState.Open)
            {
                throw new Exception("Failed to connect");
            }
            Console.WriteLine("Database connection opened...");
        }

        void IDisposable.Dispose()
        {
            Console.WriteLine("Closing database connection...");
            conn?.Close();
        }
    }
}

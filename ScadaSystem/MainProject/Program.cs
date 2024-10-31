using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary;
using WebLibrary;

namespace MainProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connLines = File.ReadAllLines(@"connection.txt");
                if (connLines == null || connLines.Length < 5)
                {
                    throw new Exception("Failed to read database connection parameters");
                }
                ScadaDatabase database = new ScadaDatabase(connLines[0], int.Parse(connLines[1]), connLines[2], connLines[3], connLines[4]);
                WebService service = new WebService(database);

                // Create a ServiceHost instance
                using (ServiceHost host = new ServiceHost(service))
                {
                    try
                    {
                        // Open the host to start listening for incoming messages
                        host.Open();
                        Console.WriteLine("Successfully hosted web service");

                        Console.WriteLine("Press Enter to stop web service...");
                        Console.ReadLine();

                        // Close the host
                        host.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        host.Abort();
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Exception!!!!");
                Console.WriteLine(ex.Message);
            }
        }
    }
}

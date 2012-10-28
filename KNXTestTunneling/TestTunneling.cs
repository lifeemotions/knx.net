using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KNXTest
{
    class TestTunneling
    {
        static void Main(string[] args)
        {
            KNXLib.KNXConnection connection = new KNXLib.KNXConnectionTunneling("10.100.24.130", 3671, "10.0.8.6", 3671);
            connection.Debug = true;
            connection.Connect();
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);
            connection.KNXStatusDelegate += new KNXLib.KNXConnection.KNXStatus(Status);

            Console.WriteLine("Press [ENTER] to send command (5/0/16) - true");
            Console.ReadLine();
            connection.Action("5/0/16", true);
            Thread.Sleep(5000);
            connection.RequestStatus("5/2/16");
            Thread.Sleep(5000);
            Console.WriteLine("Press [ENTER] to send command (5/0/16) - false");
            Console.ReadLine();
            connection.Action("5/0/16", false);
            Thread.Sleep(5000);
            connection.RequestStatus("5/2/16");
            Thread.Sleep(5000);

            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            connection.Disconnect();
            System.Environment.Exit(0);
        }

        static void Event(string address, string state)
        {
            Console.WriteLine("New Event: device " + address + " has status " + state);
        }
        static void Status(string address, string state)
        {
            Console.WriteLine("New Status: device " + address + " has status " + state);
        }
    }
}

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

            Console.WriteLine("Press [ENTER] to send command (0/0/15) - true");
            Console.ReadLine();
            connection.Action("5/0/15", true);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (0/0/15) - false");
            Console.ReadLine();
            connection.Action("5/0/15", false);
            Thread.Sleep(2000);

            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            connection.Disconnect();
            System.Environment.Exit(0);
        }

        static void Event(string address, string state)
        {
            Console.WriteLine("New Event: device " + address + " has status " + state);
        }
    }
}

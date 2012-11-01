using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KNXTest
{
    class TestTunneling
    {

        private static KNXLib.KNXConnection connection = null;
        static void Main(string[] args)
        {
            connection = new KNXLib.KNXConnectionTunneling("10.100.26.20", 3671, "10.100.26.53", 3671);
            connection.Debug = false;
            connection.Connect();
            connection.KNXConnectedDelegate += new KNXLib.KNXConnection.KNXConnected(Connected);
            connection.KNXDisconnectedDelegate += new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);
            connection.KNXStatusDelegate += new KNXLib.KNXConnection.KNXStatus(Status);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("Press [ENTER] to send command (4/0/23) - true");
                //Console.ReadLine();
                connection.Action("4/0/23", true);
                Thread.Sleep(1000);
                Console.WriteLine("Requesting status of 4/1/23");
                connection.RequestStatus("4/1/23");
                Thread.Sleep(1000);
                //Console.WriteLine("Press [ENTER] to send command (4/0/23) - false");
                //Console.ReadLine();
                //connection.Action("4/0/23", false);
                //Thread.Sleep(1000);
                //Console.WriteLine("Requesting status of 4/1/23");
                //connection.RequestStatus("4/1/23");
                //Thread.Sleep(1000);
            }
            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            connection.KNXDisconnectedDelegate -= new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
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
        static void Connected()
        {
            Console.WriteLine("Connected!");
        }
        static void Disconnected()
        {
            Console.WriteLine("Disconnected! Reconnecting");
            if (connection != null)
            {
                Thread.Sleep(1000);
                connection.Connect();
            }
        }
    }
}

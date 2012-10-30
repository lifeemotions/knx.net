using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KNXTest
{
    class TestRouting
    {

        private static KNXLib.KNXConnection connection = null;
        static void Main(string[] args)
        {
            connection = new KNXLib.KNXConnectionRouting();
            connection.Debug = false;
            connection.Connect();
            connection.KNXConnectedDelegate += new KNXLib.KNXConnection.KNXConnected(Connected);
            connection.KNXDisconnectedDelegate += new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);
            connection.KNXStatusDelegate += new KNXLib.KNXConnection.KNXStatus(Status);

            //for (int i = 0; i < 9000; i++)
            //{
            //    connection.RequestStatus("5/2/15");
            //    Thread.Sleep(100);
            //}

            Console.WriteLine("Press [ENTER] to send command (5/0/15) - true");
            Console.ReadLine();
            connection.Action("5/0/15", true);
            Thread.Sleep(5000);
            Console.WriteLine("Requesting status of 5/2/15");
            connection.RequestStatus("5/2/15");
            Thread.Sleep(5000);
            Console.WriteLine("Press [ENTER] to send command (5/0/15) - false");
            Console.ReadLine();
            connection.Action("5/0/15", false);
            Thread.Sleep(5000);
            Console.WriteLine("Requesting status of 5/2/15");
            connection.RequestStatus("5/2/15");
            Thread.Sleep(5000);

            //Thread.Sleep(2000);
            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
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

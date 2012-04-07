using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KNXTest
{
    class Program
    {
        static void Main(string[] args)
        {
            KNXLib.KNXConnection connection = new KNXLib.KNXConnectionMulticast();
            //connection.Debug = true;
            connection.Connect();
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);

            Console.WriteLine("Press [ENTER] to send command (0/0/1) - true");
            Console.ReadLine();
            connection.Action("0/0/1", true);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (0/0/1) - false");
            Console.ReadLine();
            connection.Action("0/0/1", false);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (0/2/1 - 127)");
            Console.ReadLine();
            connection.Action("0/2/1", 127);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (0/2/1 - 0)");
            Console.ReadLine();
            connection.Action("0/2/1", 0);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (14/2/0 - 0)");
            Console.ReadLine();
            connection.Action("14/2/0", 0);
            Thread.Sleep(2000);
            Console.WriteLine("Press [ENTER] to send command (0/7/0 - false)");
            Console.ReadLine();
            connection.Action("0/7/0", false);

            Thread.Sleep(2000);
            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            System.Environment.Exit(0);
        }

        static void Event(string address, string state)
        {
            Console.WriteLine("New Event: device " + address + " has status " + state);
        }
    }
}

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
            connection.ActionMessageCode = 0x29;
            connection.Connect();
            connection.KNXConnectedDelegate += new KNXLib.KNXConnection.KNXConnected(Connected);
            connection.KNXDisconnectedDelegate += new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);
            connection.KNXStatusDelegate += new KNXLib.KNXConnection.KNXStatus(Status);

            //// LIGHT ON/OFF
            Console.WriteLine("Press [ENTER] to send command (5/0/2) - false");
            Console.ReadLine();
            connection.Action("5/0/2", false);
            Thread.Sleep(200);
            Console.WriteLine("Press [ENTER] to send command (5/0/2) - true");
            Console.ReadLine();
            connection.Action("5/0/2", true);
            Thread.Sleep(200);

            //// BLIND UP/DOWN
            //Console.WriteLine("Press [ENTER] to send command (2/1/1) - false");
            //Console.ReadLine();
            //connection.Action("2/1/1", false);
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (2/1/1) - true");
            //Console.ReadLine();
            //connection.Action("2/1/1", true);
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (2/2/1) - true");
            //Console.ReadLine();
            //connection.Action("2/2/1", true);
            //Thread.Sleep(200);

            //// BLIND UP/DOWN
            //Console.WriteLine("Press [ENTER] to send command (2/3/1) - \x00");
            //Console.ReadLine();
            //connection.Action("2/3/1", 0x00);
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (2/3/1) - \xFF");
            //Console.ReadLine();
            //connection.Action("2/3/1", 0xFF);
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (2/3/1) - \x80");
            //Console.ReadLine();
            //connection.Action("2/3/1", 0x80);
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (2/2/1) - true");
            //Console.ReadLine();
            //connection.Action("2/2/1", true);
            //Thread.Sleep(200);

            // TEMPERATURE SETPOINT
            //Console.WriteLine("Press [ENTER] to send command (1/1/16) - 28ºC");
            //Console.ReadLine();
            //connection.Action("1/1/16", connection.toDPT("9.001", 28.0f));
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (1/1/16) - 27ºC");
            //Console.ReadLine();
            //connection.Action("1/1/16", connection.toDPT("9.001", 27.0f));
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (1/1/16) - 26ºC");
            //Console.ReadLine();
            //connection.Action("1/1/16", connection.toDPT("9.001", 26.0f));
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (1/1/16) - 25ºC");
            //Console.ReadLine();
            //connection.Action("1/1/16", connection.toDPT("9.001", 25.0f));
            //Thread.Sleep(200);
            //Console.WriteLine("Press [ENTER] to send command (1/1/16) - 24ºC");
            //Console.ReadLine();
            //connection.Action("1/1/16", connection.toDPT("9.001", 24.0f));
            //Thread.Sleep(200);

            // 1/1/16
            // 1/1/18 feedback
            // 1/1/17 temp feedback

            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            System.Environment.Exit(0);
        }

        static void Event(string address, string state)
        {
            if (address.Equals("1/1/18") || address.Equals("1/1/17"))
            {
                float temp = (float)connection.fromDPT("9.001", state);
                Console.WriteLine("New Event: TEMPERATURE device " + address + " has status (" + state + ")" + temp);
            }
            if (address.Equals("5/1/2"))
            {
                Console.WriteLine("New Event: LIGHT device " + address + " has status (" + state + ")" + state);
            }
            else
            {
                Console.WriteLine("New Event: device " + address + " has status " + state);
            }
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

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
            connection = new KNXLib.KNXConnectionTunneling("10.0.2.183", 3671, "10.0.0.99", 3671);
            connection.Debug = false;
            connection.Connect();
            connection.KNXConnectedDelegate += new KNXLib.KNXConnection.KNXConnected(Connected);
            connection.KNXDisconnectedDelegate += new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
            connection.KNXEventDelegate += new KNXLib.KNXConnection.KNXEvent(Event);
            connection.KNXStatusDelegate += new KNXLib.KNXConnection.KNXStatus(Status);


            string devAddress = "1/0/5";
            float value = 18.0f;

            for (int i = 0; i <= 10; i++)
            {
                byte[] temperature = null;
                try
                {
                    temperature = connection.toDPT("9.001", value);
                }
                catch (Exception)
                {
                    Console.WriteLine("DriverKNX: NOT sending " + devAddress + " (9.001) - " + value + " (ERROR IN VALUE)");
                    return;
                }
                Console.WriteLine("Press [ENTER] to send " + value);
                Console.ReadLine();

                Console.WriteLine("DriverKNX: sending " + devAddress + " (9.001) - " + value + "(" + BitConverter.ToString(temperature) + ")");
                connection.Action(devAddress, temperature);

                value += 1f;

            }


            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            connection.KNXDisconnectedDelegate -= new KNXLib.KNXConnection.KNXDisconnected(Disconnected);
            connection.Disconnect();
            System.Environment.Exit(0);
        }

        static void Event(string address, string state)
        {
            if (address.Equals("1/0/1") || address.Equals("1/0/6"))
            {
                Console.WriteLine("New Event: device " + address + " has status (" + state + ")" + connection.fromDPT("9.001", state));
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

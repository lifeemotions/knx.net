using System;
using System.Linq;
using System.Threading;
using KNXLib;

namespace KNXTest
{
    public class TestTunneling
    {
        private static KNXConnection _connection;

        private static void Main()
        {
            _connection = new KNXConnectionTunneling("10.0.2.183", 3671, "10.0.0.186", 3671) { Debug = false };
            _connection.KNXConnectedDelegate += Connected;
            _connection.KNXDisconnectedDelegate += Disconnected;
            _connection.KNXEventDelegate += Event;
            _connection.KNXStatusDelegate += Status;
            _connection.Connect();

            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();

            _connection.KNXDisconnectedDelegate -= Disconnected;
            _connection.Disconnect();
            Environment.Exit(0);
        }

        private static void Event(string address, string state)
        {
            if (address.Equals("1/2/1") || address.Equals("1/2/2"))
            {
                Console.WriteLine("New Event: device " + address + " has status (" + state + ") --> " + _connection.FromDPT("9.001", state));
            }
            else if (
                address.Equals("1/2/3") ||
                address.Equals("1/2/4") ||
                address.Equals("1/2/5") ||
                address.Equals("1/2/5") ||
                address.Equals("1/2/6") ||
                address.Equals("1/2/7") ||
                address.Equals("1/2/8") ||
                address.Equals("1/2/9") ||
                address.Equals("1/2/10") ||
                address.Equals("1/2/11") ||
                address.Equals("1/2/12") ||
                address.Equals("1/2/13") ||
                address.Equals("1/2/14") ||
                address.Equals("1/2/15") ||
                address.Equals("1/2/16") ||
                address.Equals("1/2/17") ||
                address.Equals("1/2/18"))
            {
                var data = string.Empty;

                if (state.Length == 1)
                {
                    data = ((byte)state[0]).ToString();
                }
                else
                {
                    var bytes = new byte[state.Length];
                    for (var i = 0; i < state.Length; i++)
                    {
                        bytes[i] = Convert.ToByte(state[i]);
                    }

                    data = state.Aggregate(data, (current, t) => current + t.ToString());
                }

                Console.WriteLine("New Event: device " + address + " has status (" + state + ") --> " + data);
            }
        }

        private static void Status(string address, string state)
        {
            Console.WriteLine("New Status: device " + address + " has status " + state);
        }

        private static void Connected()
        {
            Console.WriteLine("Connected!");
        }

        private static void Disconnected()
        {
            Console.WriteLine("Disconnected! Reconnecting");
            if (_connection == null)
                return;

            Thread.Sleep(1000);
            _connection.Connect();
        }
    }
}

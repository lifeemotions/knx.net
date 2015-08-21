using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using KNXLib;

namespace KNXTest
{
    public class TestRouting
    {
        private static KnxConnection _connection;

        private const string LightOnOffAddress = "5/0/2";

        private static readonly IList<string> Lights = new List<string> { "5/1/2" };

        private static readonly IDictionary<string, string> Temperatures = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/4/0", "Inkomhal (Plafond)" },
            { "0/4/1", "Toilet beneden (Plafond)" },
            { "0/4/2", "Berging (Plafond)" },
            { "0/4/3", "Garage (Plafond)" },
            { "0/4/4", "Nachthal trap (Plafond)" },
            { "0/4/5", "Nachthal badkamer (Plafond)" },
            { "0/4/6", "Badkamer (Plafond)" },
            { "0/4/7", "Toilet boven (Plafond)" },
            { "0/4/8", "Slaapkamer 1 (Gezamelijk)" },
            { "0/4/9", "Slaapkamer 2 (Merel)" },
            { "0/4/10", "Slaapkamer 3 (Norah)" },
            { "0/4/11", "Bureau" },
            { "0/4/12", "Badkamer" },
            { "0/4/13", "Weerstation" },
            { "0/4/14", "Keuken" },

            { "1/3/0", "Boiler - Buitentemperatuur" },
            { "1/3/1", "Boiler - Temperatuur" },
            { "1/3/5", "Zonnecollector - Zonnecollector temperatuur" },
            { "1/3/6", "Zonnecollector - Zonnecylinder temperatuur" },
            { "1/3/12", "Vloerverwarming - Temperatuur" },
            { "1/3/17", "Warmwater - Temperatuur" }
        });

        private static void Main()
        {
            _connection = new KnxConnectionRouting { Debug = false, ActionMessageCode = 0x29 };
            _connection.KnxConnectedDelegate += Connected;
            _connection.KnxDisconnectedDelegate += Disconnected;
            _connection.KnxEventDelegate += Event;
            _connection.KnxStatusDelegate += Status;
            _connection.Connect();

            //LightOnOff();
            //BlindUpDown1();
            //BlindUpDown2();
            //TemperatureSetpoint();

            Console.WriteLine("Done. Press [ENTER] to finish");
            Console.Read();
            Environment.Exit(0);
        }

        private static void LightOnOff()
        {
            Console.WriteLine("Press [ENTER] to send command ({0}) - false", LightOnOffAddress);
            Console.ReadLine();
            _connection.Action(LightOnOffAddress, false);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command ({0}) - true", LightOnOffAddress);
            Console.ReadLine();
            _connection.Action(LightOnOffAddress, true);
            Thread.Sleep(200);
        }

        private static void BlindUpDown1()
        {
            Console.WriteLine("Press [ENTER] to send command (2/1/1) - false");
            Console.ReadLine();
            _connection.Action("2/1/1", false);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (2/1/1) - true");
            Console.ReadLine();
            _connection.Action("2/1/1", true);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (2/2/1) - true");
            Console.ReadLine();
            _connection.Action("2/2/1", true);
            Thread.Sleep(200);
        }

        private static void BlindUpDown2()
        {
            Console.WriteLine("Press [ENTER] to send command (2/3/1) - \x00");
            Console.ReadLine();
            _connection.Action("2/3/1", 0x00);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (2/3/1) - \xFF");
            Console.ReadLine();
            _connection.Action("2/3/1", 0xFF);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (2/3/1) - \x80");
            Console.ReadLine();
            _connection.Action("2/3/1", 0x80);
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (2/2/1) - true");
            Console.ReadLine();
            _connection.Action("2/2/1", true);
            Thread.Sleep(200);
        }

        private static void TemperatureSetpoint()
        {
            Console.WriteLine("Press [ENTER] to send command (1/1/16) - 28ºC");
            Console.ReadLine();
            _connection.Action("1/1/16", _connection.ToDataPoint("9.001", 28.0f));
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (1/1/16) - 27ºC");
            Console.ReadLine();
            _connection.Action("1/1/16", _connection.ToDataPoint("9.001", 27.0f));
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (1/1/16) - 26ºC");
            Console.ReadLine();
            _connection.Action("1/1/16", _connection.ToDataPoint("9.001", 26.0f));
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (1/1/16) - 25ºC");
            Console.ReadLine();
            _connection.Action("1/1/16", _connection.ToDataPoint("9.001", 25.0f));
            Thread.Sleep(200);

            Console.WriteLine("Press [ENTER] to send command (1/1/16) - 24ºC");
            Console.ReadLine();
            _connection.Action("1/1/16", _connection.ToDataPoint("9.001", 24.0f));
            Thread.Sleep(200);
        }

        private static void Event(string address, byte[] state)
        {
            string description;

            if (Temperatures.TryGetValue(address, out description))
            {
                var temp = (decimal)_connection.FromDataPoint("9.001", state);
                Console.WriteLine("[TEMP] {0} ({1} °C)", description, temp);
            }

            //if (Temperatures.ContainsKey(address))
            //{
            //    var temp = (float)_connection.FromDataPoint("9.001", state);
            //    Console.WriteLine("New Event: TEMPERATURE device " + address + " has status (" + state + ")" + temp);
            //}
            //else if (Lights.Contains(address))
            //{
            //    Console.WriteLine("New Event: LIGHT device " + address + " has status (" + state + ")" + state);
            //}
            //else
            //{
            //    Console.WriteLine("New Event: device " + address + " has status " + state);
            //}
        }

        private static void Status(string address, byte[] state)
        {
            //Console.WriteLine("New Status: device " + address + " has status " + state);
            string description;

            if (Temperatures.TryGetValue(address, out description))
            {
                var temp = (decimal)_connection.FromDataPoint("9.001", state);
                Console.WriteLine("[TEMP] {0} ({1} °C)", description, temp);
            }
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

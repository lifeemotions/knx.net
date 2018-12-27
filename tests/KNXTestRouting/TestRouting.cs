namespace KNXTest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading;

    using KNXLib;
    using FunctionalLiving.Parser;

    public class TestRouting
    {
        private static KnxConnection _connection;

        private const string LightOnOffAddress = "5/0/2";

        private static readonly IList<string> Lights = new List<string> { "5/1/2" };

        // 1.001 Switches
        private static readonly IDictionary<string, string> Switches = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/1/0", "Verlichting - Leefruimte - Spots TV - Aan/Uit" },
            { "0/1/22", "Verlichting - Bureau - Centraal - Aan/Uit" },
        });

        // 1.002 Toggles (boolean)
        private static readonly IDictionary<string, string> Toggles = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/5/4", "Beweging in nachthal trap" },
            { "0/5/5", "Beweging in nachthal badkamer" },
        });

        // 5.001 Percentages (0..100%)
        private static readonly IDictionary<string, string> Percentages = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/2/7", "Verlichting - Bureau - Spots Binnencirkel - Dimmen" },
            { "0/2/8", "Verlichting - Bureau - Spots Buitencirkel - Dimmen" },
        });

        // 7.007 Time (h)
        private static readonly IDictionary<string, string> Duration = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/7/10", "Aantal uren activiteit droogkast" },
            { "0/7/12", "Aantal uren activiteit wasmachine" },
            { "0/7/14", "Aantal uren activiteit diepvries" },
            { "0/7/16", "Aantal uren activiteit koelkast" },
        });

        // 7.012 Current (mA)
        private static readonly IDictionary<string, string> Current = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/7/11", "Huidig verbruik droogkast" },
            { "0/7/13", "Huidig verbruik wasmachine" },
            { "0/7/15", "Huidig verbruik diepvries" },
            { "0/7/17", "Huidig verbruik koelkast" },
        });

        // 9.001 Temperate (degrees C)
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

        // 9.004 Light (lux)
        private static readonly IDictionary<string, string> LightStrength = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/3/4", "Lichtsterkte in nachthal trap" },
            { "0/3/5", "Lichtsterkte in nachthal badkamer" }
        });

        // 9.005 Speed (m/s)
        private static readonly IDictionary<string, string> Speed = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/0/0", "Windsnelheid buiten" }
        });

        // 10.001 Time of day
        private static readonly IDictionary<string, string> Times = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/0/1", "Centrale Tijd" }
        });

        // 11.001 Date
        private static readonly IDictionary<string, string> Dates = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "0/0/2", "Centrale Datum" }
        });

        // 13.010 Energy (Wh)
        private static readonly IDictionary<string, string> EnergyWattHour = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/3/10", "Zonnecollector - Warmteopbrengst vandaag" }
        });

        // 13.013 Energy (kWh)
        private static readonly IDictionary<string, string> EnergyKiloWattHour = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            { "1/3/9", "Zonnecollector - Warmteopbrengst" }
        });

        private static StreamWriter _logFile;

        private static void Main()
        {
            _logFile = new StreamWriter("telegrams.txt");

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
            _logFile.Dispose();
            Environment.Exit(0);
        }

        private static void Event(string address, byte[] state)
        {
            Print(address, state);
        }

        private static void Status(string address, byte[] state)
        {
            Print(address, state);
        }

        private static void Print(string address, byte[] state)
        {
            const int categoryWidth = 15;
            const int descriptionWidth = -70;
            string description;

            if (Switches.TryGetValue(address, out description))
            {
                var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                Console.WriteLine($"{"[ON/OFF]", categoryWidth} {description, descriptionWidth} ({(functionalToggle.Exists() ? functionalToggle.Value.Text : "N/A")})");
            }
            else if (Toggles.TryGetValue(address, out description))
            {
                var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                Console.WriteLine($"{"[TRUE/FALSE]", categoryWidth} {description, descriptionWidth} ({(functionalToggle.Exists() ? functionalToggle.Value.Text : "N/A")})");
            }
            else if (Percentages.TryGetValue(address, out description))
            {
                var functionalPercentage = Category5_Scaling.parseScaling(0, 100, state[0]);
                Console.WriteLine($"{"[PERCENTAGE]", categoryWidth} {description, descriptionWidth} ({functionalPercentage} %)");
            }
            else if (Duration.TryGetValue(address, out description))
            {
                var functionalDuration = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                Console.WriteLine($"{"[DURATION]", categoryWidth} {description, descriptionWidth} ({functionalDuration} h)");
            }
            else if (Current.TryGetValue(address, out description))
            {
                var functionalCurrent = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                Console.WriteLine($"{"[ENERGY]", categoryWidth} {description, descriptionWidth} ({functionalCurrent} mA)");
            }
            else if (Temperatures.TryGetValue(address, out description))
            {
                var temp = _connection.FromDataPoint("9.001", state); // (decimal)
                var functionalTemp = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Console.WriteLine($"{"[TEMP]", categoryWidth} {description, descriptionWidth} (C#: {temp} °C) (F#: {functionalTemp} °C)");
            }
            else if (LightStrength.TryGetValue(address, out description))
            {
                var functionalLightStrength = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Console.WriteLine($"{"[LUX]", categoryWidth} {description, descriptionWidth} ({functionalLightStrength} Lux)");
            }
            else if (Times.TryGetValue(address, out description))
            {
                var functionalTime = Category10_Time.parseTime(state[0], state[1], state[2]);
                Console.WriteLine($"{"[TIME]", categoryWidth} {description, descriptionWidth} ({(functionalTime.Item1.Exists() ? functionalTime.Item1.Value.Text : string.Empty)}, {functionalTime.Item2:c})");
            }
            else if (EnergyWattHour.TryGetValue(address, out description))
            {
                var wattHour = _connection.FromDataPoint("13.010", state); // (int)
                var functionalWattHour = Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3]);
                Console.WriteLine($"{"[ENERGY]", categoryWidth} {description, descriptionWidth} (C#: {wattHour} Wh) (F#: {functionalWattHour} Wh)");
            }
            else if (Dates.TryGetValue(address, out description))
            {
                var date = (DateTime)_connection.FromDataPoint("11.001", state); // (DateTime)
                var functionalDate = Category11_Date.parseDate(state[0], state[1], state[2]);
                Console.WriteLine($"{"[DATE]", categoryWidth} {description, descriptionWidth} (C#: {date:dd/MM/yyyy}) (F#: {functionalDate:dd/MM/yyyy})");
            }
            else if (Speed.TryGetValue(address, out description))
            {
                var speed = _connection.FromDataPoint("9.005", state); // (decimal)
                var functionalSpeed = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Console.WriteLine($"{"[SPEED]",categoryWidth} {description, descriptionWidth} (C#: {speed} m/s) (F#: {functionalSpeed} m/s)");
            }
            else
            {
                _logFile.WriteLine("{0} - {1}", address, BitConverter.ToString(state));
            }
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

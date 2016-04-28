using System;
using System.Globalization;
using System.Linq;
using KNXLib.Log;

namespace KNXLib.DPT
{
    internal sealed class DataPoint8BitNoSignScaledScaling : DataPoint
    {
        public override string[] Ids
        {
            get { return new[] { "5.001" }; }
        }

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte) data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            if (data == null)
                return 0;
            if (data.Length == 2)
                data = data.Skip(1).ToArray();
            else if (data.Length != 1)
                return 0;

            var value = (int) data[0];

            decimal result = value * 100;
            result = result / 255;

            return result;
        }

        public override byte[] ToDataPoint(string value)
        {
            return ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));
        }

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[2];
            dataPoint[0] = 0x00;
            dataPoint[1] = 0x00;

            decimal input = 0;
            if (val is int)
                input = (decimal) ((int) val);
            else if (val is float)
                input = (decimal) ((float) val);
            else if (val is long)
                input = (decimal) ((long) val);
            else if (val is double)
                input = (decimal) ((double) val);
            else if (val is decimal)
                input = (decimal) val;
            else
            {
                Logger.Error("5.001", "input value received is not a valid type");
                return dataPoint;
            }

            if (input < 0 || input > 100)
            {
                Logger.Error("5.001", "input value received is not in a valid range");
                return dataPoint;
            }

            input = input * 255;
            input = input / 100;

            dataPoint[1] = (byte) (input);

            return dataPoint;
        }
    }
}
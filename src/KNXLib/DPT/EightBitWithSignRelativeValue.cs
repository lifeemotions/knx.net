using System;
using System.Globalization;
using System.Linq;

namespace KNXLib.DPT
{
    internal sealed class EightBitWithSignRelativeValue : DataPoint
    {
        public override string[] Ids
        {
            get { return new[] { "6.001", "6.010" }; }
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
            if (data == null || data.Length != 1)
                return 0;

            return (int) ((sbyte) data[0]);
        }

        public override byte[] ToDataPoint(string value)
        {
            return ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));
        }

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[1];
            dataPoint[0] = 0x00;
            
            int input = 0;
            if (val is int)
                input = ((int) val);
            else if (val is float)
                input = (int) ((float) val);
            else if (val is long)
                input = (int) ((long) val);
            else if (val is double)
                input = (int) ((double) val);
            else if (val is decimal)
                input = (int) ((decimal) val);
            else
                return dataPoint;
            
            if (input < -128 || input > 127)
                return dataPoint;

            dataPoint[0] = (byte) ((sbyte) ((int) input));

            return dataPoint;
        }
    }
}
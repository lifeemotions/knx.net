using System;
using System.Globalization;

namespace KNXLib.DPT
{
    internal sealed class Temperature : DataPoint
    {
        public override string Id
        {
            get { return "9.001"; }
        }

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte)data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            // DPT bits high byte: MEEEEMMM, low byte: MMMMMMMM
            // left align all mantissa bits
            Int32 v = ((data[0] & 0x80) << 24) | ((data[0] & 0x7) << 28) | (data[1] << 20);

            // normalize
            v >>= 20;
            Int32 exp = (data[0] & 0x78) >> 3;
            return (float)((1 << exp) * v * 0.01);
        }

        public override byte[] ToDataPoint(string value)
        {
            return ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));
        }

        public override byte[] ToDataPoint(object val)
        {
            var value = (float)val;
            var dataPoint = new byte[3];
            if (value < -273 || value > +670760)
                return null;

            // encoding: value = (0.01*M)*2^E
            var v = (value / 2f) * 100.0f;
            var e = 1;
            for (; v < -2048.0f; v /= 2)
                e++;
            for (; v > 2047.0f; v /= 2)
                e++;

            var m = ((int)Math.Round(v)) & 0x7FF;
            var msb = (short)(e << 3 | m >> 8);
            if (value < 0.0f)
                msb |= 0x80;

            dataPoint[0] = 0x00;
            dataPoint[1] = (byte)msb;
            dataPoint[2] = (byte)m;

            return dataPoint;
        }
    }
}

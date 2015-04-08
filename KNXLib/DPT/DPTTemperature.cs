using System;
using System.Globalization;

namespace KNXLib.DPT
{
    internal sealed class DPTTemperature : DPT
    {
        public override string Id
        {
            get { return "9.001"; }
        }

        public override object FromDPT(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                dataConverted[i] = (byte)data[i];
            }
            return FromDPT(dataConverted);
        }

        public override object FromDPT(byte[] data)
        {
            // DPT bits high byte: MEEEEMMM, low byte: MMMMMMMM
            // left align all mantissa bits
            Int32 v = ((data[0] & 0x80) << 24) | ((data[0] & 0x7) << 28) | (data[1] << 20);

            // normalize
            v >>= 20;
            Int32 exp = (data[0] & 0x78) >> 3;
            return (float)((1 << exp) * v * 0.01);
        }

        public override byte[] ToDPT(string value)
        {
            return ToDPT(float.Parse(value, CultureInfo.InvariantCulture));
        }

        public override byte[] ToDPT(object val)
        {
            float value = (float)val;
            byte[] dst = new byte[3];
            if (value < -273 || value > +670760)
                return null;

            // encoding: value = (0.01*M)*2^E
            float v = (value / 2f) * 100.0f;
            int e = 1;
            for (; v < -2048.0f; v /= 2)
                e++;
            for (; v > 2047.0f; v /= 2)
                e++;
            int m = ((int)Math.Round(v)) & 0x7FF;
            short msb = (short)(e << 3 | m >> 8);
            if (value < 0.0f)
                msb |= 0x80;
            dst[0] = 0x00;
            dst[1] = (byte)msb;
            dst[2] = (byte)m;

            return dst;
        }
    }
}

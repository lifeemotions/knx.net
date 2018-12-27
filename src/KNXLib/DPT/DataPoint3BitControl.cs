namespace KNXLib.DPT
{
    using System.Globalization;
    using Log;

    internal sealed class DataPoint3BitControl : DataPoint
    {
        public override string[] Ids => new[] { "3.008", "3.007" };

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

            var input = data[0] & 0x0F;

            var direction = input >> 3 == 1;
            var step = input & 0x07;

            if (step != 0)
            {
                if (direction)
                {
                    step = step * -1;
                    step = step + 8;
                }
                else
                {
                    step = step * -1;
                    step = step + 8;
                    step = step * -1;
                }
            }

            return step;
        }

        public override byte[] ToDataPoint(string value) => ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[1];
            dataPoint[0] = 0x00;

            int input;
            if (val is int)
                input = (int) val;
            else if (val is float)
                input = (int) (float) val;
            else if (val is long)
                input = (int) (long) val;
            else if (val is double)
                input = (int) (double) val;
            else if (val is decimal)
                input = (int) (decimal) val;
            else
            {
                Logger.Error("6.xxx", "input value received is not a valid type");
                return dataPoint;
            }

            if (input > 7 || input < -7)
            {
                Logger.Error("3.xxx", "input value received is not in a valid range");
                return dataPoint;
            }

            var direction = 8; // binary 1000


            if (input <= 0)
            {
                direction = 0;
                input = input * -1;
                input = input - 8;
                input = input * -1;
            }
            else
            {
                input = input * -1;
                input = input + 8;
            }

            var step = input & 7;

            dataPoint[0] = (byte) (step | direction);

            return dataPoint;
        }
    }
}

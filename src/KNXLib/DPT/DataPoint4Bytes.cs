namespace KNXLib.DPT
{
    using System;

    internal class DataPoint4Bytes : DataPoint
    {
        public override string[] Ids => new[] { "13.010" };

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte) data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data); //need the bytes in the reverse order

            return BitConverter.ToInt32(data, 0);
        }

        public override byte[] ToDataPoint(string value)
        {
            throw new NotImplementedException();
        }

        public override byte[] ToDataPoint(object value)
        {
            throw new NotImplementedException();
        }
    }
}
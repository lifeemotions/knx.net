using System;

namespace KNXLib.DPT
{
    internal class DataPoint3BytesDate : DataPoint
    {
        public override string[] Ids
        {
            get { return new[] { "11.001" }; }
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
            // date: 15 08 0F : hex->dec DD MM YY
            // day: data[0]
            // month: data[1]
            // year - 2000: data[2]

            return new DateTime(data[2] + 2000, data[1], data[0]);
        }

        public override byte[] ToDataPoint(string value)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] ToDataPoint(object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
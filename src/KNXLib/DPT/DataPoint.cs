using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KNXLibTests")]

namespace KNXLib.DPT
{
    internal abstract class DataPoint
    {
        public abstract string[] Ids { get; }

        public abstract object FromDataPoint(string data);

        public abstract object FromDataPoint(byte[] data);

        public abstract byte[] ToDataPoint(string value);

        public abstract byte[] ToDataPoint(object value);
    }
}

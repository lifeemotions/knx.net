namespace KNXLib.DPT
{
    public abstract class DataPoint
    {
        public abstract string Id { get; }

        public abstract object FromDataPoint(string data);

        public abstract object FromDataPoint(byte[] data);

        public abstract byte[] ToDataPoint(string value);

        public abstract byte[] ToDataPoint(object value);
    }
}

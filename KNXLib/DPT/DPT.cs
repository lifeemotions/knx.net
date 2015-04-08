namespace KNXLib.DPT
{
    public abstract class DPT
    {
        public abstract string Id { get; }

        public abstract object FromDPT(string data);

        public abstract object FromDPT(byte[] data);

        public abstract byte[] ToDPT(string value);

        public abstract byte[] ToDPT(object value);
    }
}

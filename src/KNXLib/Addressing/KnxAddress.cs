namespace KNXLib.Addressing
{
    public abstract class KnxAddress
    {
        protected KnxAddress() {}

        public abstract byte[] GetAddress();
        protected abstract void InternalParse(string address);
        protected abstract void InternalParse(byte[] address);
        public abstract bool IsValid();

        public static KnxAddress Parse(string address)
        {
            if (address.Contains("."))
                return KnxIndividualAddress.Parse(address);

            return KnxGroupAddress.Parse(address);
        }

        public bool Equals(string address) => Equals(Parse(address));

        public override bool Equals(object obj)
        {
            if (!(obj is KnxAddress otherAddress))
                return base.Equals(obj);

            var address1 = GetAddress();
            var address2 = otherAddress.GetAddress();

            return address1[0].Equals(address2[0]) && address1[1].Equals(address2[1]);
        }

        public override int GetHashCode()
        {
            var address = GetAddress();
            return (address[1] << 8) | address[0];
        }
    }
}

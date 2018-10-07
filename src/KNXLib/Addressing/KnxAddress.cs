namespace KNXLib.Addressing
{
    public abstract class KnxAddress
    {
        public KnxAddress()
        {
        }

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

        public bool Equals(string address)
        {
            return Equals(Parse(address));
        }

        public override bool Equals(object obj)
        {
            if (obj is KnxAddress otherAddress)
            {
                byte[] address1 = GetAddress();
                byte[] address2 = otherAddress.GetAddress();

                return address1[0].Equals(address2[0]) && address1[1].Equals(address2[1]);

            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            byte[] address = GetAddress();
            return (address[1] << 8) | address[0];
        }
    }
}

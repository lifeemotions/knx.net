using KNXLib.Enums;

namespace KNXLib.GA
{
    public abstract class KnxGroupAddress
    {
        public int MainGroup { get; set; }
        public int MiddleGroup { get; set; }
        public int SubGroup { get; set; }


        public KnxGroupAddress()
        {
        }

        public KnxGroupAddress(int mainGroup, int middleGroup, int subGroup)
        {
            MainGroup = mainGroup;
            MiddleGroup = middleGroup;
            SubGroup = subGroup;
        }

        public abstract byte[] GetAddress();
        protected abstract void InternalParse(string groupAddress);
        protected abstract void InternalParse(byte[] groupAddress);

        public virtual bool IsValid()
        {
            // The GA 0x00 is not allowed
            if (MainGroup == 0 && MiddleGroup == 0 && SubGroup == 0)
                return false;

            return true;
        }

        public static KnxGroupAddress Parse(string groupAddress)
        {
            var groupParts = groupAddress.Split('/');

            if (groupParts.Length == 3)
                return new KnxThreeLevelGroupAddress(groupAddress);
            else if (groupParts.Length == 2)
                return new KnxTwoLevelGroupAddress(groupAddress);

            return new KnxFreeStyleGroupAddress(groupAddress);
        }

        public static KnxGroupAddress Parse(byte[] groupAddress, KnxGroupAddressStyle style)
        {
            if (style == KnxGroupAddressStyle.ThreeLevel)
                return new KnxThreeLevelGroupAddress(groupAddress);
            else if (style == KnxGroupAddressStyle.TwoLevel)
                return new KnxTwoLevelGroupAddress(groupAddress);

            return new KnxFreeStyleGroupAddress(groupAddress);
        }
    }
}

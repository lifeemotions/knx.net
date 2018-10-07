using KNXLib.Enums;

namespace KNXLib.Addressing
{
    public abstract class KnxGroupAddress : KnxAddress
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

        public override bool IsValid()
        {
            // The GA 0x00 is not allowed
            if (MainGroup == 0 && MiddleGroup == 0 && SubGroup == 0)
                return false;

            return true;
        }

        public static new KnxGroupAddress Parse(string groupAddress)
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

        public bool Equals(int mainGroup, int middleGroup, int subGroup)
        {
            return (MainGroup == mainGroup && MiddleGroup == middleGroup && SubGroup == subGroup);
        }

        public bool Equals(int mainGroup, int subGroup)
        {
            return (MainGroup == mainGroup && MiddleGroup == 0 && SubGroup == subGroup);
        }

        public bool Equals(int subGroup)
        {
            return (MainGroup == 0 && MiddleGroup == 0 && SubGroup == subGroup);
        }

        public override bool Equals(object obj)
        {
            if (obj is KnxGroupAddress)
                return base.Equals(obj);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

namespace KNXLib.Addressing
{
    using Enums;

    public abstract class KnxGroupAddress : KnxAddress
    {
        public int MainGroup { get; set; }
        public int MiddleGroup { get; set; }
        public int SubGroup { get; set; }

        protected KnxGroupAddress() {}

        protected KnxGroupAddress(int mainGroup, int middleGroup, int subGroup)
        {
            MainGroup = mainGroup;
            MiddleGroup = middleGroup;
            SubGroup = subGroup;
        }

        public override bool IsValid()
        {
            // The GA 0x00 is not allowed
            return MainGroup != 0 || MiddleGroup != 0 || SubGroup != 0;
        }

        public new static KnxGroupAddress Parse(string groupAddress)
        {
            var groupParts = groupAddress.Split('/');

            switch (groupParts.Length)
            {
                case 3:
                    return new KnxThreeLevelGroupAddress(groupAddress);
                case 2:
                    return new KnxTwoLevelGroupAddress(groupAddress);
                default:
                    return new KnxFreeStyleGroupAddress(groupAddress);
            }
        }

        public static KnxGroupAddress Parse(byte[] groupAddress, KnxGroupAddressStyle style)
        {
            switch (style)
            {
                case KnxGroupAddressStyle.ThreeLevel:
                    return new KnxThreeLevelGroupAddress(groupAddress);
                case KnxGroupAddressStyle.TwoLevel:
                    return new KnxTwoLevelGroupAddress(groupAddress);
                default:
                    return new KnxFreeStyleGroupAddress(groupAddress);
            }
        }

        public bool Equals(int mainGroup, int middleGroup, int subGroup) 
            => MainGroup == mainGroup && MiddleGroup == middleGroup && SubGroup == subGroup;

        public bool Equals(int mainGroup, int subGroup) 
            => MainGroup == mainGroup && MiddleGroup == 0 && SubGroup == subGroup;

        public bool Equals(int subGroup) 
            => MainGroup == 0 && MiddleGroup == 0 && SubGroup == subGroup;

        public override bool Equals(object obj) => obj is KnxGroupAddress && base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}

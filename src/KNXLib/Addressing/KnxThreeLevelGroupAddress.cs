namespace KNXLib.Addressing
{
    using Exceptions;

    public class KnxThreeLevelGroupAddress : KnxGroupAddress
    {
        public KnxThreeLevelGroupAddress(int mainGroup, int middleGroup, int subGroup) : base(mainGroup, middleGroup, subGroup) {}

        public KnxThreeLevelGroupAddress(string groupAddress) => InternalParse(groupAddress);

        public KnxThreeLevelGroupAddress(byte[] groupAddress) => InternalParse(groupAddress);

        //           +-----------------------------------------------+
        // 16 bits   |             GROUP ADDRESS (3 level)           |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           | E| Main Grp  | Midd G |       Sub Group       |
        //           +-----------------------+-----------------------+
        //           Note: When using extended GAs Bit 7 of OCTET 0 
        //                 is also used for MainGroup

        public override byte[] GetAddress()
        {
            if (!IsValid())
                throw new InvalidKnxAddressException(ToString());

            var addr = new byte[2];

            addr[0] = (byte)(MainGroup << 3);
            addr[0] = (byte)(addr[0] | MiddleGroup);
            addr[1] = (byte)SubGroup;

            return addr;
        }

        public override bool IsValid()
        {
            // Check range for MainGroup 0-31
            if (MainGroup < 0 || MainGroup > 31)
                return false;

            // Check range for MiddleGroup 0-7
            if (MiddleGroup < 0 || MiddleGroup > 7)
                return false;

            // Check range for SubGroup 0-255
            if (SubGroup < 0 || SubGroup > 255)
                return false;

            return base.IsValid();
        }

        protected override void InternalParse(string groupAddress)
        {
            var groupParts = groupAddress.Split('/');

            // Check if GA consists of 3 parts
            if (groupParts.Length != 3)
                return;

            if (!int.TryParse(groupParts[0], out int mainGroup) ||
                !int.TryParse(groupParts[1], out int middleGroup) ||
                !int.TryParse(groupParts[2], out int subGroup))
                return;

            MainGroup = mainGroup;
            MiddleGroup = middleGroup;
            SubGroup = subGroup;
        }

        protected override void InternalParse(byte[] groupAddress)
        {
            if (groupAddress.Length != 2)
                return;

            MainGroup = (groupAddress[0] >> 3);
            MiddleGroup = (groupAddress[0] & 0x07);
            SubGroup = groupAddress[1];
        }

        public override string ToString() => $"{MainGroup}/{MiddleGroup}/{SubGroup}";

        public override bool Equals(object obj) => obj is KnxThreeLevelGroupAddress && base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}

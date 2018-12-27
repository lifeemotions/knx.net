using KNXLib.Exceptions;

namespace KNXLib.Addressing
{
    public class KnxTwoLevelGroupAddress : KnxGroupAddress
    {
        public KnxTwoLevelGroupAddress(int mainGroup, int subGroup) : base(mainGroup, 0, subGroup)
        {
        }

        public KnxTwoLevelGroupAddress(string groupAddress)
        {
            InternalParse(groupAddress);
        }

        public KnxTwoLevelGroupAddress(byte[] groupAddress)
        {
            InternalParse(groupAddress);
        }

        //           +-----------------------------------------------+
        // 16 bits   |             GROUP ADDRESS (2 level)           |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           | E| Main Grp  |            Sub Group           |
        //           +-----------------------+-----------------------+
        //           Note: When using extended GAs Bit 7 of OCTET 0 
        //                 is also used for MainGroup

        public override byte[] GetAddress()
        {
            if (!IsValid())
                throw new InvalidKnxAddressException(ToString());

            var addr = new byte[2];

            addr[0] = (byte)(MainGroup << 3);
            addr[0] = (byte)(addr[0] | (SubGroup >> 8));
            addr[1] = (byte)(SubGroup);

            return addr;
        }

        public override bool IsValid()
        {
            // TwoLevel-Style does not have a MiddleGroup
            if (MiddleGroup != 0)
                return false;

            // Check range for MainGroup 0-31
            if (MainGroup < 0 || MainGroup > 31)
                return false;

            // Check range for SubGroup 0-2047
            if (SubGroup < 0 || SubGroup > 2047)
                return false;

            return base.IsValid();
        }

        protected override void InternalParse(string groupAddress)
        {
            var groupParts = groupAddress.Split('/');

            // Check if GA consists of 2 parts
            if (groupParts.Length != 2)
                return;

            if (!int.TryParse(groupParts[0], out int mainGroup) ||
                !int.TryParse(groupParts[1], out int subGroup))
                return;

            MainGroup = mainGroup;
            MiddleGroup = 0;
            SubGroup = subGroup;
        }

        protected override void InternalParse(byte[] groupAddress)
        {
            if (groupAddress.Length != 2)
                return;

            MainGroup = (groupAddress[0] >> 3);
            MiddleGroup = 0;
            SubGroup = (((groupAddress[0] & 0x07) << 8) | groupAddress[1]);
        }

        public override string ToString()
        {
            return $"{MainGroup}/{SubGroup}";
        }

        public override bool Equals(object obj)
        {
            if (obj is KnxTwoLevelGroupAddress)
                return base.Equals(obj);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

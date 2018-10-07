using KNXLib.Exceptions;

namespace KNXLib.Addressing
{
    public class KnxFreeStyleGroupAddress : KnxGroupAddress
    {

        public KnxFreeStyleGroupAddress(int subGroup) : base(0, 0, subGroup)
        {
        }

        public KnxFreeStyleGroupAddress(string groupAddress)
        {
            InternalParse(groupAddress);
        }

        public KnxFreeStyleGroupAddress(byte[] groupAddress)
        {
            InternalParse(groupAddress);
        }

        //           +-----------------------------------------------+
        // 16 bits   |             GROUP ADDRESS (FreeStyle)         |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           |                  Sub Group                    |
        //           +-----------------------+-----------------------+


        public override byte[] GetAddress()
        {
            if (!IsValid())
                throw new InvalidKnxAddressException(ToString());

            var addr = new byte[2];

            addr[0] = (byte)(SubGroup >> 8);
            addr[1] = (byte)(SubGroup);

            return addr;
        }

        public override bool IsValid()
        {
            // FreeStyle only has a SubGroup
            if (MainGroup != 0 || MiddleGroup != 0)
                return false;

            // Check range for SubGroup 1-65535
            if (SubGroup < 1 || SubGroup > 65535)
                return false;

            return base.IsValid();
        }

        protected override void InternalParse(string groupAddress)
        {
            var groupParts = groupAddress.Split('/');

            // Check if GA consists of 1 part
            if (groupParts.Length != 1)
                return;

            if (!int.TryParse(groupParts[0], out int subGroup))
                return;

            MainGroup = 0;
            MiddleGroup = 0;
            SubGroup = subGroup;
        }

        protected override void InternalParse(byte[] groupAddress)
        {
            if (groupAddress.Length != 2)
                return;

            MainGroup = 0;
            MiddleGroup = 0;
            SubGroup = ((groupAddress[0] << 8) | groupAddress[1]);
        }

        public override string ToString()
        {
            return $"{SubGroup}";
        }

        public override bool Equals(object obj)
        {
            if (obj is KnxFreeStyleGroupAddress)
                return base.Equals(obj);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

namespace KNXLib
{
    using Addressing;
    using Enums;

    public class KnxControlField2
    {
        // Multicast telegrams are send to a group address with a hop count of 6
        // Unicast telegrams are send to a individual address with a hop count of 7
        // see: https://support.knx.org/hc/en-us/articles/115003793409
        public const int DEFAULT_HOP_COUNT_INDIVIDUAL = 7;
        public const int DEFAULT_HOP_COUNT_GROUP = 6;

        // Bit order
        // +---+---+---+---+---+---+---+---+
        // | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // +---+---+---+---+---+---+---+---+

        //  Control Field 2

        //   Bit  |
        //  ------+---------------------------------------------------------------
        //    7   | Destination Address Type - 0x0 individual address
        //        |                          - 0x1 group address
        //  ------+---------------------------------------------------------------
        //   6-4  | Hop Count (0-7)
        //  ------+---------------------------------------------------------------
        //   3-0  | Extended Frame Format - 0x0 standard frame
        //  ------+---------------------------------------------------------------

        public KnxDestinationAddressType DestinationAddressType { get; private set; } = KnxDestinationAddressType.Individual;
        public int HopCount { get; private set; } = DEFAULT_HOP_COUNT_INDIVIDUAL;

        internal KnxControlField2(byte value) => Parse(value);

        internal KnxControlField2(KnxDestinationAddressType destinationAddressType, int hopCount)
        {
            DestinationAddressType = destinationAddressType;
            HopCount = hopCount;
        }

        internal KnxControlField2(KnxAddress address)
        {
            if (address is KnxGroupAddress)
            {
                DestinationAddressType = KnxDestinationAddressType.Group;
                HopCount = DEFAULT_HOP_COUNT_GROUP;
            }
        }

        public byte GetValue()
        {
            byte result = 0b0000_0000;

            if (DestinationAddressType == KnxDestinationAddressType.Group)
                result = (byte)(result | 0b1000_0000);

            result = (byte)(result | ((HopCount << 4) & 0b0111_0000));

            return result;
        }

        private void Parse(byte value)
        {
            // D7
            // 0 = Individual Address
            // 1 = Group Address
            if ((value & 0b1000_0000) != 0)
                DestinationAddressType = KnxDestinationAddressType.Group;

            // D6 + D5 + D4
            // Hop Count
            HopCount = ((value & 0b0111_0000) >> 4);
        }

        public override string ToString() => $"Destination: {DestinationAddressType.ToString()} // HopCount: {HopCount}";
    }
}

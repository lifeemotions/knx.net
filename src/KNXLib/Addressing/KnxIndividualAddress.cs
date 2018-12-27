namespace KNXLib.Addressing
{
    using Exceptions;

    public class KnxIndividualAddress : KnxAddress
    {
        public int Area { get; set; }
        public int Line { get; set; }
        public int Participant { get; set; }

        public KnxIndividualAddress(int area, int line, int participant) 
        {
            Area = area;
            Line = line;
            Participant = participant;
        }

        public KnxIndividualAddress(string address) => InternalParse(address);

        public KnxIndividualAddress(byte[] address) => InternalParse(address);

        //           +-----------------------------------------------+
        // 16 bits   |             INDIVIDUAL ADDRESS                |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           |   Area    |   Line    |      Participant      |
        //           +-----------------------+-----------------------+

        public override byte[] GetAddress()
        {
            if (!IsValid())
                throw new InvalidKnxAddressException(ToString());

            var addr = new byte[2];

            addr[0] = (byte)(Area << 4);
            addr[0] = (byte)(addr[0] | Line);
            addr[1] = (byte)Participant;

            return addr;
        }

        public override bool IsValid()
        {
            // 0.0.0 is not allowed
            // see https://support.knx.org/hc/en-us/articles/115003185789-Individual-Address
            if (Area == 0 && Line == 0 && Participant == 0)
                return false;

            // Check range for Area 0-15
            if (Area < 0 || Area > 15)
                return false;

            // Check range for Line 0-15
            if (Line < 0 || Line > 15)
                return false;

            // Check range for Participant 0-255
            if (Participant < 0 || Participant > 255)
                return false;

            return true;
        }

        protected override void InternalParse(string address)
        {
            var addressParts = address.Split('.');

            // Check if individual address consists of 3 parts
            if (addressParts.Length != 3)
                return;

            if (!int.TryParse(addressParts[0], out int area) ||
                !int.TryParse(addressParts[1], out int line) ||
                !int.TryParse(addressParts[2], out int participant))
                return;

            Area = area;
            Line = line;
            Participant = participant;
        }

        protected override void InternalParse(byte[] address)
        {
            if (address.Length != 2)
                return;

            Area = address[0] >> 4;
            Line = address[0] & 0x0F;
            Participant = address[1];
        }

        public override string ToString() => $"{Area}.{Line}.{Participant}";

        public new static KnxIndividualAddress Parse(string address) => new KnxIndividualAddress(address);

        public static KnxIndividualAddress Parse(byte[] address) => new KnxIndividualAddress(address);

        public bool Equals(int area, int line, int participant) 
            => Area == area && Line == line && Participant == participant;

        public override bool Equals(object obj) => obj is KnxIndividualAddress && base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();
    }
}

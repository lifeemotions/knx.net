namespace KNXLib
{
    internal abstract class KnxSender
    {
        protected KnxSender(KnxConnection connection)
        {
            KnxConnection = connection;
        }

        protected KnxConnection KnxConnection { get; private set; }

        public abstract void SendData(byte[] datagram);

        public void Action(string destinationAddress, byte[] data)
        {
            SendData(CreateActionDatagram(destinationAddress, data));
        }

        public void RequestStatus(string destinationAddress)
        {
            SendData(CreateRequestStatusDatagram(destinationAddress));
        }

        protected abstract byte[] CreateActionDatagram(string destinationAddress, byte[] data);

        protected abstract byte[] CreateRequestStatusDatagram(string destinationAddress);

        protected byte[] CreateActionDatagramCommon(string destinationAddress, byte[] data, byte[] header)
        {
            int i;
            var dataLength = KnxHelper.GetDataLength(data);

            // HEADER
            var datagram = new byte[dataLength + 10 + header.Length];
            for (i = 0; i < header.Length; i++)
                datagram[i] = header[i];

            // CEMI (start at position 6)
            // +--------+--------+--------+--------+----------------+----------------+--------+----------------+
            // |  Msg   |Add.Info| Ctrl 1 | Ctrl 2 | Source Address | Dest. Address  |  Data  |      APDU      |
            // | Code   | Length |        |        |                |                | Length |                |
            // +--------+--------+--------+--------+----------------+----------------+--------+----------------+
            //   1 byte   1 byte   1 byte   1 byte      2 bytes          2 bytes       1 byte      2 bytes
            //
            //  Message Code    = 0x11 - a L_Data.req primitive
            //      COMMON EMI MESSAGE CODES FOR DATA LINK LAYER PRIMITIVES
            //          FROM NETWORK LAYER TO DATA LINK LAYER
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          | Data Link Layer Primitive | Message Code | Data Link Layer Service | Service Description | Common EMI Frame |
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          |        L_Raw.req          |    0x10      |                         |                     |                  |
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          |                           |              |                         | Primitive used for  | Sample Common    |
            //          |        L_Data.req         |    0x11      |      Data Service       | transmitting a data | EMI frame        |
            //          |                           |              |                         | frame               |                  |
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          |        L_Poll_Data.req    |    0x13      |    Poll Data Service    |                     |                  |
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          |        L_Raw.req          |    0x10      |                         |                     |                  |
            //          +---------------------------+--------------+-------------------------+---------------------+------------------+
            //          FROM DATA LINK LAYER TO NETWORK LAYER
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          | Data Link Layer Primitive | Message Code | Data Link Layer Service | Service Description |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |        L_Poll_Data.con    |    0x25      |    Poll Data Service    |                     |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |                           |              |                         | Primitive used for  |
            //          |        L_Data.ind         |    0x29      |      Data Service       | receiving a data    |
            //          |                           |              |                         | frame               |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |        L_Busmon.ind       |    0x2B      |   Bus Monitor Service   |                     |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |        L_Raw.ind          |    0x2D      |                         |                     |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |                           |              |                         | Primitive used for  |
            //          |                           |              |                         | local confirmation  |
            //          |        L_Data.con         |    0x2E      |      Data Service       | that a frame was    |
            //          |                           |              |                         | sent (does not mean |
            //          |                           |              |                         | successful receive) |
            //          +---------------------------+--------------+-------------------------+---------------------+
            //          |        L_Raw.con          |    0x2F      |                         |                     |
            //          +---------------------------+--------------+-------------------------+---------------------+

            //  Add.Info Length = 0x00 - no additional info
            //  Control Field 1 = see the bit structure above
            //  Control Field 2 = see the bit structure above
            //  Source Address  = 0x0000 - filled in by router/gateway with its source address which is
            //                    part of the KNX subnet
            //  Dest. Address   = KNX group or individual address (2 byte)
            //  Data Length     = Number of bytes of data in the APDU excluding the TPCI/APCI bits
            //  APDU            = Application Protocol Data Unit - the actual payload including transport
            //                    protocol control information (TPCI), application protocol control
            //                    information (APCI) and data passed as an argument from higher layers of
            //                    the KNX communication stack
            //

            datagram[i++] =
                KnxConnection.ActionMessageCode != 0x00
                    ? KnxConnection.ActionMessageCode
                    : (byte)0x11;

            datagram[i++] = 0x00;
            datagram[i++] = 0xAC;

            datagram[i++] =
                KnxHelper.IsAddressIndividual(destinationAddress)
                    ? (byte)0x50
                    : (byte)0xF0;

            datagram[i++] = 0x00;
            datagram[i++] = 0x00;
            var dst_address = KnxHelper.GetAddress(destinationAddress);
            datagram[i++] = dst_address[0];
            datagram[i++] = dst_address[1];
            datagram[i++] = (byte)dataLength;
            datagram[i++] = 0x00;
            datagram[i] = 0x80;

            KnxHelper.WriteData(datagram, data, i);

            return datagram;
        }

        protected byte[] CreateRequestStatusDatagramCommon(string destinationAddress, byte[] datagram, int cemi_start_pos)
        {
            var i = 0;

            datagram[cemi_start_pos + i++] =
                KnxConnection.ActionMessageCode != 0x00
                    ? KnxConnection.ActionMessageCode
                    : (byte)0x11;

            datagram[cemi_start_pos + i++] = 0x00;
            datagram[cemi_start_pos + i++] = 0xAC;

            datagram[cemi_start_pos + i++] =
                KnxHelper.IsAddressIndividual(destinationAddress)
                    ? (byte)0x50
                    : (byte)0xF0;

            datagram[cemi_start_pos + i++] = 0x00;
            datagram[cemi_start_pos + i++] = 0x00;
            byte[] dst_address = KnxHelper.GetAddress(destinationAddress);
            datagram[cemi_start_pos + i++] = dst_address[0];
            datagram[cemi_start_pos + i++] = dst_address[1];

            datagram[cemi_start_pos + i++] = 0x01;
            datagram[cemi_start_pos + i++] = 0x00;
            datagram[cemi_start_pos + i] = 0x00;

            return datagram;
        }
    }
}

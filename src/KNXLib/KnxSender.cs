using KNXLib.Addressing;
using KNXLib.Enums;

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

        public void Action(KnxAddress destinationAddress, byte[] data)
        {
            SendData(CreateActionDatagram(destinationAddress, data));
        }

        public void RequestStatus(KnxAddress destinationAddress)
        {
            SendData(CreateRequestStatusDatagram(destinationAddress));
        }

        protected abstract byte[] CreateActionDatagram(KnxAddress destinationAddress, byte[] data);

        protected abstract byte[] CreateRequestStatusDatagram(KnxAddress destinationAddress);

        protected byte[] CreateActionDatagramCommon(KnxAddress destinationAddress, byte[] data, byte[] header)
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

            // Additional Info Length
            datagram[i++] = 0x00;

            // Control Fields
            datagram[i++] = new KnxControlField1(KnxTelegramType.StandardFrame, KnxTelegramRepetitionStatus.Original, KnxTelegramPriority.Low).GetValue();
            datagram[i++] = new KnxControlField2(destinationAddress).GetValue();

            // Source Address
            datagram[i++] = 0x00;
            datagram[i++] = 0x01;

            // Destination Address
            var dst_address = destinationAddress.GetAddress();
            datagram[i++] = dst_address[0];
            datagram[i++] = dst_address[1];

            // Data Length
            datagram[i++] = (byte)dataLength;

            // APDU
            datagram[i++] = 0x00;
            datagram[i] = 0x80;

            KnxHelper.WriteData(datagram, data, i);

            return datagram;
        }

        protected byte[] CreateRequestStatusDatagramCommon(KnxAddress destinationAddress, byte[] datagram, int cemi_start_pos)
        {
            var i = cemi_start_pos;

            datagram[i++] =
                KnxConnection.ActionMessageCode != 0x00
                    ? KnxConnection.ActionMessageCode
                    : (byte)0x11;

            // Additional Info Length
            datagram[i++] = 0x00;

            // Control Fields
            datagram[i++] = new KnxControlField1(KnxTelegramType.StandardFrame, KnxTelegramRepetitionStatus.Original, KnxTelegramPriority.Low).GetValue();
            datagram[i++] = new KnxControlField2(destinationAddress).GetValue();

            // Source Address 
            datagram[i++] = 0x00;
            datagram[i++] = 0x01;

            // Destination Address
            byte[] dst_address = destinationAddress.GetAddress();
            datagram[i++] = dst_address[0];
            datagram[i++] = dst_address[1];

            // Data Length
            datagram[i++] = 0x01;

            // APDU
            datagram[i++] = 0x00;
            datagram[i] = 0x00;

            return datagram;
        }
    }
}

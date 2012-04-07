using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal abstract class KNXSender
    {
        #region constructor
        internal KNXSender(KNXConnection connection)
        {
            this.KNXConnection = connection;
        }
        #endregion

        #region variables
        private KNXConnection _connection;
        private KNXConnection KNXConnection
        {
            get
            {
                return this._connection;
            }
            set
            {
                this._connection = value;
            }
        }
        #endregion

        #region send
        internal void Action(string destination_address, byte[] data)
        {
            SendData(CreateDatagram(destination_address, data));
        }

        internal abstract void SendData(byte[] dgram);
        #endregion

        #region datagram processing
        internal byte[] CreateDatagram(string destination_address, byte[] data)
        {
            int data_length = KNXHelper.GetDataLength(data);
            // HEADER
            byte[] dgram = new byte[data_length + 16];
            dgram[0] = 0x06;
            dgram[1] = 0x10;
            dgram[2] = 0x05;
            dgram[3] = 0x30;
            byte[] total_length = BitConverter.GetBytes(data_length + 16);
            dgram[4] = total_length[1];
            dgram[5] = total_length[0];

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
            dgram[6] = 0x11;
            dgram[7] = 0x00;
            dgram[8] = 0xBC;
            if (KNXHelper.IsAddressIndividual(destination_address))
            {
                dgram[9] = 0x50;
            }
            else
            {
                dgram[9] = 0xD0;
            }
            dgram[10] = 0x00;
            dgram[11] = 0x00;
            byte[] dst_address = KNXHelper.GetAddress(destination_address);
            dgram[12] = dst_address[0];
            dgram[13] = dst_address[1];
            dgram[14] = (byte)(data_length);
            dgram[15] = 0x00;
            dgram[16] = 0x80;
            KNXHelper.WriteData(dgram, data);

            return dgram;
        }
        #endregion
    }
}

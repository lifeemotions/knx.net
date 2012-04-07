using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal abstract class KNXReceiver
    {
        #region constructor
        internal KNXReceiver(KNXConnection connection)
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

        private Thread _receiverThread;
        private Thread ReceiverThread
        {
            get
            {
                return this._receiverThread;
            }
            set
            {
                this._receiverThread = value;
            }
        }
        #endregion

        #region start / stop
        internal void Start()
        {
            ReceiverThread = new Thread(this.ReceiverThreadFlow);
            ReceiverThread.Start();
        }
        internal void Stop()
        {
            ReceiverThread.Abort();
        }
        #endregion

        #region thread
        internal abstract void ReceiverThreadFlow();
        #endregion

        #region datagram processing
        internal void ProcessDatagram(byte[] dgram)
        {
            try
            {
                // HEADER
                KNXDatagram datagram = new KNXDatagram();
                datagram.header_length = (int)dgram[0];
                datagram.protocol_version = dgram[1];
                datagram.service_type = new byte[] { dgram[2], dgram[3] };
                datagram.total_length = (int)dgram[4] + (int)dgram[5];

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
                datagram.message_code = dgram[6];
                datagram.aditional_info_length = (int)dgram[7];
                if (datagram.aditional_info_length > 0)
                {
                    datagram.aditional_info = new byte[datagram.aditional_info_length];
                    for (int i = 0; i < datagram.aditional_info_length; i++)
                    {
                        datagram.aditional_info[i] = dgram[8 + i];
                    }
                }
                datagram.control_field_1 = dgram[8 + datagram.aditional_info_length];
                datagram.control_field_2 = dgram[9 + datagram.aditional_info_length];
                datagram.source_address = KNXHelper.GetIndividualAddress(new byte[] { dgram[10 + datagram.aditional_info_length], dgram[11 + datagram.aditional_info_length] });
                if (KNXHelper.GetKNXDestinationAddressType(datagram.control_field_2).Equals(KNXHelper.KNXDestinationAddressType.INDIVIDUAL))
                {
                    datagram.destination_address = KNXHelper.GetIndividualAddress(new byte[] { dgram[12 + datagram.aditional_info_length], dgram[13 + datagram.aditional_info_length] });
                }
                else
                {
                    datagram.destination_address = KNXHelper.GetGroupAddress(new byte[] { dgram[12 + datagram.aditional_info_length], dgram[13 + datagram.aditional_info_length] }, KNXConnection.ThreeLevelGroupAddressing);
                }
                datagram.data_length = (int)dgram[14 + datagram.aditional_info_length];
                datagram.apdu = new byte[datagram.data_length + 1];
                for (int i = 0; i < datagram.apdu.Length; i++)
                {
                    datagram.apdu[i] = dgram[15 + i];
                }
                datagram.data = KNXHelper.GetData(datagram.data_length, datagram.apdu);

                if (KNXConnection.Debug)
                {
                    Console.WriteLine(BitConverter.ToString(dgram));
                    Console.WriteLine("Event Header Length: " + datagram.header_length);
                    Console.WriteLine("Event Protocol Version: " + datagram.protocol_version.ToString("x"));
                    Console.WriteLine("Event Service Type: 0x" + BitConverter.ToString(datagram.service_type).Replace("-", string.Empty));
                    Console.WriteLine("Event Total Length: " + datagram.total_length);

                    Console.WriteLine("Event Message Code: " + datagram.message_code.ToString("x"));
                    Console.WriteLine("Event Aditional Info Length: " + datagram.aditional_info_length);
                    if (datagram.aditional_info_length > 0)
                    {
                        Console.WriteLine("Event Aditional Info: 0x" + BitConverter.ToString(datagram.aditional_info).Replace("-", string.Empty));
                    }
                    Console.WriteLine("Event Control Field 1: " + Convert.ToString(datagram.control_field_1, 2));
                    Console.WriteLine("Event Control Field 2: " + Convert.ToString(datagram.control_field_2, 2));
                    Console.WriteLine("Event Source Address: " + datagram.source_address);
                    Console.WriteLine("Event Destination Address: " + datagram.destination_address);
                    Console.WriteLine("Event Data Length: " + datagram.data_length);
                    Console.WriteLine("Event APDU: 0x" + BitConverter.ToString(datagram.apdu).Replace("-", string.Empty));
                    Console.WriteLine("Event Data: " + datagram.data);
                }

                if (datagram.message_code == 0x29)
                    this.KNXConnection.Event(datagram);
            }
            catch (Exception)
            {
                // ignore, missing warning information
            }
        }
        #endregion

    }
}

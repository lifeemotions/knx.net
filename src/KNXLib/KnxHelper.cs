using System;
using System.Linq;
using KNXLib.Exceptions;

namespace KNXLib
{
    internal class KnxHelper
    {
        #region Address Processing
        //           +-----------------------------------------------+
        // 16 bits   |              INDIVIDUAL ADDRESS               |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           |  Subnetwork Address   |                       |
        //           +-----------+-----------+     Device Address    |
        //           |(Area Adrs)|(Line Adrs)|                       |
        //           +-----------------------+-----------------------+

        //           +-----------------------------------------------+
        // 16 bits   |             GROUP ADDRESS (3 level)           |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           |  | Main Grp  | Midd G |       Sub Group       |
        //           +--+--------------------+-----------------------+

        //           +-----------------------------------------------+
        // 16 bits   |             GROUP ADDRESS (2 level)           |
        //           +-----------------------+-----------------------+
        //           | OCTET 0 (high byte)   |  OCTET 1 (low byte)   |
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //    bits   | 7| 6| 5| 4| 3| 2| 1| 0| 7| 6| 5| 4| 3| 2| 1| 0|
        //           +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        //           |  | Main Grp  |            Sub Group           |
        //           +--+--------------------+-----------------------+
        public static bool IsAddressIndividual(string address)
        {
            return address.Contains('.');
        }

        public static string GetIndividualAddress(byte[] addr)
        {
            return GetAddress(addr, '.', false);
        }

        public static string GetGroupAddress(byte[] addr, bool threeLevelAddressing)
        {
            return GetAddress(addr, '/', threeLevelAddressing);
        }

        private static string GetAddress(byte[] addr, char separator, bool threeLevelAddressing)
        {
            var group = separator.Equals('/');
            string address;

            if (group && !threeLevelAddressing)
            {
                // 2 level group
                address = (addr[0] >> 3).ToString();
                address += separator;
                address += (((addr[0] & 0x07) << 8) + addr[1]).ToString(); // this may not work, must be checked
            }
            else
            {
                // 3 level individual or group
                address = group
                    ? ((addr[0] & 0x7F) >> 3).ToString()
                    : (addr[0] >> 4).ToString();

                address += separator;

                if (group)
                    address += (addr[0] & 0x07).ToString();
                else
                    address += (addr[0] & 0x0F).ToString();

                address += separator;
                address += addr[1].ToString();
            }

            return address;
        }

        public static byte[] GetAddress(string address)
        {
            try
            {
                var addr = new byte[2];
                var threeLevelAddressing = true;
                string[] parts;
                var group = address.Contains('/');

                if (!group)
                {
                    // individual address
                    parts = address.Split('.');
                    if (parts.Length != 3 || parts[0].Length > 2 || parts[1].Length > 2 || parts[2].Length > 3)
                        throw new InvalidKnxAddressException(address);
                }
                else
                {
                    // group address
                    parts = address.Split('/');
                    if (parts.Length != 3 || parts[0].Length > 2 || parts[1].Length > 1 || parts[2].Length > 3)
                    {
                        if (parts.Length != 2 || parts[0].Length > 2 || parts[1].Length > 4)
                            throw new InvalidKnxAddressException(address);

                        threeLevelAddressing = false;
                    }
                }

                if (!threeLevelAddressing)
                {
                    var part = int.Parse(parts[0]);
                    if (part > 15)
                        throw new InvalidKnxAddressException(address);

                    addr[0] = (byte)(part << 3);
                    part = int.Parse(parts[1]);
                    if (part > 2047)
                        throw new InvalidKnxAddressException(address);

                    var part2 = BitConverter.GetBytes(part);
                    if (part2.Length > 2)
                        throw new InvalidKnxAddressException(address);

                    addr[0] = (byte)(addr[0] | part2[0]);
                    addr[1] = part2[1];
                }
                else
                {
                    var part = int.Parse(parts[0]);
                    if (part > 15)
                        throw new InvalidKnxAddressException(address);

                    addr[0] = group
                        ? (byte)(part << 3)
                        : (byte)(part << 4);

                    part = int.Parse(parts[1]);
                    if ((group && part > 7) || (!group && part > 15))
                        throw new InvalidKnxAddressException(address);

                    addr[0] = (byte)(addr[0] | part);
                    part = int.Parse(parts[2]);
                    if (part > 255)
                        throw new InvalidKnxAddressException(address);

                    addr[1] = (byte)part;
                }

                return addr;
            }
            catch (Exception)
            {
                throw new InvalidKnxAddressException(address);
            }
        }
        #endregion

        #region Control Fields
        // Bit order
        // +---+---+---+---+---+---+---+---+
        // | 7 | 6 | 5 | 4 | 3 | 2 | 1 | 0 |
        // +---+---+---+---+---+---+---+---+

        //  Control Field 1

        //   Bit  |
        //  ------+---------------------------------------------------------------
        //    7   | Frame Type  - 0x0 for extended frame
        //        |               0x1 for standard frame
        //  ------+---------------------------------------------------------------
        //    6   | Reserved
        //        |
        //  ------+---------------------------------------------------------------
        //    5   | Repeat Flag - 0x0 repeat frame on medium in case of an error
        //        |               0x1 do not repeat
        //  ------+---------------------------------------------------------------
        //    4   | System Broadcast - 0x0 system broadcast
        //        |                    0x1 broadcast
        //  ------+---------------------------------------------------------------
        //    3   | Priority    - 0x0 system
        //        |               0x1 normal (also called alarm priority)
        //  ------+               0x2 urgent (also called high priority)
        //    2   |               0x3 low
        //        |
        //  ------+---------------------------------------------------------------
        //    1   | Acknowledge Request - 0x0 no ACK requested
        //        | (L_Data.req)          0x1 ACK requested
        //  ------+---------------------------------------------------------------
        //    0   | Confirm      - 0x0 no error
        //        | (L_Data.con) - 0x1 error
        //  ------+---------------------------------------------------------------


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
        public enum KnxDestinationAddressType
        {
            INDIVIDUAL = 0,
            GROUP = 1
        }

        public static KnxDestinationAddressType GetKnxDestinationAddressType(byte control_field_2)
        {
            return (0x80 & control_field_2) != 0
                ? KnxDestinationAddressType.GROUP
                : KnxDestinationAddressType.INDIVIDUAL;
        }

        #endregion

        #region Data Processing
        // In the Common EMI frame, the APDU payload is defined as follows:

        // +--------+--------+--------+--------+--------+
        // | TPCI + | APCI + |  Data  |  Data  |  Data  |
        // |  APCI  |  Data  |        |        |        |
        // +--------+--------+--------+--------+--------+
        //   byte 1   byte 2  byte 3     ...     byte 16

        // For data that is 6 bits or less in length, only the first two bytes are used in a Common EMI
        // frame. Common EMI frame also carries the information of the expected length of the Protocol
        // Data Unit (PDU). Data payload can be at most 14 bytes long.  <p>

        // The first byte is a combination of transport layer control information (TPCI) and application
        // layer control information (APCI). First 6 bits are dedicated for TPCI while the two least
        // significant bits of first byte hold the two most significant bits of APCI field, as follows:

        //   Bit 1    Bit 2    Bit 3    Bit 4    Bit 5    Bit 6    Bit 7    Bit 8      Bit 1   Bit 2
        // +--------+--------+--------+--------+--------+--------+--------+--------++--------+----....
        // |        |        |        |        |        |        |        |        ||        |
        // |  TPCI  |  TPCI  |  TPCI  |  TPCI  |  TPCI  |  TPCI  | APCI   |  APCI  ||  APCI  |
        // |        |        |        |        |        |        |(bit 1) |(bit 2) ||(bit 3) |
        // +--------+--------+--------+--------+--------+--------+--------+--------++--------+----....
        // +                            B  Y  T  E    1                            ||       B Y T E  2
        // +-----------------------------------------------------------------------++-------------....

        //Total number of APCI control bits can be either 4 or 10. The second byte bit structure is as follows:

        //   Bit 1    Bit 2    Bit 3    Bit 4    Bit 5    Bit 6    Bit 7    Bit 8      Bit 1   Bit 2
        // +--------+--------+--------+--------+--------+--------+--------+--------++--------+----....
        // |        |        |        |        |        |        |        |        ||        |
        // |  APCI  |  APCI  | APCI/  |  APCI/ |  APCI/ |  APCI/ | APCI/  |  APCI/ ||  Data  |  Data
        // |(bit 3) |(bit 4) | Data   |  Data  |  Data  |  Data  | Data   |  Data  ||        |
        // +--------+--------+--------+--------+--------+--------+--------+--------++--------+----....
        // +                            B  Y  T  E    2                            ||       B Y T E  3
        // +-----------------------------------------------------------------------++-------------....
        public static string GetData(int dataLength, byte[] apdu)
        {
            
            switch (dataLength)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return Convert.ToChar(0x3F & apdu[1]).ToString();
                case 2:
                    return Convert.ToChar(apdu[2]).ToString();
                default:
                    var data = string.Empty;
                    for (var i = 2; i < apdu.Length; i++)
                        data += Convert.ToChar(apdu[i]);

                    return data;
            }
        }

        public static int GetDataLength(byte[] data)
        {
            if (data.Length <= 0)
                return 0;

            if (data.Length == 1 && data[0] < 0x3F)
                return 1;

            if (data[0] < 0x3F)
                return data.Length;

            return data.Length + 1;
        }

        public static void WriteData(byte[] datagram, byte[] data, int dataStart)
        {
            if (data.Length == 1)
            {
                if (data[0] < 0x3F)
                {
                    datagram[dataStart] = (byte)(datagram[dataStart] | data[0]);
                }
                else
                {
                    datagram[dataStart + 1] = data[0];
                }
            }
            else if (data.Length > 1)
            {
                if (data[0] < 0x3F)
                {
                    datagram[dataStart] = (byte)(datagram[dataStart] | data[0]);

                    for (var i = 1; i < data.Length; i++)
                    {
                        datagram[dataStart + i] = data[i];
                    }
                }
                else
                {
                    for (var i = 0; i < data.Length; i++)
                    {
                        datagram[dataStart + 1 + i] = data[i];
                    }
                }
            }
        }
        #endregion

        #region Service Type

        public enum SERVICE_TYPE
        {
            //0x0201
            SEARCH_REQUEST,
            //0x0202
            SEARCH_RESPONSE,
            //0x0203
            DESCRIPTION_REQUEST,
            //0x0204
            DESCRIPTION_RESPONSE,
            //0x0205
            CONNECT_REQUEST,
            //0x0206
            CONNECT_RESPONSE,
            //0x0207
            CONNECTIONSTATE_REQUEST,
            //0x0208
            CONNECTIONSTATE_RESPONSE,
            //0x0209
            DISCONNECT_REQUEST,
            //0x020A
            DISCONNECT_RESPONSE,
            //0x0310
            DEVICE_CONFIGURATION_REQUEST,
            //0x0311
            DEVICE_CONFIGURATION_ACK,
            //0x0420
            TUNNELLING_REQUEST,
            //0x0421
            TUNNELLING_ACK,
            //0x0530
            ROUTING_INDICATION,
            //0x0531
            ROUTING_LOST_MESSAGE,
            // UNKNOWN
            UNKNOWN
        }

        public static SERVICE_TYPE GetServiceType(byte[] datagram)
        {
            switch (datagram[2])
            {
                case (0x02):
                    {
                        switch (datagram[3])
                        {
                            case (0x06):
                                return SERVICE_TYPE.CONNECT_RESPONSE;
                            case (0x09):
                                return SERVICE_TYPE.DISCONNECT_REQUEST;
                            case (0x0a):
                                return SERVICE_TYPE.DISCONNECT_RESPONSE;
                            case (0x08):
                                return SERVICE_TYPE.CONNECTIONSTATE_RESPONSE;
                        }
                    }
                    break;
                case (0x04):
                    {
                        switch (datagram[3])
                        {
                            case (0x20):
                                return SERVICE_TYPE.TUNNELLING_REQUEST;
                            case (0x21):
                                return SERVICE_TYPE.TUNNELLING_ACK;
                        }
                    }
                    break;
            }
            return SERVICE_TYPE.UNKNOWN;
        }

        public static int GetChannelId(byte[] datagram)
        {
            if (datagram.Length > 6)
                return datagram[6];

            return -1;
        }

        #endregion
    }
}

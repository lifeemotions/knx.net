using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNXLib
{
    public class KNXDatagram
    {
        // HEADER
        internal int header_length;
        internal byte protocol_version;
        internal byte[] service_type;
        internal int total_length;

        // CEMI
        internal byte message_code;
        internal int aditional_info_length;
        internal byte[] aditional_info;
        internal byte control_field_1;
        internal byte control_field_2;
        internal string source_address;
        internal string destination_address;
        internal int data_length;
        internal byte[] apdu;
        internal string data;
    }
}

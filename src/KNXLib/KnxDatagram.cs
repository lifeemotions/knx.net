using KNXLib.Addressing;

namespace KNXLib
{
    internal class KnxDatagram
    {
        // HEADER
        public int header_length;
        public byte protocol_version;
        public byte[] service_type;
        public int total_length;

        // CONNECTION
        public byte channel_id;
        public byte status;

        // CEMI
        public byte message_code;
        public int aditional_info_length;
        public byte[] aditional_info;
        public KnxControlField1 control_field_1;
        public KnxControlField2 control_field_2;
        public KnxAddress source_address;
        public KnxAddress destination_address;
        public int data_length;
        public byte[] apdu;
        public string data;
    }
}

using KNXLib.Addressing;
using System;

namespace KNXLib.Events
{
    public class KnxEventArgs : EventArgs
    {
        public KnxAddress SourceAddress { get; }
        public KnxAddress DestinationAddress { get; }
        public KnxControlField1 ControlField1 { get; }
        public KnxControlField2 ControlField2 { get; }
        public string State { get; }

        internal KnxEventArgs(KnxAddress sourceAddress, KnxAddress destinationAddress, KnxControlField1 controlField1, KnxControlField2 controlField2, string state)
        {
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            ControlField1 = controlField1;
            ControlField2 = controlField2;
            State = state;
        }

        internal KnxEventArgs(KnxDatagram datagram) : 
            this(datagram.source_address, datagram.destination_address, datagram.control_field_1, datagram.control_field_2, datagram.data) {
        }
    }
}

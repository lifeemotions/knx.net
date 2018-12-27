
namespace KNXLib.Events
{
    using System;
    using System.Linq;
    using Addressing;

    public class KnxEventArgs : EventArgs
    {
        public KnxAddress SourceAddress { get; }
        public KnxAddress DestinationAddress { get; }

        public KnxControlField1 ControlField1 { get; }
        public KnxControlField2 ControlField2 { get; }

        public byte[] State { get; }

        public string StateHex => $"0x{string.Join(string.Empty, State.Select(c => ((int)c).ToString("X2")))}";

        internal KnxEventArgs(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress, 
            KnxControlField1 controlField1, 
            KnxControlField2 controlField2, 
            byte[] state)
        {
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            ControlField1 = controlField1;
            ControlField2 = controlField2;
            State = state;
        }

        internal KnxEventArgs(KnxDatagram datagram) : 
            this(
                datagram.source_address, 
                datagram.destination_address, 
                datagram.control_field_1, 
                datagram.control_field_2, 
                datagram.data) {
        }
    }
}

namespace KNXLib.Events
{
    using Addressing;

    public class KnxStatusArgs : KnxEventArgs
    {
        internal KnxStatusArgs(
            KnxAddress sourceAddress,
            KnxAddress destinationAddress,
            KnxControlField1 controlField1,
            KnxControlField2 controlField2,
            byte[] state) : base(sourceAddress, destinationAddress, controlField1, controlField2, state)
        {
        }

        internal KnxStatusArgs(KnxDatagram datagram) : base(datagram)
        {
        }
    }
}

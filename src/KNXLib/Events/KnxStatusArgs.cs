using KNXLib.Addressing;
using System;

namespace KNXLib.Events
{
    public class KnxStatusArgs : KnxEventArgs
    {
        internal KnxStatusArgs(KnxAddress sourceAddress, KnxAddress destinationAddress, KnxControlField1 controlField1, KnxControlField2 controlField2, string state) :
            base(sourceAddress, destinationAddress, controlField1, controlField2, state)
        {
        }

        internal KnxStatusArgs(KnxDatagram datagram) : base(datagram)
        {
        }
    }
}

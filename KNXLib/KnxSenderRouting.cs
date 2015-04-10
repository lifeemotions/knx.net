using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace KNXLib
{
    internal class KnxSenderRouting : KnxSender
    {
        private readonly IList<UdpClient> _udpClients;
        private readonly IPEndPoint _remoteEndpoint;

        internal KnxSenderRouting(KnxConnection connection, IList<UdpClient> udpClients, IPEndPoint remoteEndpoint)
            : base(connection)
        {
            _udpClients = udpClients;
            _remoteEndpoint = remoteEndpoint;
        }

        public override void SendData(byte[] datagram)
        {
            foreach (var client in _udpClients)
                client.Send(datagram, datagram.Length, _remoteEndpoint);
        }

        protected override byte[] CreateActionDatagram(string destinationAddress, byte[] data)
        {
            var dataLength = KnxHelper.GetDataLength(data);

            // HEADER
            var datagram = new byte[6];
            datagram[0] = 0x06;
            datagram[1] = 0x10;
            datagram[2] = 0x05;
            datagram[3] = 0x30;
            var totalLength = BitConverter.GetBytes(dataLength + 16);
            datagram[4] = totalLength[1];
            datagram[5] = totalLength[0];

            return CreateActionDatagramCommon(destinationAddress, data, datagram);
        }

        protected override byte[] CreateRequestStatusDatagram(string destinationAddress)
        {
            // TODO: Test this

            // HEADER
            var datagram = new byte[17];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x05;
            datagram[03] = 0x30;
            datagram[04] = 0x00;
            datagram[05] = 0x11;

            return CreateRequestStatusDatagramCommon(destinationAddress, datagram, 6);
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;

namespace KNXLib
{
    internal class KnxSenderTunneling : KnxSender
    {
        internal KnxSenderTunneling(KnxConnection connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
            : base(connection)
        {
            RemoteEndpoint = remoteEndpoint;
            UdpClient = udpClient;
        }

        private IPEndPoint RemoteEndpoint { get; set; }

        public UdpClient UdpClient { get; set; }

        private KnxConnectionTunneling KnxConnectionTunneling
        {
            get { return (KnxConnectionTunneling)KnxConnection; }
        }

        public void SendDataSingle(byte[] datagram)
        {
            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
        }

        public override void SendData(byte[] datagram)
        {
            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
        }

        public void SendTunnelingAck(byte sequenceNumber)
        {
            // HEADER
            var datagram = new byte[10];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x04;
            datagram[03] = 0x21;
            datagram[04] = 0x00;
            datagram[05] = 0x0A;

            datagram[06] = 0x04;
            datagram[07] = KnxConnectionTunneling.ChannelId;
            datagram[08] = sequenceNumber;
            datagram[09] = 0x00;

            UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
        }

        protected override byte[] CreateActionDatagram(string destinationAddress, byte[] data)
        {
            lock (KnxConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    var dataLength = KnxHelper.GetDataLength(data);

                    // HEADER
                    var datagram = new byte[10];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;

                    var totalLength = BitConverter.GetBytes(dataLength + 20);
                    datagram[04] = totalLength[1];
                    datagram[05] = totalLength[0];

                    datagram[06] = 0x04;
                    datagram[07] = KnxConnectionTunneling.ChannelId;
                    datagram[08] = KnxConnectionTunneling.GenerateSequenceNumber();
                    datagram[09] = 0x00;

                    return CreateActionDatagramCommon(destinationAddress, data, datagram);
                }
                catch
                {
                    KnxConnectionTunneling.RevertSingleSequenceNumber();

                    return null;
                }
            }
        }

        protected override byte[] CreateRequestStatusDatagram(string destinationAddress)
        {
            lock (KnxConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    // HEADER
                    var datagram = new byte[21];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;
                    datagram[04] = 0x00;
                    datagram[05] = 0x15;

                    datagram[06] = 0x04;
                    datagram[07] = KnxConnectionTunneling.ChannelId;
                    datagram[08] = KnxConnectionTunneling.GenerateSequenceNumber();
                    datagram[09] = 0x00;

                    return CreateRequestStatusDatagramCommon(destinationAddress, datagram, 10);
                }
                catch
                {
                    KnxConnectionTunneling.RevertSingleSequenceNumber();

                    return null;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXReceiverTunneling : KNXReceiver
    {
        #region constructor
        internal KNXReceiverTunneling(KNXConnectionTunneling connection, UdpClient udpClient, IPEndPoint localEndpoint)
            : base(connection)
        {
            this.LocalEndpoint = localEndpoint;
            this.UdpClient = udpClient;
        }
        #endregion

        #region variables
        private IPEndPoint _localEndpoint;
        private IPEndPoint LocalEndpoint
        {
            get
            {
                return this._localEndpoint;
            }
            set
            {
                this._localEndpoint = value;
            }
        }

        private UdpClient _udpClient;
        private UdpClient UdpClient
        {
            get
            {
                return this._udpClient;
            }
            set
            {
                this._udpClient = value;
            }
        }
        internal KNXConnectionTunneling KNXConnectionTunneling
        {
            get
            {
                return (KNXConnectionTunneling) base.KNXConnection;
            }
            set
            {
                base.KNXConnection = value;
            }
        }

        #endregion

        #region thread
        internal override void ReceiverThreadFlow()
        {
            try
            {
                byte[] dgram;
                while (true)
                {
                    dgram = UdpClient.Receive(ref this._localEndpoint);
                    ProcessDatagram(dgram);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
        #endregion


        #region datagram processing
        internal override void ProcessDatagram(byte[] dgram)
        {
            try
            {
                switch (KNXHelper.GetServiceType(dgram))
                {
                    case KNXHelper.SERVICE_TYPE.CONNECT_RESPONSE:
                        ProcessConnectResponse(dgram);
                        break;
                    case KNXHelper.SERVICE_TYPE.TUNNELLING_ACK:
                        ProcessTunnelingAck(dgram);
                        break;
                    case KNXHelper.SERVICE_TYPE.DISCONNECT_REQUEST:
                        ProcessDisconnectRequest(dgram);
                        break;
                    default:
                        base.ProcessDatagram(dgram);
                        break;
                }
            }
            catch (Exception)
            {
                // ignore, missing warning information
            }
        }

        private void ProcessDisconnectRequest(byte[] dgram)
        {
        }
        private void ProcessTunnelingAck(byte[] dgram)
        {
        }
        private void ProcessConnectResponse(byte[] dgram)
        {
            // HEADER
            KNXDatagram datagram = new KNXDatagram();
            datagram.header_length = (int)dgram[0];
            datagram.protocol_version = dgram[1];
            datagram.service_type = new byte[] { dgram[2], dgram[3] };
            datagram.total_length = (int)dgram[4] + (int)dgram[5];

            datagram.channel_id = dgram[6];
            datagram.status = dgram[7];

            if (datagram.channel_id == 0x00 && datagram.status == 0x24)
            {
                if (KNXConnection.Debug)
                {
                    Console.WriteLine("KNXReceiverTunneling: Received connect response - No more connections available");
                }
            }
            else
            {
                this.KNXConnectionTunneling.ChannelId = datagram.channel_id;
                this.KNXConnectionTunneling.SequenceNumber = 0x00;

                this.KNXConnectionTunneling.Connected();
            }
        }
        #endregion

    }
}

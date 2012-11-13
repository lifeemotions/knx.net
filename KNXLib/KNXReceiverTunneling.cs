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
        public UdpClient UdpClient
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
                return (KNXConnectionTunneling)base.KNXConnection;
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
            catch (SocketException)
            {
                // ignore, probably reconnect happening
            }
            catch (ObjectDisposedException)
            {
                // ignore, probably reconnect happening
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
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
                    case KNXHelper.SERVICE_TYPE.CONNECTIONSTATE_RESPONSE:
                        ProcessConnectionStateResponse(dgram);
                        break;
                    case KNXHelper.SERVICE_TYPE.TUNNELLING_ACK:
                        ProcessTunnelingAck(dgram);
                        break;
                    case KNXHelper.SERVICE_TYPE.DISCONNECT_REQUEST:
                        ProcessDisconnectRequest(dgram);
                        break;
                    case KNXHelper.SERVICE_TYPE.TUNNELLING_REQUEST:
                        ProcessDatagramHeaders(dgram);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                Console.Write(e.ToString());
                Console.Write(e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.Write(e.InnerException.Message);
                    Console.Write(e.ToString());
                    Console.Write(e.InnerException.StackTrace);
                }
                
                // ignore, missing warning information
            }
        }

        private void ProcessDatagramHeaders(byte[] dgram)
        {
            // HEADER
            KNXDatagram datagram = new KNXDatagram();
            datagram.header_length = (int)dgram[0];
            datagram.protocol_version = dgram[1];
            datagram.service_type = new byte[] { dgram[2], dgram[3] };
            datagram.total_length = (int)dgram[4] + (int)dgram[5];

            byte chID = dgram[7];
            if (chID != this.KNXConnectionTunneling.ChannelId)
                return;

            byte[] cemi = new byte[dgram.Length - 10];
            Array.Copy(dgram, 10, cemi, 0, dgram.Length - 10);

            base.ProcessCEMI(datagram, cemi);

            ((KNXSenderTunneling)KNXConnectionTunneling.KNXSender).SendTunnelingAck(dgram[8]);
        }

        private void ProcessDisconnectRequest(byte[] dgram)
        {
            this.Stop();
            this.KNXConnection.Disconnected();
            this.UdpClient.Close();
        }
        private void ProcessTunnelingAck(byte[] dgram)
        {
            // do nothing
        }
        private void ProcessConnectionStateResponse(byte[] dgram)
        {
            // HEADER
            // 06 10 02 08 00 08 -- 48 21
            KNXDatagram datagram = new KNXDatagram();
            datagram.header_length = (int)dgram[0];
            datagram.protocol_version = dgram[1];
            datagram.service_type = new byte[] { dgram[2], dgram[3] };
            datagram.total_length = (int)dgram[4] + (int)dgram[5];

            datagram.channel_id = dgram[6];
            byte response = dgram[7];

            if (response == 0x21)
            {
                if (KNXConnection.Debug)
                {
                    Console.WriteLine("KNXReceiverTunneling: Received connection state response - No active connection with channel ID " + datagram.channel_id);
                }
                this.KNXConnection.Disconnect();
            }
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

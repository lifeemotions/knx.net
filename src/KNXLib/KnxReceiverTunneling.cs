using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KNXLib
{
    internal class KnxReceiverTunneling : KnxReceiver
    {
        private UdpClient _udpClient;
        private IPEndPoint _localEndpoint;

        private readonly object _rxSequenceNumberLock = new object();
        private byte _rxSequenceNumber;

        internal KnxReceiverTunneling(KnxConnection connection, UdpClient udpClient, IPEndPoint localEndpoint)
            : base(connection)
        {
            _udpClient = udpClient;
            _localEndpoint = localEndpoint;
        }

        private KnxConnectionTunneling KnxConnectionTunneling
        {
            get { return (KnxConnectionTunneling)KnxConnection; }
        }

        public void SetClient(UdpClient client)
        {
            _udpClient = client;
        }

        public override void ReceiverThreadFlow()
        {
            try
            {
                while (true)
                {
                    var datagram = _udpClient.Receive(ref _localEndpoint);
                    ProcessDatagram(datagram);
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

        private void ProcessDatagram(byte[] datagram)
        {
            try
            {
                switch (KnxHelper.GetServiceType(datagram))
                {
                    case KnxHelper.SERVICE_TYPE.CONNECT_RESPONSE:
                        ProcessConnectResponse(datagram);
                        break;
                    case KnxHelper.SERVICE_TYPE.CONNECTIONSTATE_RESPONSE:
                        ProcessConnectionStateResponse(datagram);
                        break;
                    case KnxHelper.SERVICE_TYPE.TUNNELLING_ACK:
                        ProcessTunnelingAck(datagram);
                        break;
                    case KnxHelper.SERVICE_TYPE.DISCONNECT_REQUEST:
                        ProcessDisconnectRequest(datagram);
                        break;
                    case KnxHelper.SERVICE_TYPE.TUNNELLING_REQUEST:
                        ProcessDatagramHeaders(datagram);
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

        private void ProcessDatagramHeaders(byte[] datagram)
        {
            // HEADER
            // TODO: Might be interesting to take out these magic numbers for the datagram indices
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5]
            };

            var channelId = datagram[7];
            if (channelId != KnxConnectionTunneling.ChannelId)
                return;

            var sequenceNumber = datagram[8];
            var process = true;
            lock (_rxSequenceNumberLock)
            {
                if (sequenceNumber <= _rxSequenceNumber)
                    process = false;

                _rxSequenceNumber = sequenceNumber;
            }

            if (process)
            {
                // TODO: Magic number 10, what is it?
                var cemi = new byte[datagram.Length - 10];
                Array.Copy(datagram, 10, cemi, 0, datagram.Length - 10);

                ProcessCEMI(knxDatagram, cemi);
            }

            ((KnxSenderTunneling)KnxConnectionTunneling.KnxSender).SendTunnelingAck(sequenceNumber);
        }

        private void ProcessDisconnectRequest(byte[] datagram)
        {
            var channelId = datagram[6];
            if (channelId != KnxConnectionTunneling.ChannelId)
                return;

            Stop();
            KnxConnection.Disconnected();
            _udpClient.Close();
        }

        private void ProcessTunnelingAck(byte[] datagram)
        {
            // do nothing
        }

        private void ProcessConnectionStateResponse(byte[] datagram)
        {
            // HEADER
            // 06 10 02 08 00 08 -- 48 21
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5],
                channel_id = datagram[6]
            };

            var response = datagram[7];

            if (response != 0x21)
                return;

            if (KnxConnection.Debug)
                Console.WriteLine("KnxReceiverTunneling: Received connection state response - No active connection with channel ID {0}", knxDatagram.channel_id);

            KnxConnection.Disconnect();
        }

        private void ProcessConnectResponse(byte[] datagram)
        {
            // HEADER
            var knxDatagram = new KnxDatagram
            {
                header_length = datagram[0],
                protocol_version = datagram[1],
                service_type = new[] { datagram[2], datagram[3] },
                total_length = datagram[4] + datagram[5],
                channel_id = datagram[6],
                status = datagram[7]
            };

            if (knxDatagram.channel_id == 0x00 && knxDatagram.status == 0x24)
            {
                if (KnxConnection.Debug)
                    Console.WriteLine("KnxReceiverTunneling: Received connect response - No more connections available");
            }
            else
            {
                KnxConnectionTunneling.ChannelId = knxDatagram.channel_id;
                KnxConnectionTunneling.ResetSequenceNumber();

                KnxConnectionTunneling.Connected();
            }
        }
    }
}

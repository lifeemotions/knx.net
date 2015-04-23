using System.Net;
using System.Net.Sockets;
using System.Timers;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KnxConnectionTunneling : KnxConnection
    {
        private readonly IPEndPoint _localEndpoint;
        private readonly Timer _stateRequestTimer;
        private UdpClient _udpClient;
        private byte _sequenceNumber;

        public KnxConnectionTunneling(string remoteIpAddress, int remotePort, string localIpAddress, int localPort)
            : base(remoteIpAddress, remotePort)
        {
            _localEndpoint = new IPEndPoint(IPAddress.Parse(localIpAddress), localPort);

            ChannelId = 0x00;
            SequenceNumberLock = new object();
            _stateRequestTimer = new Timer(60000) { AutoReset = true }; // same time as ETS with group monitor open
            _stateRequestTimer.Elapsed += StateRequest;
        }

        internal byte ChannelId { get; set; }

        internal object SequenceNumberLock { get; set; }

        internal byte GenerateSequenceNumber()
        {
            return _sequenceNumber++;
        }

        internal void RevertSingleSequenceNumber()
        {
            _sequenceNumber--;
        }

        internal void ResetSequenceNumber()
        {
            _sequenceNumber = 0x00;
        }

        public override void Connect()
        {
            try
            {
                if (_udpClient != null)
                {
                    try
                    {
                        _udpClient.Close();
                        _udpClient.Client.Dispose();
                    }
                    catch
                    {
                        // ignore
                    }
                }

                _udpClient = new UdpClient(_localEndpoint)
                {
                    Client = { DontFragment = true, SendBufferSize = 0 }
                };
            }
            catch (SocketException ex)
            {
                throw new ConnectionErrorException(ConnectionConfiguration, ex);
            }

            if (KnxReceiver == null || KnxSender == null)
            {
                KnxReceiver = new KnxReceiverTunneling(this, _udpClient, _localEndpoint);
                KnxSender = new KnxSenderTunneling(this, _udpClient, RemoteEndpoint);
            }
            else
            {
                ((KnxReceiverTunneling)KnxReceiver).SetClient(_udpClient);
                ((KnxSenderTunneling)KnxSender).SetClient(_udpClient);
            }

            KnxReceiver.Start();

            try
            {
                ConnectRequest();
            }
            catch
            {
                // ignore
            }
        }

        public override void Disconnect()
        {
            try
            {
                TerminateStateRequest();
                DisconnectRequest();
                KnxReceiver.Stop();
                _udpClient.Close();
            }
            catch
            {
                // ignore
            }

            base.Disconnected();
        }

        public override void Connected()
        {
            base.Connected();

            InitializeStateRequest();
        }

        public override void Disconnected()
        {
            base.Disconnected();

            TerminateStateRequest();
        }

        private void InitializeStateRequest()
        {
            _stateRequestTimer.Enabled = true;
        }

        private void TerminateStateRequest()
        {
            if (_stateRequestTimer == null)
                return;

            _stateRequestTimer.Enabled = false;
        }

        // TODO: I wonder if we can extract all these types of requests
        private void ConnectRequest()
        {
            // HEADER
            var datagram = new byte[26];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x05;
            datagram[04] = 0x00;
            datagram[05] = 0x1A;

            datagram[06] = 0x08;
            datagram[07] = 0x01;
            datagram[08] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[09] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[12] = (byte)(_localEndpoint.Port >> 8);
            datagram[13] = (byte)_localEndpoint.Port;
            datagram[14] = 0x08;
            datagram[15] = 0x01;
            datagram[16] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[17] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[18] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[19] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[20] = (byte)(_localEndpoint.Port >> 8);
            datagram[21] = (byte)_localEndpoint.Port;
            datagram[22] = 0x04;
            datagram[23] = 0x04;
            datagram[24] = 0x02;
            datagram[25] = 0x00;

            ((KnxSenderTunneling)KnxSender).SendDataSingle(datagram);
        }

        private void StateRequest(object sender, ElapsedEventArgs e)
        {
            // HEADER
            var datagram = new byte[16];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x07;
            datagram[04] = 0x00;
            datagram[05] = 0x10;

            datagram[06] = ChannelId;
            datagram[07] = 0x00;
            datagram[08] = 0x08;
            datagram[09] = 0x01;
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[12] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[13] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[14] = (byte)(_localEndpoint.Port >> 8);
            datagram[15] = (byte)_localEndpoint.Port;

            try
            {
                KnxSender.SendData(datagram);
            }
            catch
            {
                // ignore
            }
        }

        private void DisconnectRequest()
        {
            // HEADER
            var datagram = new byte[16];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x02;
            datagram[03] = 0x09;
            datagram[04] = 0x00;
            datagram[05] = 0x10;

            datagram[06] = ChannelId;
            datagram[07] = 0x00;
            datagram[08] = 0x08;
            datagram[09] = 0x01;
            datagram[10] = _localEndpoint.Address.GetAddressBytes()[0];
            datagram[11] = _localEndpoint.Address.GetAddressBytes()[1];
            datagram[12] = _localEndpoint.Address.GetAddressBytes()[2];
            datagram[13] = _localEndpoint.Address.GetAddressBytes()[3];
            datagram[14] = (byte)(_localEndpoint.Port >> 8);
            datagram[15] = (byte)_localEndpoint.Port;

            KnxSender.SendData(datagram);
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disconnect();
            }
        }
    }
}

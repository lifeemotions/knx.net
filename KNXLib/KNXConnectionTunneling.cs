using System.Net;
using System.Net.Sockets;
using System.Timers;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KnxConnectionTunneling : KnxConnection
    {
        private readonly Timer _stateRequestTimer;

        public KnxConnectionTunneling(string remoteIpAddress, int remotePort, string localIpAddress, int localPort)
            : base(remoteIpAddress, remotePort)
        {
            RemoteEndpoint = new IPEndPoint(IpAddress, remotePort);
            LocalEndpoint = new IPEndPoint(IPAddress.Parse(localIpAddress), localPort);

            ChannelId = 0x00;
            SequenceNumberLock = new object();
            _stateRequestTimer = new Timer(60000) { AutoReset = true }; // same time as ETS with group monitor open
            _stateRequestTimer.Elapsed += StateRequest;
        }

        private UdpClient UdpClient { get; set; }

        private IPEndPoint LocalEndpoint { get; set; }

        private IPEndPoint RemoteEndpoint { get; set; }

        internal byte ChannelId { get; set; }

        internal byte SequenceNumber { get; set; }

        internal object SequenceNumberLock { get; set; }

        internal byte GenerateSequenceNumber()
        {
            return SequenceNumber++;
        }

        internal void RevertSingleSequenceNumber()
        {
            SequenceNumber--;
        }

        public override void Connect()
        {
            try
            {
                if (UdpClient != null)
                {
                    try
                    {
                        UdpClient.Close();
                        UdpClient.Client.Dispose();
                    }
                    catch
                    {
                        // ignore
                    }
                }

                UdpClient = new UdpClient(LocalEndpoint)
                {
                    Client = { DontFragment = true, SendBufferSize = 0 }
                };
            }
            catch (SocketException)
            {
                throw new ConnectionErrorException(Host, Port);
            }

            if (KnxReceiver == null || KnxSender == null)
            {
                KnxReceiver = new KnxReceiverTunneling(this, UdpClient, LocalEndpoint);
                KnxSender = new KNXSenderTunneling(this, UdpClient, RemoteEndpoint);
            }
            else
            {
                ((KnxReceiverTunneling)KnxReceiver).UdpClient = UdpClient;
                ((KNXSenderTunneling)KnxSender).UdpClient = UdpClient;
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
                UdpClient.Close();
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
            var dgram = new byte[26];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x05;
            dgram[04] = 0x00;
            dgram[05] = 0x1A;

            dgram[06] = 0x08;
            dgram[07] = 0x01;
            dgram[08] = LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[09] = LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[10] = LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[11] = LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[12] = (byte)(LocalEndpoint.Port >> 8);
            dgram[13] = (byte)(LocalEndpoint.Port);
            dgram[14] = 0x08;
            dgram[15] = 0x01;
            dgram[16] = LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[17] = LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[18] = LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[19] = LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[20] = (byte)(LocalEndpoint.Port >> 8);
            dgram[21] = (byte)(LocalEndpoint.Port);
            dgram[22] = 0x04;
            dgram[23] = 0x04;
            dgram[24] = 0x02;
            dgram[25] = 0x00;

            ((KNXSenderTunneling)KnxSender).SendDataSingle(dgram);
        }

        private void StateRequest(object sender, ElapsedEventArgs e)
        {
            // HEADER
            var dgram = new byte[16];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x07;
            dgram[04] = 0x00;
            dgram[05] = 0x10;

            dgram[06] = ChannelId;
            dgram[07] = 0x00;
            dgram[08] = 0x08;
            dgram[09] = 0x01;
            dgram[10] = LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[11] = LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[12] = LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[13] = LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[14] = (byte)(LocalEndpoint.Port >> 8);
            dgram[15] = (byte)(LocalEndpoint.Port);

            try
            {
                KnxSender.SendData(dgram);
            }
            catch
            {
                // ignore
            }
        }

        private void DisconnectRequest()
        {
            // HEADER
            var dgram = new byte[16];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x09;
            dgram[04] = 0x00;
            dgram[05] = 0x10;

            dgram[06] = ChannelId;
            dgram[07] = 0x00;
            dgram[08] = 0x08;
            dgram[09] = 0x01;
            dgram[10] = LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[11] = LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[12] = LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[13] = LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[14] = (byte)(LocalEndpoint.Port >> 8);
            dgram[15] = (byte)(LocalEndpoint.Port);

            KnxSender.SendData(dgram);
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KNXConnectionTunneling : KNXConnection
    {
        #region constructor
        public KNXConnectionTunneling(String remoteIP, int remotePort, string localIP, int localPort)
            : base(remoteIP, remotePort)
        {
            RemoteEndpoint = new IPEndPoint(IP, remotePort);
            LocalEndpoint = new IPEndPoint(IPAddress.Parse(localIP), localPort);

            Initialize();
        }

        private void Initialize()
        {
            stateRequestTimer = new Timer(10000);
            stateRequestTimer.AutoReset = true;
            stateRequestTimer.Elapsed += new ElapsedEventHandler(StateRequest);
        }
        #endregion

        #region variables
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

        private IPEndPoint _remoteEndpoint;
        private IPEndPoint RemoteEndpoint
        {
            get
            {
                return this._remoteEndpoint;
            }
            set
            {
                this._remoteEndpoint = value;
            }
        }

        private byte _channelId;
        internal byte ChannelId
        {
            get
            {
                return _channelId;
            }
            set
            {
                this._channelId = value;
            }
        }

        private byte _sequenceNumber;
        internal byte SequenceNumber
        {
            get
            {
                return this._sequenceNumber;
            }
            set
            {
                this._sequenceNumber = value;
            }
        }

        internal byte GenerateSequenceNumber()
        {
            return this._sequenceNumber++;
        }
        #endregion

        #region connection

        public override void Connect()
        {
            try
            {
                UdpClient = new UdpClient(LocalEndpoint);
            }
            catch (SocketException)
            {
                throw new ConnectionErrorException(this.Host, this.Port);
            }

            KNXReceiver = new KNXReceiverTunneling(this, this.UdpClient, LocalEndpoint);
            KNXReceiver.Start();

            KNXSender = new KNXSenderTunneling(this, this.UdpClient, RemoteEndpoint);

            ConnectRequest();
        }

        public override void Disconnect()
        {
            this.TerminateStateRequest();
            this.DisconnectRequest();
            this.KNXReceiver.Stop();
            this.UdpClient.Close();
        }

        #endregion

        #region connect request
        internal void ConnectRequest()
        {
            // HEADER
            byte[] dgram = new byte[26];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x05;
            dgram[04] = 0x00;
            dgram[05] = 0x1A;

            dgram[06] = 0x08;
            dgram[07] = 0x01;
            dgram[08] = this.LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[09] = this.LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[10] = this.LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[11] = this.LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[12] = (byte)(this.LocalEndpoint.Port >> 8);
            dgram[13] = (byte)(this.LocalEndpoint.Port);
            dgram[14] = 0x08;
            dgram[15] = 0x01;
            dgram[16] = this.LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[17] = this.LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[18] = this.LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[19] = this.LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[20] = (byte)(this.LocalEndpoint.Port >> 8);
            dgram[21] = (byte)(this.LocalEndpoint.Port);
            dgram[22] = 0x04;
            dgram[23] = 0x04;
            dgram[24] = 0x02;
            dgram[25] = 0x00;

            this.KNXSender.SendData(dgram);
        }
        #endregion

        #region events
        public override void Connected()
        {
            base.Connected();

            this.InitializeStateRequest();
        }
        #endregion

        #region state request
        private Timer stateRequestTimer;
        private void InitializeStateRequest()
        {
            stateRequestTimer.Enabled = true;
        }
        private void TerminateStateRequest()
        {
            if (stateRequestTimer == null)
                return;

            stateRequestTimer.Enabled = false;
        }
        private void StateRequest(object sender, System.Timers.ElapsedEventArgs e)
        {
            // HEADER
            byte[] dgram = new byte[16];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x07;
            dgram[04] = 0x00;
            dgram[05] = 0x10;

            dgram[06] = this.ChannelId;
            dgram[07] = 0x00;
            dgram[08] = 0x08;
            dgram[09] = 0x01;
            dgram[10] = this.LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[11] = this.LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[12] = this.LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[13] = this.LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[14] = (byte)(this.LocalEndpoint.Port >> 8);
            dgram[15] = (byte)(this.LocalEndpoint.Port);

            this.KNXSender.SendData(dgram);
        }
        #endregion

        #region disconnect request
        internal void DisconnectRequest()
        {
            // HEADER
            byte[] dgram = new byte[16];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x02;
            dgram[03] = 0x09;
            dgram[04] = 0x00;
            dgram[05] = 0x10;

            dgram[06] = this.ChannelId;
            dgram[07] = 0x00;
            dgram[08] = 0x08;
            dgram[09] = 0x01;
            dgram[10] = this.LocalEndpoint.Address.GetAddressBytes()[0];
            dgram[11] = this.LocalEndpoint.Address.GetAddressBytes()[1];
            dgram[12] = this.LocalEndpoint.Address.GetAddressBytes()[2];
            dgram[13] = this.LocalEndpoint.Address.GetAddressBytes()[3];
            dgram[14] = (byte)(this.LocalEndpoint.Port >> 8);
            dgram[15] = (byte)(this.LocalEndpoint.Port);

            this.KNXSender.SendData(dgram);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXSenderTunneling : KnxSender
    {
        #region constructor
        internal KNXSenderTunneling(KnxConnectionTunneling connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
            : base(connection)
        {
            this.RemoteEndpoint = remoteEndpoint;
            this.UdpClient = udpClient;
        }
        #endregion

        #region variables
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

        private KnxConnectionTunneling KNXConnectionTunneling
        {
            get
            {
                return (KnxConnectionTunneling)this.KnxConnection;
            }
            set
            {
                this.KnxConnection = value;
            }
        }
        #endregion

        #region send
        internal void SendDataSingle(byte[] dgram)
        {
            this.UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
        }

        public override void SendData(byte[] datagram)
        {
            this.UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            this.UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            this.UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
            this.UdpClient.Send(datagram, datagram.Length, RemoteEndpoint);
        }
        internal void SendTunnelingAck(byte seq_number)
        {
            // HEADER
            byte[] dgram = new byte[10];
            dgram[00] = 0x06;
            dgram[01] = 0x10;
            dgram[02] = 0x04;
            dgram[03] = 0x21;
            dgram[04] = 0x00;
            dgram[05] = 0x0A;

            dgram[06] = 0x04;
            dgram[07] = this.KNXConnectionTunneling.ChannelId;
            dgram[08] = seq_number;
            dgram[09] = 0x00;

            this.UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
        }
        #endregion

        #region action datagram processing

        protected override byte[] CreateActionDatagram(string destinationAddress, byte[] data)
        {
            lock (this.KNXConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    int data_length = KnxHelper.GetDataLength(data);
                    // HEADER
                    byte[] dgram = new byte[10];
                    dgram[00] = 0x06;
                    dgram[01] = 0x10;
                    dgram[02] = 0x04;
                    dgram[03] = 0x20;
                    byte[] total_length = BitConverter.GetBytes(data_length + 20);
                    dgram[04] = total_length[1];
                    dgram[05] = total_length[0];

                    dgram[06] = 0x04;
                    dgram[07] = this.KNXConnectionTunneling.ChannelId;
                    dgram[08] = this.KNXConnectionTunneling.GenerateSequenceNumber();
                    dgram[09] = 0x00;

                    return base.CreateActionDatagramCommon(destinationAddress, data, dgram);
                }
                catch (Exception)
                {
                    this.KNXConnectionTunneling.RevertSingleSequenceNumber();
                    return null;
                }
            }
        }
        #endregion

        #region request status datagram processing

        protected override byte[] CreateRequestStatusDatagram(string destinationAddress)
        {
            lock (this.KNXConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    // HEADER
                    byte[] dgram = new byte[21];
                    dgram[00] = 0x06;
                    dgram[01] = 0x10;
                    dgram[02] = 0x04;
                    dgram[03] = 0x20;
                    dgram[04] = 0x00;
                    dgram[05] = 0x15;

                    dgram[06] = 0x04;
                    dgram[07] = this.KNXConnectionTunneling.ChannelId;
                    dgram[08] = this.KNXConnectionTunneling.GenerateSequenceNumber();
                    dgram[09] = 0x00;

                    return base.CreateRequestStatusDatagramCommon(destinationAddress, dgram, 10);
                }
                catch (Exception)
                {
                    this.KNXConnectionTunneling.RevertSingleSequenceNumber();
                    return null;
                }
            }
        }
        #endregion
    }
}

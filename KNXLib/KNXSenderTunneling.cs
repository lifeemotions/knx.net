using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXSenderTunneling : KNXSender
    {
        #region constructor
        internal KNXSenderTunneling(KNXConnectionTunneling connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
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

        private KNXConnectionTunneling KNXConnectionTunneling
        {
            get
            {
                return (KNXConnectionTunneling)this.KNXConnection;
            }
            set
            {
                this.KNXConnection = value;
            }
        }
        #endregion

        #region send
        internal override void SendData(byte[] dgram)
        {
            UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
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

            UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
        }
        #endregion

        #region action datagram processing
        internal override byte[] CreateActionDatagram(string destination_address, byte[] data)
        {
            lock (this.KNXConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    int data_length = KNXHelper.GetDataLength(data);
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

                    return base.CreateActionDatagramCommon(destination_address, data, dgram);
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
        internal override byte[] CreateRequestStatusDatagram(string destination_address)
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

                    return base.CreateRequestStatusDatagramCommon(destination_address, dgram, 10);
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

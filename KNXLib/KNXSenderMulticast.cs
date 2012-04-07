using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXSenderMulticast : KNXSender
    {
        #region constructor
        internal KNXSenderMulticast(KNXConnection connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
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
        #endregion

        #region send
        internal override void SendData(byte[] dgram)
        {
            UdpClient.Send(dgram, dgram.Length, RemoteEndpoint);
        }
        #endregion
    }
}

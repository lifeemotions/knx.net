using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KNXConnectionRouting : KNXConnection
    {
        #region constructor
        public KNXConnectionRouting()
            : this("224.0.23.12", 3671)
        {
        }

        public KNXConnectionRouting(int port)
            : this("224.0.23.12", port)
        {
        }

        public KNXConnectionRouting(String host)
            : this(host, 3671)
        {
        }

        public KNXConnectionRouting(String host, int port) : base(host, port)
        {
            RemoteEndpoint = new IPEndPoint(IP, port);
            LocalEndpoint = new IPEndPoint(IPAddress.Any, 0);
        }

        private void Initialize()
        {
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
        #endregion

        #region connection

        public override void Connect()
        {
            try
            {
                UdpClient = new UdpClient(base.Port);
                UdpClient.JoinMulticastGroup(IP);
            }
            catch (SocketException)
            {
                throw new ConnectionErrorException(this.Host, this.Port);
            }

            KNXReceiver = new KNXReceiverRouting(this, UdpClient, LocalEndpoint);
            KNXReceiver.Start();

            KNXSender = new KNXSenderRouting(this, UdpClient, RemoteEndpoint);
        }

        public override void Disconnect()
        {
            this.KNXReceiver.Stop();
            this.UdpClient.DropMulticastGroup(IP);
            this.UdpClient.Close();
        }

        #endregion
    }
}

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

        public KNXConnectionRouting(String host, int port)
            : base(host, port)
        {
            RemoteEndpoint = new IPEndPoint(IP, port);
            LocalEndpoint = new IPEndPoint(IPAddress.Any, port);
            Initialize();
        }

        private void Initialize()
        {
            this.UdpClients = new List<UdpClient>();
        }
        #endregion

        #region variables
        private IList<UdpClient> _udpClients;
        private IList<UdpClient> UdpClients
        {
            get
            {
                return this._udpClients;
            }
            set
            {
                this._udpClients = value;
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

                foreach (IPAddress localIp in Dns.GetHostAddresses(Dns.GetHostName()).Where(i => i.AddressFamily == AddressFamily.InterNetwork))
                {
                    IPAddress ipToUse = localIp;

                    UdpClient client = new UdpClient(new IPEndPoint(ipToUse, this.LocalEndpoint.Port));
                    this.UdpClients.Add(client);
                    client.JoinMulticastGroup(IP, ipToUse);
                }
            }
            catch (SocketException)
            {
                throw new ConnectionErrorException(this.Host, this.Port);
            }

            KNXReceiver = new KNXReceiverRouting(this, this.UdpClients, LocalEndpoint);
            KNXReceiver.Start();

            KNXSender = new KNXSenderRouting(this, this.UdpClients, LocalEndpoint, RemoteEndpoint);

            base.Connected();
        }

        public override void Disconnect()
        {
            this.KNXReceiver.Stop();
            foreach (UdpClient client in this.UdpClients)
            {
                client.DropMulticastGroup(IP);
                client.Close();
            }
        }

        #endregion
    }
}

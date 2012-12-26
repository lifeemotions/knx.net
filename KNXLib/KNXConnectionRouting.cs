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

            this._connected = ConnectionStatus.CONNECTING;
            base.Connected();
        }

        public override void Disconnect()
        {
            this.KNXReceiver.Stop();
            this.UdpClient.DropMulticastGroup(IP);
            this.UdpClient.Close();

            base.Disconnected();
        }

        #endregion
    }
}

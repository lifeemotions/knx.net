using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KnxConnectionRouting : KnxConnection
    {
        private const string DefaultMulticastAddress = "224.0.23.12";
        private const int DefaultMulticastPort = 3671;

        private readonly IPEndPoint _localEndpoint;
        private readonly IList<UdpClient> _udpClients = new List<UdpClient>();

        public KnxConnectionRouting()
            : this(DefaultMulticastAddress, DefaultMulticastPort)
        {
        }

        public KnxConnectionRouting(int port)
            : this(DefaultMulticastAddress, port)
        {
        }

        public KnxConnectionRouting(string host)
            : this(host, DefaultMulticastPort)
        {
        }

        public KnxConnectionRouting(string host, int port)
            : base(host, port)
        {
            _localEndpoint = new IPEndPoint(IPAddress.Any, port);
        }

        public override void Connect()
        {
            try
            {
                var ipv4Addresses =
                    Dns
                    .GetHostAddresses(Dns.GetHostName())
                    .Where(i => i.AddressFamily == AddressFamily.InterNetwork);

                foreach (var localIp in ipv4Addresses)
                {
                    var client = new UdpClient(new IPEndPoint(localIp, _localEndpoint.Port));
                    _udpClients.Add(client);
                    client.JoinMulticastGroup(ConnectionConfiguration.IpAddress, localIp);
                }
            }
            catch (SocketException ex)
            {
                throw new ConnectionErrorException(ConnectionConfiguration, ex);
            }

            // TODO: Maybe if we have a base Connect helper which takes in a KnxReceiver and KnxSender,
            // we can make the property setters more restricted
            KnxReceiver = new KnxReceiverRouting(this, _udpClients);
            KnxReceiver.Start();

            KnxSender = new KnxSenderRouting(this, _udpClients, RemoteEndpoint);

            Connected();
        }

        public override void Disconnect()
        {
            KnxReceiver.Stop();
            foreach (var client in _udpClients)
            {
                client.DropMulticastGroup(ConnectionConfiguration.IpAddress);
                client.Close();
            }
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

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
            LocalEndpoint = new IPEndPoint(IPAddress.Any, port);
            UdpClients = new List<UdpClient>();
        }

        private IPEndPoint LocalEndpoint { get; set; }

        private IList<UdpClient> UdpClients { get; set; }

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
                    var client = new UdpClient(new IPEndPoint(localIp, LocalEndpoint.Port));
                    UdpClients.Add(client);
                    client.JoinMulticastGroup(ConnectionConfiguration.IpAddress, localIp);
                }
            }
            catch (SocketException ex)
            {
                throw new ConnectionErrorException(ConnectionConfiguration, ex);
            }

            // TODO: Maybe if we have a base Connect helper which takes in a KnxReceiver and KnxSender,
            // we can make the property setters more restricted
            KnxReceiver = new KnxReceiverRouting(this, UdpClients);
            KnxReceiver.Start();

            KnxSender = new KnxSenderRouting(this, UdpClients, RemoteEndpoint);

            Connected();
        }

        public override void Disconnect()
        {
            KnxReceiver.Stop();
            foreach (var client in UdpClients)
            {
                client.DropMulticastGroup(ConnectionConfiguration.IpAddress);
                client.Close();
            }
        }
    }
}

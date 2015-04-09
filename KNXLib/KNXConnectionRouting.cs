using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KNXLib.Exceptions;

namespace KNXLib
{
    public class KnxConnectionRouting : KnxConnection
    {
        public KnxConnectionRouting()
            : this("224.0.23.12", 3671)
        {
        }

        // TOOD: Is this a magic IP Address in KNX spec? Or just some test value?
        public KnxConnectionRouting(int port)
            : this("224.0.23.12", port)
        {
        }

        public KnxConnectionRouting(string host)
            : this(host, 3671)
        {
        }

        public KnxConnectionRouting(string host, int port)
            : base(host, port)
        {
            RemoteEndpoint = new IPEndPoint(IpAddress, port);
            LocalEndpoint = new IPEndPoint(IPAddress.Any, port);
            UdpClients = new List<UdpClient>();
        }

        private IList<UdpClient> UdpClients { get; set; }

        private IPEndPoint LocalEndpoint { get; set; }

        private IPEndPoint RemoteEndpoint { get; set; }

        public override void Connect()
        {
            try
            {
                var ipv4Addresses =
                    Dns
                    .GetHostAddresses(Dns.GetHostName())
                    .Where(i => i.AddressFamily == AddressFamily.InterNetwork)
                    .ToList(); // TODO: I can probably leave the ToList off, there are no closures below either

                foreach (var localIp in ipv4Addresses)
                {
                    var client = new UdpClient(new IPEndPoint(localIp, LocalEndpoint.Port));
                    UdpClients.Add(client);
                    client.JoinMulticastGroup(IpAddress, localIp);
                }
            }
            catch (SocketException)
            {
                throw new ConnectionErrorException(Host, Port);
            }

            // TODO: Maybe if we have a base Connect helper which takes in a KnxReceiver and KnxSender,
            // we can make the property setters more restricted
            KnxReceiver = new KnxReceiverRouting(this, UdpClients);
            KnxReceiver.Start();

            KnxSender = new KNXSenderRouting(this, UdpClients, LocalEndpoint, RemoteEndpoint);

            Connected();
        }

        public override void Disconnect()
        {
            KnxReceiver.Stop();
            foreach (var client in UdpClients)
            {
                client.DropMulticastGroup(IpAddress);
                client.Close();
            }
        }
    }
}

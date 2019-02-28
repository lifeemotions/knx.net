using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KNXLib.Exceptions;

namespace KNXLib
{
    /// <summary>
    ///     Class that controls a Routing KNX connection, a routing connection is UDP based and has no state.
    ///     This class will bind to a multicast address to listen for events and send actions and requests to
    ///     that same address
    /// </summary>
    public class KnxConnectionRouting : KnxConnection
    {
        private const string DefaultMulticastAddress = "224.0.23.12";
        private const int DefaultMulticastPort = 3671;

        private readonly IPEndPoint _localEndpoint;
        private readonly IList<UdpClient> _udpClients = new List<UdpClient>();

        /// <summary>
        ///     Initializes a new KNX routing connection with default values. The default multicast address is
        ///     224.0.23.12 and the default port is 3671. Make sure the local system allows UDP messages to this port
        /// </summary>
        public KnxConnectionRouting()
            : this(DefaultMulticastAddress, DefaultMulticastPort)
        {
        }

        /// <summary>
        ///     Initializes a new KNX routing connection with default address and provided port. The default multicast
        ///     address is 224.0.23.12. Make sure the local system allows UDP messages to the provided port
        /// </summary>
        /// <param name="port">UDP port to send/receive KNX messages</param>
        public KnxConnectionRouting(int port)
            : this(DefaultMulticastAddress, port)
        {
        }

        /// <summary>
        ///     Initializes a new KNX routing connection with provided address and default port. The default port is
        ///     3671. Make sure the local system allows UDP messages to this port
        /// </summary>
        /// <param name="host">UDP multicast address to send/receive KNX messages</param>
        public KnxConnectionRouting(string host)
            : this(host, DefaultMulticastPort)
        {
        }

        /// <summary>
        ///     Initializes a new KNX routing connection with provided address and port. Make sure the local system
        ///     allows UDP messages to the provided port
        /// </summary>
        /// <param name="host">UDP multicast address to send/receive KNX messages</param>
        /// <param name="port">UDP port to send/receive KNX messages</param>
        public KnxConnectionRouting(string host, int port)
            : base(host, port)
        {
            _localEndpoint = new IPEndPoint(IPAddress.Any, port);
        }

        /// <summary>
        ///     Start the connection
        /// </summary>
        public override void Connect()
        {
            try
            {
                IEnumerable<IPAddress> ipv4Addresses =
                    Dns
                        .GetHostAddresses(Dns.GetHostName())
                        .Where(i => i.AddressFamily == AddressFamily.InterNetwork);

                foreach (IPAddress localIp in ipv4Addresses)
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

        /// <summary>
        ///     Stop the connection
        /// </summary>
        public override void Disconnect()
        {
            KnxReceiver.Stop();
            foreach (UdpClient client in _udpClients)
            {
                client.DropMulticastGroup(ConnectionConfiguration.IpAddress);
                client.Close();
            }
            base.Disconnected();
        }
    }
}

using System.Net;
using System.Net.Sockets;
using KNXLib.Exceptions;

namespace KNXLib
{
    internal class KnxConnectionConfiguration
    {
        public KnxConnectionConfiguration(string host, int port)
        {
            Host = host;
            Port = port;

            IpAddress = null;
            try
            {
                IpAddress = IPAddress.Parse(host);
            }
            catch
            {
                try
                {
                    IpAddress = Dns.GetHostEntry(host).AddressList[0];
                }
                catch (SocketException)
                {
                }
            }

            if (IpAddress == null)
                throw new InvalidHostException(host);

            EndPoint = new IPEndPoint(IpAddress, port);
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public IPAddress IpAddress { get; private set; }

        public IPEndPoint EndPoint { get; private set; }
    }
}
namespace KNXLib
{
    using System;
    using System.Net;
    using Exceptions;

    internal class KnxConnectionConfiguration
    {
        public string Host { get; }

        public int Port { get; }

        public IPAddress IpAddress { get; }

        public IPEndPoint EndPoint { get; }

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
                catch (Exception)
                {
                    throw new InvalidHostException(host);
                }
            }

            if (IpAddress == null)
                throw new InvalidHostException(host);

            EndPoint = new IPEndPoint(IpAddress, port);
        }
    }
}

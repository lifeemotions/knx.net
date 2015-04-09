using System;

namespace KNXLib.Exceptions
{
    public class ConnectionErrorException : Exception
    {
        private readonly string _host;
        private readonly int _port;

        public ConnectionErrorException(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public override string ToString()
        {
            return string.Format("ConnectionErrorException: Error connecting to {0}:{1}", _host, _port);
        }
    }
}
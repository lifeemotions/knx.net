using System;

namespace KNXLib.Exceptions
{
    public class ConnectionErrorException : System.Exception
    {
        private string host;
        private int port;

        public ConnectionErrorException(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public override string ToString()
        {
            return "ConnectionErrorException: Error connecting to " + host + ":" + port;
        }
    }
}
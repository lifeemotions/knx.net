using System;

namespace KNXLib.Exceptions
{
    internal class ConnectionErrorException : Exception
    {
        public ConnectionErrorException(KnxConnectionConfiguration configuration)
            : base(string.Format("ConnectionErrorException: Error connecting to {0}:{1}", configuration.Host, configuration.Port))
        {
        }

        public ConnectionErrorException(KnxConnectionConfiguration configuration, Exception innerException)
            : base(string.Format("ConnectionErrorException: Error connecting to {0}:{1}", configuration.Host, configuration.Port), innerException)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
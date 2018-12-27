namespace KNXLib.Exceptions
{
    using System;

    internal class ConnectionErrorException : Exception
    {
        public ConnectionErrorException(KnxConnectionConfiguration configuration)
            : base($"ConnectionErrorException: Error connecting to {configuration.Host}:{configuration.Port}")
        {
        }

        public ConnectionErrorException(KnxConnectionConfiguration configuration, Exception innerException)
            : base($"ConnectionErrorException: Error connecting to {configuration.Host}:{configuration.Port}", innerException)
        {
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
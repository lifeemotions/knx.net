using System;

namespace KNXLib.Exceptions
{
    public class InvalidHostException : Exception
    {
        private readonly string _host;

        public InvalidHostException(string host)
        {
            _host = host;
        }

        public override string ToString()
        {
            return string.Format("InvalidHostException: Host {0} is invalid.", _host);
        }
    }
}
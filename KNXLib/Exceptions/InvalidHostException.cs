using System;

namespace KNXLib.Exceptions
{
    public class InvalidHostException : System.Exception
    {
        private string host;

        public InvalidHostException(string host)
        {
            this.host = host;
        }

        public override string ToString()
        {
            return "InvalidHostException: Host " + host + " is invalid.";
        }
    }
}
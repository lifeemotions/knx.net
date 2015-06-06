using System;

namespace KNXLib.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to connect to an invalid host
    /// </summary>
    public class InvalidHostException : Exception
    {
        private readonly string _host;

        /// <summary>
        /// Initializes a new instance of the InvalidHostException class
        /// </summary>
        /// <param name="host"></param>
        public InvalidHostException(string host)
        {
            _host = host;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        public override string ToString()
        {
            return string.Format("InvalidHostException: Host {0} is invalid.", _host);
        }
    }
}
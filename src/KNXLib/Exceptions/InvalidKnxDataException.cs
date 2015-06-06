using System;

namespace KNXLib.Exceptions
{
    /// <summary>
    /// Exception is thrown when invalid data has been provided to an action method
    /// </summary>
    public class InvalidKnxDataException : Exception
    {
        private readonly string _data;

        /// <summary>
        /// Initializes a new instance of the InvalidKnxDataException class.
        /// </summary>
        /// <param name="data"></param>
        public InvalidKnxDataException(string data)
        {
            _data = data;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        public override string ToString()
        {
            return string.Format("InvalidKnxDataException: Data {0} is invalid.", _data);
        }
    }
}

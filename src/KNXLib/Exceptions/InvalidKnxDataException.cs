using System;

namespace KNXLib.Exceptions
{
    public class InvalidKnxDataException : Exception
    {
        private readonly string _data;

        public InvalidKnxDataException(string data)
        {
            _data = data;
        }

        public override string ToString()
        {
            return string.Format("InvalidKnxDataException: Data {0} is invalid.", _data);
        }
    }
}

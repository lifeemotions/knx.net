using System;

namespace KNXLib.Exceptions
{
    public class InvalidKnxAddressException : Exception
    {
        private readonly string _address;

        public InvalidKnxAddressException(string address)
        {
            _address = address;
        }

        public override string ToString()
        {
            return string.Format("InvalidKnxAddressException: Address {0} is invalid.", _address);
        }
    }
}

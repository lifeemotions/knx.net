using System;

namespace KNXLib.Exceptions
{
    public class InvalidKNXAddressException : System.Exception
    {
        private string address;

        public InvalidKNXAddressException(string address)
        {
            this.address = address;
        }

        public override string ToString()
        {
            return "InvalidKNXAddressException: Address " + address + " is invalid.";
        }
    }
}
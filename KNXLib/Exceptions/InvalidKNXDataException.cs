using System;

namespace KNXLib.Exceptions
{
    public class InvalidKNXDataException : System.Exception
    {
        private string data;

        public InvalidKNXDataException(string data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return "InvalidKNXDataException: Data " + data + " is invalid.";
        }
    }
}
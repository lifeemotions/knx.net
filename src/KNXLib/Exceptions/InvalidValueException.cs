using System;

namespace KNXLib.Exceptions
{
    public class InvalidValueException : Exception
    {
        private readonly string _value;
        private readonly string _dpt;

        public InvalidValueException(string value, string dpt)
        {
            _value = value;
            _dpt = dpt;
        }

        public override string ToString()
        {
            return string.Format("InvalidValueException: Value {0} is invalid for datapoint type {1}.", _value, _dpt);
        }
    }
}
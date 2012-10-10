using System;

namespace KNXLib.Exceptions
{
    public class MultipleLocalInterfacesException : System.Exception
    {
        public MultipleLocalInterfacesException()
        {
        }

        public override string ToString()
        {
            return "MultipleLocalInterfacesException: With IP Tunneling you can only have a single network interface or you must provide the local interface addres you are using to connect!";
        }
    }
}
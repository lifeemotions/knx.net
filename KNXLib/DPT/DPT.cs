using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNXLib.DPT
{
    public abstract class DPT
    {
        public abstract string ID
        {
            get;
        }
        public abstract object fromDPT(byte[] data);

        public abstract object fromDPT(String data);

        public abstract byte[] toDPT(object val);

        public abstract byte[] toDPT(String value);
    }
}

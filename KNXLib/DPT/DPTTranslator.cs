using System;
using System.Collections.Generic;

namespace KNXLib.DPT
{
    internal sealed class DPTTranslator
    {
        private static readonly DPTTranslator instance = new DPTTranslator();
        private readonly IDictionary<string, DPT> _dpts = new Dictionary<string, DPT>();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DPTTranslator()
        {
        }

        private DPTTranslator()
        {
            // TODO: Should we provide an extension point for users to add their own DPTs?
            DPT dpt = new DPTTemperature();
            _dpts.Add(dpt.Id, dpt);
        }

        public static DPTTranslator Instance
        {
            get { return instance; }
        }

        public object FromDPT(string type, string data)
        {
            try
            {
                DPT dpt;
                if (_dpts.TryGetValue(type, out dpt))
                    return dpt.FromDPT(data);
            }
            catch (Exception) // TODO: Not clear why you would want to supress all exceptions?
            {
            }

            return null;
        }

        public object FromDPT(string type, byte[] data)
        {
            try
            {
                DPT dpt;
                if (_dpts.TryGetValue(type, out dpt))
                    return dpt.FromDPT(data);
            }
            catch (Exception) // TODO: Not clear why you would want to supress all exceptions?
            {
            }

            return null;
        }

        public byte[] ToDPT(string type, string value)
        {
            try
            {
                DPT dpt;
                if (_dpts.TryGetValue(type, out dpt))
                    return dpt.ToDPT(value);
            }
            catch (Exception) // TODO: Not clear why you would want to supress all exceptions?
            {
            }

            return null;
        }

        public byte[] ToDPT(string type, object value)
        {
            try
            {
                DPT dpt;
                if (_dpts.TryGetValue(type, out dpt))
                    return dpt.ToDPT(value);
            }
            catch (Exception) // TODO: Not clear why you would want to supress all exceptions?
            {
            }

            return null;
        }
    }
}

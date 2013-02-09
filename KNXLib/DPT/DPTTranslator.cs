using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNXLib.DPT
{
    public class DPTTranslator
    {
        #region Singleton
        private static readonly DPTTranslator instance = new DPTTranslator();

        private DPTTranslator()
        {
            this.Initialize();
        }

        public static DPTTranslator Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        private IDictionary<string, DPT> dpts = new Dictionary<string, DPT>();

        private void Initialize()
        {
            DPT dpt;

            dpt = new DPTTemperature();
            dpts.Add(dpt.ID, dpt);
        }

        public object fromDPT(string type, byte[] data)
        {
            try
            {
                if (dpts.ContainsKey(type))
                {
                    DPT dpt = dpts[type];
                    return dpt.fromDPT(data);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public object fromDPT(string type, String data)
        {
            try
            {
                if (dpts.ContainsKey(type))
                {
                    DPT dpt = dpts[type];
                    return dpt.fromDPT(data);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public byte[] toDPT(string type, object value)
        {
            try
            {
                if (dpts.ContainsKey(type))
                {
                    DPT dpt = dpts[type];
                    return dpt.toDPT(value);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public byte[] toDPT(string type, String value)
        {
            try
            {
                if (dpts.ContainsKey(type))
                {
                    DPT dpt = dpts[type];
                    return dpt.toDPT(value);
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
    }
}

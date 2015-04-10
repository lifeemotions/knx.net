using System.Collections.Generic;

namespace KNXLib.DPT
{
    internal sealed class DataPointTranslator
    {
        private static readonly DataPointTranslator instance = new DataPointTranslator();
        private readonly IDictionary<string, DataPoint> _dataPoints = new Dictionary<string, DataPoint>();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DataPointTranslator()
        {
        }

        private DataPointTranslator()
        {
            // TODO: Should we provide an extension point for users to add their own DPTs?
            DataPoint dataPoint = new Temperature();
            _dataPoints.Add(dataPoint.Id, dataPoint);
        }

        public static DataPointTranslator Instance
        {
            get { return instance; }
        }

        public object FromDataPoint(string type, string data)
        {
            try
            {
                DataPoint dpt;
                if (_dataPoints.TryGetValue(type, out dpt))
                    return dpt.FromDataPoint(data);
            }
            catch
            {
            }

            return null;
        }

        public object FromDataPoint(string type, byte[] data)
        {
            try
            {
                DataPoint dpt;
                if (_dataPoints.TryGetValue(type, out dpt))
                    return dpt.FromDataPoint(data);
            }
            catch
            {
            }

            return null;
        }

        public byte[] ToDataPoint(string type, string value)
        {
            try
            {
                DataPoint dpt;
                if (_dataPoints.TryGetValue(type, out dpt))
                    return dpt.ToDataPoint(value);
            }
            catch
            {
            }

            return null;
        }

        public byte[] ToDataPoint(string type, object value)
        {
            try
            {
                DataPoint dpt;
                if (_dataPoints.TryGetValue(type, out dpt))
                    return dpt.ToDataPoint(value);
            }
            catch
            {
            }

            return null;
        }
    }
}

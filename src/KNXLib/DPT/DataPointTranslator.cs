namespace KNXLib.DPT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class DataPointTranslator
    {
        public static readonly DataPointTranslator Instance = new DataPointTranslator();
        private readonly IDictionary<string, DataPoint> _dataPoints = new Dictionary<string, DataPoint>();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DataPointTranslator()
        {
        }

        private DataPointTranslator()
        {
            var type = typeof(DataPoint);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p != type);

            foreach (var t in types)
            {
                var dp = (DataPoint) Activator.CreateInstance(t);

                foreach (var id in dp.Ids)
                    _dataPoints.Add(id, dp);
            }
        }

        public object FromDataPoint(string type, string data)
        {
            try
            {
                if (_dataPoints.TryGetValue(type, out DataPoint dpt))
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
                if (_dataPoints.TryGetValue(type, out DataPoint dpt))
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
                if (_dataPoints.TryGetValue(type, out DataPoint dpt))
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
                if (_dataPoints.TryGetValue(type, out DataPoint dpt))
                    return dpt.ToDataPoint(value);
            }
            catch
            {
            }

            return null;
        }
    }
}

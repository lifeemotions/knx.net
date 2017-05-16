using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint8BitNoSignScaledScaling
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Test]
        public void DataPoint8BitNoSignScaledScalingTest()
        {
            string dptType = "5.001";

            int scale0 = 0;
            byte[] scale0Bytes = { 0x00, 0x00 };
            int scale20 = 20;
            byte[] scale20Bytes = { 0x00, 0x33 };
            int scale60 = 60;
            byte[] scale60Bytes = { 0x00, 0x99 };
            int scale80 = 80;
            byte[] scale80Bytes = { 0x00, 0xCC };
            int scale100 = 100;
            byte[] scale100Bytes = { 0x00, 0xFF };

            Assert.AreEqual(scale0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale0Bytes)));
            Assert.AreEqual(scale20, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale20Bytes)));
            Assert.AreEqual(scale60, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale60Bytes)));
            Assert.AreEqual(scale80, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale80Bytes)));
            Assert.AreEqual(scale100,
                ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale100Bytes)));

            Assert.AreEqual(scale0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale0));
            Assert.AreEqual(scale20Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale20));
            Assert.AreEqual(scale60Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale60));
            Assert.AreEqual(scale80Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale80));
            Assert.AreEqual(scale100Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale100));
        }
    }
}

using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint8BitNoSignScaledAngle
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Test]
        public void DataPoint8BitNoSignScaledAngleTest()
        {
            var dptType = "5.003";

            var angle0 = 0;
            var angle0Bytes = new byte[] { 0x00 };
            var angle72 = 72;
            var angle72Bytes = new byte[] { 0x33 };
            var angle120 = 120;
            var angle120Bytes = new byte[] { 0x55 };
            var angle288 = 288;
            var angle288Bytes = new byte[] { 0xCC };
            var angle360 = 360;
            var angle360Bytes = new byte[] { 0xFF };

            Assert.AreEqual(angle0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle0Bytes)));
            Assert.AreEqual(angle72, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle72Bytes)));
            Assert.AreEqual(angle120, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle120Bytes)));
            Assert.AreEqual(angle288, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle288Bytes)));
            Assert.AreEqual(angle360, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle360Bytes)));

            Assert.AreEqual(angle0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle0));
            Assert.AreEqual(angle72Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle72));
            Assert.AreEqual(angle120Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle120));
            Assert.AreEqual(angle288Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle288));
            Assert.AreEqual(angle360Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle360));
        }
    }
}
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
            string dptType = "5.003";

            int angle0 = 0;
            byte[] angle0Bytes = {0x00};
            int angle72 = 72;
            byte[] angle72Bytes = {0x33};
            int angle120 = 120;
            byte[] angle120Bytes = {0x55};
            int angle288 = 288;
            byte[] angle288Bytes = {0xCC};
            int angle360 = 360;
            byte[] angle360Bytes = {0xFF};

            Assert.AreEqual(angle0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle0Bytes)));
            Assert.AreEqual(angle72, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle72Bytes)));
            Assert.AreEqual(angle120,
                ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle120Bytes)));
            Assert.AreEqual(angle288,
                ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle288Bytes)));
            Assert.AreEqual(angle360,
                ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle360Bytes)));

            Assert.AreEqual(angle0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle0));
            Assert.AreEqual(angle72Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle72));
            Assert.AreEqual(angle120Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle120));
            Assert.AreEqual(angle288Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle288));
            Assert.AreEqual(angle360Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle360));
        }
    }
}

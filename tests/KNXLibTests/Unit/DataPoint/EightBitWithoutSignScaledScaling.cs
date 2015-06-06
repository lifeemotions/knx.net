using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class EightBitWithoutSignScaledScaling
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        [Category("KNXLib.Unit.DataPoint.5.xxx"), Test]
        public void DataPointScaledScalingTest()
        {
            var dptType = "5.001";

            var scale0 = 0;
            var scale0Bytes = new byte[] { 0x00 };
            var scale20 = 20;
            var scale20Bytes = new byte[] { 0x33 };
            var scale60 = 60;
            var scale60Bytes = new byte[] { 0x99 };
            var scale80 = 80;
            var scale80Bytes = new byte[] { 0xCC };
            var scale100 = 100;
            var scale100Bytes = new byte[] { 0xFF };

            Assert.AreEqual(scale0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale0Bytes)));
            Assert.AreEqual(scale20, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale20Bytes)));
            Assert.AreEqual(scale60, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale60Bytes)));
            Assert.AreEqual(scale80, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale80Bytes)));
            Assert.AreEqual(scale100, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale100Bytes)));

            Assert.AreEqual(scale0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale0));
            Assert.AreEqual(scale20Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale20));
            Assert.AreEqual(scale60Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale60));
            Assert.AreEqual(scale80Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale80));
            Assert.AreEqual(scale100Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale100));
        }
    }
}
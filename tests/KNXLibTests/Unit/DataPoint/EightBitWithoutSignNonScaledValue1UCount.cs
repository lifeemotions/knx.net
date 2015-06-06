using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class EightBitWithoutSignNonScaledValue1UCount
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
        public void DataPointNonScaledValue1UCountTest()
        {
            var dptType = "5.010";

            var count0 = 0;
            var count0Bytes = new byte[] { 0x00 };
            var count97 = 97;
            var count97Bytes = new byte[] { 0x61 };
            var count128 = 128;
            var count128Bytes = new byte[] { 0x80 };
            var count199 = 199;
            var count199Bytes = new byte[] { 0xC7 };
            var count255 = 255;
            var count255Bytes = new byte[] { 0xFF };

            Assert.AreEqual(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.AreEqual(count97, DataPointTranslator.Instance.FromDataPoint(dptType, count97Bytes));
            Assert.AreEqual(count128, DataPointTranslator.Instance.FromDataPoint(dptType, count128Bytes));
            Assert.AreEqual(count199, DataPointTranslator.Instance.FromDataPoint(dptType, count199Bytes));
            Assert.AreEqual(count255, DataPointTranslator.Instance.FromDataPoint(dptType, count255Bytes));
            
            Assert.AreEqual(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.AreEqual(count97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count97));
            Assert.AreEqual(count128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128));
            Assert.AreEqual(count199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count199));
            Assert.AreEqual(count255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count255));
        }
    }
}
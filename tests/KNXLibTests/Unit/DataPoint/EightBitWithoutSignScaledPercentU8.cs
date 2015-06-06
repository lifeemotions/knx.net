using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class EightBitWithoutSignScaledPercentU8
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
        public void DataPointScaledPercentU8Test()
        {
            var dptType = "5.004";

            var perc0 = 0;
            var perc0Bytes = new byte[] { 0x00 };
            var perc97 = 97;
            var perc97Bytes = new byte[] { 0x61 };
            var perc128 = 128;
            var perc128Bytes = new byte[] { 0x80 };
            var perc199 = 199;
            var perc199Bytes = new byte[] { 0xC7 };
            var perc255 = 255;
            var perc255Bytes = new byte[] { 0xFF };

            Assert.AreEqual(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.AreEqual(perc97, DataPointTranslator.Instance.FromDataPoint(dptType, perc97Bytes));
            Assert.AreEqual(perc128, DataPointTranslator.Instance.FromDataPoint(dptType, perc128Bytes));
            Assert.AreEqual(perc199, DataPointTranslator.Instance.FromDataPoint(dptType, perc199Bytes));
            Assert.AreEqual(perc255, DataPointTranslator.Instance.FromDataPoint(dptType, perc255Bytes));
            
            Assert.AreEqual(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.AreEqual(perc97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc97));
            Assert.AreEqual(perc128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128));
            Assert.AreEqual(perc199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc199));
            Assert.AreEqual(perc255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc255));
        }
    }
}
using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class EightBitWithSignRelativeValue
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        [Category("KNXLib.Unit.DataPoint.6.xxx"), Test]
        public void DataPointRelativeValuePercent()
        {
            var dptType = "6.001";
            
            var perc128N = -128;
            var perc128NBytes = new byte[] { 0x80 };
            var perc1N = -1;
            var perc1NBytes = new byte[] { 0xFF };
            var perc0 = 0;
            var perc0Bytes = new byte[] { 0x00 };
            var perc55 = 55;
            var perc55Bytes = new byte[] { 0x37 };
            var perc127 = 127;
            var perc127Bytes = new byte[] { 0x7F };
            
            Assert.AreEqual(perc128N, DataPointTranslator.Instance.FromDataPoint(dptType, perc128NBytes));
            Assert.AreEqual(perc1N, DataPointTranslator.Instance.FromDataPoint(dptType, perc1NBytes));
            Assert.AreEqual(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.AreEqual(perc55, DataPointTranslator.Instance.FromDataPoint(dptType, perc55Bytes));
            Assert.AreEqual(perc127, DataPointTranslator.Instance.FromDataPoint(dptType, perc127Bytes));
            
            Assert.AreEqual(perc128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128N));
            Assert.AreEqual(perc1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc1N));
            Assert.AreEqual(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.AreEqual(perc55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc55));
            Assert.AreEqual(perc127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc127));
        }
    }
}
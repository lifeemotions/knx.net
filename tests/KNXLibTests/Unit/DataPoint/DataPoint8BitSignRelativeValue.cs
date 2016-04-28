using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint8BitSignRelativeValue
    {
        [Category("KNXLib.Unit.DataPoint.8BitSign"), Test]
        public void DataPoint8BitSignRelativeValuePercentTest()
        {
            string dptType = "6.001";

            int perc128N = -128;
            byte[] perc128NBytes = {0x80};
            int perc1N = -1;
            byte[] perc1NBytes = {0xFF};
            int perc0 = 0;
            byte[] perc0Bytes = {0x00};
            int perc55 = 55;
            byte[] perc55Bytes = {0x37};
            int perc127 = 127;
            byte[] perc127Bytes = {0x7F};

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

        [Category("KNXLib.Unit.DataPoint.8BitSign"), Test]
        public void DataPoint8BitSignRelativeValueCountTest()
        {
            string dptType = "6.010";

            int count128N = -128;
            byte[] count128NBytes = {0x80};
            int count1N = -1;
            byte[] count1NBytes = {0xFF};
            int count0 = 0;
            byte[] count0Bytes = {0x00};
            int count55 = 55;
            byte[] count55Bytes = {0x37};
            int count127 = 127;
            byte[] count127Bytes = {0x7F};

            Assert.AreEqual(count128N, DataPointTranslator.Instance.FromDataPoint(dptType, count128NBytes));
            Assert.AreEqual(count1N, DataPointTranslator.Instance.FromDataPoint(dptType, count1NBytes));
            Assert.AreEqual(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.AreEqual(count55, DataPointTranslator.Instance.FromDataPoint(dptType, count55Bytes));
            Assert.AreEqual(count127, DataPointTranslator.Instance.FromDataPoint(dptType, count127Bytes));

            Assert.AreEqual(count128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128N));
            Assert.AreEqual(count1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count1N));
            Assert.AreEqual(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.AreEqual(count55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count55));
            Assert.AreEqual(count127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count127));
        }
    }
}

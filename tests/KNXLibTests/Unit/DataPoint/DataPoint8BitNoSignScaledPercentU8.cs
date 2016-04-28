using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint8BitNoSignScaledPercentU8
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Test]
        public void DataPoint8BitNoSignScaledPercentU8Test()
        {
            string dptType = "5.004";

            int perc0 = 0;
            byte[] perc0Bytes = {0x00};
            int perc97 = 97;
            byte[] perc97Bytes = {0x61};
            int perc128 = 128;
            byte[] perc128Bytes = {0x80};
            int perc199 = 199;
            byte[] perc199Bytes = {0xC7};
            int perc255 = 255;
            byte[] perc255Bytes = {0xFF};

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

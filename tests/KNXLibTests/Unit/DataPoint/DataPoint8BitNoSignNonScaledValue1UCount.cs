using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint8BitNoSignNonScaledValue1UCount
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Test]
        public void DataPoint8BitNoSignNonScaledValue1UCountTest()
        {
            string dptType = "5.010";

            int count0 = 0;
            byte[] count0Bytes = {0x00};
            int count97 = 97;
            byte[] count97Bytes = {0x61};
            int count128 = 128;
            byte[] count128Bytes = {0x80};
            int count199 = 199;
            byte[] count199Bytes = {0xC7};
            int count255 = 255;
            byte[] count255Bytes = {0xFF};

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

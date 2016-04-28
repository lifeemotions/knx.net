using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint3BitControl
    {
        [Category("KNXLib.Unit.DataPoint.3BitControl"), Test]
        public void DataPoint3BitControlDimmingTest()
        {
            string dptType = "3.007";

            int incr4 = 4;
            byte[] incr4Bytes = {0x0C};
            int incr1 = 1;
            byte[] incr1Bytes = {0x0F};
            int stop = 0;
            byte[] stopBytes = {0x00};
            int decr3 = -3;
            byte[] decr3Bytes = {0x05};
            int decr7 = -7;
            byte[] decr7Bytes = {0x01};

            Assert.AreEqual(incr4, DataPointTranslator.Instance.FromDataPoint(dptType, incr4Bytes));
            Assert.AreEqual(incr1, DataPointTranslator.Instance.FromDataPoint(dptType, incr1Bytes));
            Assert.AreEqual(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.AreEqual(decr3, DataPointTranslator.Instance.FromDataPoint(dptType, decr3Bytes));
            Assert.AreEqual(decr7, DataPointTranslator.Instance.FromDataPoint(dptType, decr7Bytes));

            Assert.AreEqual(incr4Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr4));
            Assert.AreEqual(incr1Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr1));
            Assert.AreEqual(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.AreEqual(decr3Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr3));
            Assert.AreEqual(decr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr7));
        }

        [Category("KNXLib.Unit.DataPoint.3BitControl"), Test]
        public void DataPoint3BitControlBlindsTest()
        {
            string dptType = "3.008";

            int incr7 = 7;
            byte[] incr7Bytes = {0x09};
            int incr2 = 2;
            byte[] incr2Bytes = {0x0E};
            int stop = 0;
            byte[] stopBytes = {0x00};
            int decr5 = -5;
            byte[] decr5Bytes = {0x03};
            int decr6 = -6;
            byte[] decr6Bytes = {0x02};

            Assert.AreEqual(incr7, DataPointTranslator.Instance.FromDataPoint(dptType, incr7Bytes));
            Assert.AreEqual(incr2, DataPointTranslator.Instance.FromDataPoint(dptType, incr2Bytes));
            Assert.AreEqual(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.AreEqual(decr5, DataPointTranslator.Instance.FromDataPoint(dptType, decr5Bytes));
            Assert.AreEqual(decr6, DataPointTranslator.Instance.FromDataPoint(dptType, decr6Bytes));

            Assert.AreEqual(incr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr7));
            Assert.AreEqual(incr2Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr2));
            Assert.AreEqual(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.AreEqual(decr5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr5));
            Assert.AreEqual(decr6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr6));
        }
    }
}

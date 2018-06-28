using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint3BitControl
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.3BitControl")]
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

            Assert.Equal(incr4, DataPointTranslator.Instance.FromDataPoint(dptType, incr4Bytes));
            Assert.Equal(incr1, DataPointTranslator.Instance.FromDataPoint(dptType, incr1Bytes));
            Assert.Equal(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.Equal(decr3, DataPointTranslator.Instance.FromDataPoint(dptType, decr3Bytes));
            Assert.Equal(decr7, DataPointTranslator.Instance.FromDataPoint(dptType, decr7Bytes));

            Assert.Equal(incr4Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr4));
            Assert.Equal(incr1Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr1));
            Assert.Equal(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.Equal(decr3Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr3));
            Assert.Equal(decr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr7));
        }

        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.3BitControl")]
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

            Assert.Equal(incr7, DataPointTranslator.Instance.FromDataPoint(dptType, incr7Bytes));
            Assert.Equal(incr2, DataPointTranslator.Instance.FromDataPoint(dptType, incr2Bytes));
            Assert.Equal(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.Equal(decr5, DataPointTranslator.Instance.FromDataPoint(dptType, decr5Bytes));
            Assert.Equal(decr6, DataPointTranslator.Instance.FromDataPoint(dptType, decr6Bytes));

            Assert.Equal(incr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr7));
            Assert.Equal(incr2Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr2));
            Assert.Equal(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.Equal(decr5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr5));
            Assert.Equal(decr6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr6));
        }
    }
}

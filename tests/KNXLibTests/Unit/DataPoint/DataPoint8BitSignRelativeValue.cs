using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint8BitSignRelativeValue
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.8BitSign")]
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

            Assert.Equal(perc128N, DataPointTranslator.Instance.FromDataPoint(dptType, perc128NBytes));
            Assert.Equal(perc1N, DataPointTranslator.Instance.FromDataPoint(dptType, perc1NBytes));
            Assert.Equal(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.Equal(perc55, DataPointTranslator.Instance.FromDataPoint(dptType, perc55Bytes));
            Assert.Equal(perc127, DataPointTranslator.Instance.FromDataPoint(dptType, perc127Bytes));

            Assert.Equal(perc128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128N));
            Assert.Equal(perc1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc1N));
            Assert.Equal(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.Equal(perc55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc55));
            Assert.Equal(perc127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc127));
        }

        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.8BitSign")]
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

            Assert.Equal(count128N, DataPointTranslator.Instance.FromDataPoint(dptType, count128NBytes));
            Assert.Equal(count1N, DataPointTranslator.Instance.FromDataPoint(dptType, count1NBytes));
            Assert.Equal(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.Equal(count55, DataPointTranslator.Instance.FromDataPoint(dptType, count55Bytes));
            Assert.Equal(count127, DataPointTranslator.Instance.FromDataPoint(dptType, count127Bytes));

            Assert.Equal(count128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128N));
            Assert.Equal(count1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count1N));
            Assert.Equal(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.Equal(count55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count55));
            Assert.Equal(count127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count127));
        }
    }
}

using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint8BitNoSignNonScaledValue1UCount
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.8BitNoSign")]
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

            Assert.Equal(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.Equal(count97, DataPointTranslator.Instance.FromDataPoint(dptType, count97Bytes));
            Assert.Equal(count128, DataPointTranslator.Instance.FromDataPoint(dptType, count128Bytes));
            Assert.Equal(count199, DataPointTranslator.Instance.FromDataPoint(dptType, count199Bytes));
            Assert.Equal(count255, DataPointTranslator.Instance.FromDataPoint(dptType, count255Bytes));

            Assert.Equal(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.Equal(count97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count97));
            Assert.Equal(count128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128));
            Assert.Equal(count199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count199));
            Assert.Equal(count255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count255));
        }
    }
}

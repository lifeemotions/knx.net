using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint8BitNoSignScaledPercentU8
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.8BitNoSign")]
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

            Assert.Equal(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.Equal(perc97, DataPointTranslator.Instance.FromDataPoint(dptType, perc97Bytes));
            Assert.Equal(perc128, DataPointTranslator.Instance.FromDataPoint(dptType, perc128Bytes));
            Assert.Equal(perc199, DataPointTranslator.Instance.FromDataPoint(dptType, perc199Bytes));
            Assert.Equal(perc255, DataPointTranslator.Instance.FromDataPoint(dptType, perc255Bytes));

            Assert.Equal(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.Equal(perc97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc97));
            Assert.Equal(perc128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128));
            Assert.Equal(perc199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc199));
            Assert.Equal(perc255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc255));
        }
    }
}

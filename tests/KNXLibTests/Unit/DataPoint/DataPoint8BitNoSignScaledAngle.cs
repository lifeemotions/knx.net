using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint8BitNoSignScaledAngle
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.8BitNoSign")]
        public void DataPoint8BitNoSignScaledAngleTest()
        {
            string dptType = "5.003";

            int angle0 = 0;
            byte[] angle0Bytes = {0x00};
            int angle72 = 72;
            byte[] angle72Bytes = {0x33};
            int angle120 = 120;
            byte[] angle120Bytes = {0x55};
            int angle288 = 288;
            byte[] angle288Bytes = {0xCC};
            int angle360 = 360;
            byte[] angle360Bytes = {0xFF};

            Assert.Equal(angle0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle0Bytes)));
            Assert.Equal(angle72, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle72Bytes)));
            Assert.Equal(angle120, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle120Bytes)));
            Assert.Equal(angle288, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle288Bytes)));
            Assert.Equal(angle360, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, angle360Bytes)));

            Assert.Equal(angle0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle0));
            Assert.Equal(angle72Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle72));
            Assert.Equal(angle120Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle120));
            Assert.Equal(angle288Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle288));
            Assert.Equal(angle360Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, angle360));
        }
    }
}

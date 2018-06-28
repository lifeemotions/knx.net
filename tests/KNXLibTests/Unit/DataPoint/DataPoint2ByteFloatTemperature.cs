using KNXLib.DPT;
using Xunit;

namespace KNXLibTests.Unit.DataPoint
{
    public class DataPoint2ByteFloatTemperature
    {
        [Fact]
        [Trait("Category","KNXLib.Unit.DataPoint.2ByteFloat")]
        public void DataPoint2ByteFloatTemperatureTest()
        {
            string dptType = "9.001";

            decimal temp30N = -30m;
            byte[] temp30NBytes = {0x8A, 0x24};
            decimal temp23Dec = 23.0m;
            byte[] temp23Bytes = {0x0C, 0x7E};
            decimal temp19Dec = 19.5m;
            byte[] temp19Bytes = {0x07, 0x9E};
            decimal temp5Dec = 5.8m;
            byte[] temp5Bytes = {0x02, 0x44};
            decimal tempMinus6Dec = -6.5m;
            byte[] tempMinus6Bytes = {0x85, 0x76};
            decimal temp36Dec = 36.7m;
            byte[] temp36Bytes = {0x0F, 0x2B};
            decimal temp0Dec = 0m;
            byte[] temp0Bytes = {0x00, 0x00};

            Assert.Equal(temp30N, DataPointTranslator.Instance.FromDataPoint(dptType, temp30NBytes));
            Assert.Equal(temp23Dec, DataPointTranslator.Instance.FromDataPoint(dptType, temp23Bytes));
            Assert.Equal(temp19Dec, DataPointTranslator.Instance.FromDataPoint(dptType, temp19Bytes));
            Assert.Equal(temp5Dec, DataPointTranslator.Instance.FromDataPoint(dptType, temp5Bytes));
            Assert.Equal(tempMinus6Dec, DataPointTranslator.Instance.FromDataPoint(dptType, tempMinus6Bytes));
            Assert.Equal(temp36Dec, DataPointTranslator.Instance.FromDataPoint(dptType, temp36Bytes));
            Assert.Equal(temp0Dec, DataPointTranslator.Instance.FromDataPoint(dptType, temp0Bytes));

            temp30NBytes = new byte[] {00, temp30NBytes[0], temp30NBytes[1]};
            temp23Bytes = new byte[] {00, temp23Bytes[0], temp23Bytes[1]};
            temp19Bytes = new byte[] {00, temp19Bytes[0], temp19Bytes[1]};
            temp5Bytes = new byte[] {00, temp5Bytes[0], temp5Bytes[1]};
            tempMinus6Bytes = new byte[] {00, tempMinus6Bytes[0], tempMinus6Bytes[1]};
            temp36Bytes = new byte[] {00, temp36Bytes[0], temp36Bytes[1]};
            temp0Bytes = new byte[] {00, temp0Bytes[0], temp0Bytes[1]};

            Assert.Equal(temp30NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp30N));
            Assert.Equal(temp23Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp23Dec));
            Assert.Equal(temp19Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp19Dec));
            Assert.Equal(temp5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp5Dec));
            Assert.Equal(tempMinus6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, tempMinus6Dec));
            Assert.Equal(temp36Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp36Dec));
            Assert.Equal(temp0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp0Dec));
        }
    }
}

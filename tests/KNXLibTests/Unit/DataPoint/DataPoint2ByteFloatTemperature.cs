using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class DataPoint2ByteFloatTemperature
    {
        [Category("KNXLib.Unit.DataPoint.2ByteFloat"), Test]
        public void DataPoint2ByteFloatTemperatureTest()
        {
            string dptType = "9.001";

            float temp30N = -30f;
            byte[] temp30NBytes = {0x8A, 0x24};
            float temp23Float = 23.0f;
            byte[] temp23Bytes = {0x0C, 0x7E};
            float temp19Float = 19.5f;
            byte[] temp19Bytes = {0x07, 0x9E};
            float temp5Float = 5.8f;
            byte[] temp5Bytes = {0x02, 0x44};
            float tempMinus6Float = -6.5f;
            byte[] tempMinus6Bytes = {0x85, 0x76};
            float temp36Float = 36.7f;
            byte[] temp36Bytes = {0x0F, 0x2B};
            float temp0Float = 0f;
            byte[] temp0Bytes = {0x00, 0x00};

            Assert.AreEqual(temp30N, DataPointTranslator.Instance.FromDataPoint(dptType, temp30NBytes));
            Assert.AreEqual(temp23Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp23Bytes));
            Assert.AreEqual(temp19Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp19Bytes));
            Assert.AreEqual(temp5Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp5Bytes));
            Assert.AreEqual(tempMinus6Float, DataPointTranslator.Instance.FromDataPoint(dptType, tempMinus6Bytes));
            Assert.AreEqual(temp36Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp36Bytes));
            Assert.AreEqual(temp0Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp0Bytes));

            temp30NBytes = new byte[] {00, temp30NBytes[0], temp30NBytes[1]};
            temp23Bytes = new byte[] {00, temp23Bytes[0], temp23Bytes[1]};
            temp19Bytes = new byte[] {00, temp19Bytes[0], temp19Bytes[1]};
            temp5Bytes = new byte[] {00, temp5Bytes[0], temp5Bytes[1]};
            tempMinus6Bytes = new byte[] {00, tempMinus6Bytes[0], tempMinus6Bytes[1]};
            temp36Bytes = new byte[] {00, temp36Bytes[0], temp36Bytes[1]};
            temp0Bytes = new byte[] {00, temp0Bytes[0], temp0Bytes[1]};

            Assert.AreEqual(temp30NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp30N));
            Assert.AreEqual(temp23Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp23Float));
            Assert.AreEqual(temp19Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp19Float));
            Assert.AreEqual(temp5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp5Float));
            Assert.AreEqual(tempMinus6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, tempMinus6Float));
            Assert.AreEqual(temp36Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp36Float));
            Assert.AreEqual(temp0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp0Float));
        }
    }
}

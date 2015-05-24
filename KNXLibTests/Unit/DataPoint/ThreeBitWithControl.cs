using KNXLib.DPT;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class ThreeBitWithControl
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        [Category("KNXLib.Unit.DataPoint.ControlDimming"), Test]
        public void DataPointControlDimmingTest()
        {
            var dptType = "3.007";

            var incr4 = 4;
            var incr4Bytes = new byte[] { 12 };
            var incr1 = 1;
            var incr1Bytes = new byte[] { 9 };
            var stop = 0;
            var stopBytes = new byte[] { 0 };
            var decr3 = -3;
            var decr3Bytes = new byte[] { 3 };
            var decr7 = -7;
            var decr7Bytes = new byte[] { 7 };

            Assert.AreEqual(incr4, DataPointTranslator.Instance.FromDataPoint(dptType, incr4Bytes));
            Assert.AreEqual(incr1, DataPointTranslator.Instance.FromDataPoint(dptType, incr1Bytes));
            Assert.AreEqual(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.AreEqual(decr3, DataPointTranslator.Instance.FromDataPoint(dptType, decr3Bytes));
            Assert.AreEqual(decr7, DataPointTranslator.Instance.FromDataPoint(dptType, decr7Bytes));

            incr4Bytes = new[] { incr4Bytes[0] };
            incr1Bytes = new[] { incr1Bytes[0] };
            stopBytes = new[] { stopBytes[0] };
            decr3Bytes = new[] { decr3Bytes[0] };
            decr7Bytes = new[] { decr7Bytes[0] };

            Assert.AreEqual(incr4Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr4));
            Assert.AreEqual(incr1Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr1));
            Assert.AreEqual(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.AreEqual(decr3Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr3));
            Assert.AreEqual(decr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr7));
        }

        [Category("KNXLib.Unit.DataPoint.ControlBlinds"), Test]
        public void DataPointControlBlindsTest()
        {
            var dptType = "3.008";

            var incr7 = 7;
            var incr7Bytes = new byte[] { 15 };
            var incr2 = 2;
            var incr2Bytes = new byte[] { 10 };
            var stop = 0;
            var stopBytes = new byte[] { 0 };
            var decr5 = -5;
            var decr5Bytes = new byte[] { 5 };
            var decr6 = -6;
            var decr6Bytes = new byte[] { 6 };

            Assert.AreEqual(incr7, DataPointTranslator.Instance.FromDataPoint(dptType, incr7Bytes));
            Assert.AreEqual(incr2, DataPointTranslator.Instance.FromDataPoint(dptType, incr2Bytes));
            Assert.AreEqual(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.AreEqual(decr5, DataPointTranslator.Instance.FromDataPoint(dptType, decr5Bytes));
            Assert.AreEqual(decr6, DataPointTranslator.Instance.FromDataPoint(dptType, decr6Bytes));

            incr7Bytes = new[] { incr7Bytes[0] };
            incr2Bytes = new[] { incr2Bytes[0] };
            stopBytes = new[] { stopBytes[0] };
            decr5Bytes = new[] { decr5Bytes[0] };
            decr6Bytes = new[] { decr6Bytes[0] };

            Assert.AreEqual(incr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr7));
            Assert.AreEqual(incr2Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr2));
            Assert.AreEqual(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.AreEqual(decr5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr5));
            Assert.AreEqual(decr6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr6));
        }
    }
}
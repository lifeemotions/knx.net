using NUnit.Framework;
using KNXLib.GA;
using KNXLib.Enums;

namespace KNXLibTests.Unit.GA
{
    [TestFixture]
    internal class KnxGroupAddressTest
    {
        [Category("KNXLib.Unit.GA"), Test]
        public void StringParserTestThreeLevel()
        {
            var ga = KnxGroupAddress.Parse("18/5/230");
            Assert.IsInstanceOf<KnxThreeLevelGroupAddress>(ga);

            KnxThreeLevelGroupAddress threeLevelGa = (KnxThreeLevelGroupAddress)ga;
            Assert.AreEqual(18, threeLevelGa.MainGroup);
            Assert.AreEqual(5, threeLevelGa.MiddleGroup);
            Assert.AreEqual(230, threeLevelGa.SubGroup);
        }

        [Category("KNXLib.Unit.GA"), Test]
        public void StringParserTestTwoLevel()
        {
            var ga = KnxGroupAddress.Parse("18/230");
            Assert.IsInstanceOf<KnxTwoLevelGroupAddress>(ga);

            KnxTwoLevelGroupAddress threeLevelGa = (KnxTwoLevelGroupAddress)ga;
            Assert.AreEqual(18, threeLevelGa.MainGroup);
            Assert.AreEqual(230, threeLevelGa.SubGroup);
        }

        [Category("KNXLib.Unit.GA"), Test]
        public void StringParserTestFreeStyle()
        {
            var ga = KnxGroupAddress.Parse("230");
            Assert.IsInstanceOf<KnxFreeStyleGroupAddress>(ga);

            KnxFreeStyleGroupAddress threeLevelGa = (KnxFreeStyleGroupAddress)ga;
            Assert.AreEqual(230, threeLevelGa.SubGroup);
        }

        [Category("KNXLib.Unit.GA"), Test]
        public void ByteParserTest()
        {
            var gaThreeLevel = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.ThreeLevel);
            var gaTwoLevel = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.TwoLevel);
            var gaFreeStyle = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.Free);

            Assert.IsInstanceOf<KnxThreeLevelGroupAddress>(gaThreeLevel);
            Assert.IsInstanceOf<KnxTwoLevelGroupAddress>(gaTwoLevel);
            Assert.IsInstanceOf<KnxFreeStyleGroupAddress>(gaFreeStyle);

            Assert.AreEqual("20/0/180", gaThreeLevel.ToString());
            Assert.AreEqual("20/180", gaTwoLevel.ToString());
            Assert.AreEqual("41140", gaFreeStyle.ToString());
        }


    }
}

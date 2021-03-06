using NUnit.Framework;
using KNXLib.Addressing;
using KNXLib.Exceptions;

namespace KNXLibTests.Unit.Addressing
{
    [TestFixture]
    internal class KnxThreeLevelGroupAddressTest
    {
        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void InvalidTest()
        {
            void Check(int MainGroup, int MiddleGroup, int SubGroup)
            {
                var ga = new KnxThreeLevelGroupAddress(MainGroup, MiddleGroup, SubGroup);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check(0, 0, 0);     // 0/0/0 is not allowed           
            Check(41, 2, 5);    // Main too high           
            Check(15, 8, 5);    // Middle too high
            Check(15, 2, 321);  // Sub too high
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void ValidTest()
        {
            void Check(int MainGroup, int MiddleGroup, int SubGroup)
            {
                var ga = new KnxThreeLevelGroupAddress(MainGroup, MiddleGroup, SubGroup);

                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(MainGroup, ga.MainGroup);
                Assert.AreEqual(MiddleGroup, ga.MiddleGroup);
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check(0, 0, 1);     // Min
            Check(31, 7, 255);  // Max
            Check(18, 5, 230);
            Check(21, 7, 255);
        }


        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void ValidParserTest()
        {
            void Check(string groupAddress, int MainGroup, int MiddleGroup, int SubGroup)
            {
                var ga = new KnxThreeLevelGroupAddress(groupAddress);
                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(MainGroup, ga.MainGroup);
                Assert.AreEqual(MiddleGroup, ga.MiddleGroup);
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check("0/0/1", 0, 0, 1);
            Check("31/7/255", 31, 7, 255);
            Check("18/5/230", 18, 5, 230);
            Check("21/7/255", 21, 7, 255);
        }


        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void InvalidParserTest()
        {
            void Check(string groupAddress)
            {
                var ga = new KnxThreeLevelGroupAddress(groupAddress);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check("0/0/0");
            Check("35/45/65");
            Check("5,6,4");
            Check("");
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void ConversionTest()
        {
            void Check(int MainGroup, int MiddleGroup, int SubGroup, byte[] Expected)
            {
                var ga = new KnxThreeLevelGroupAddress(MainGroup, MiddleGroup, SubGroup);
                var address = ga.GetAddress();
                var gaNew = new KnxThreeLevelGroupAddress(address);

                Assert.AreEqual(Expected, address);
                Assert.AreEqual(ga.MainGroup, gaNew.MainGroup);
                Assert.AreEqual(ga.MiddleGroup, gaNew.MiddleGroup);
                Assert.AreEqual(ga.SubGroup, gaNew.SubGroup);
            }

            Check(20, 0, 180, new byte[] { 0xa0, 0xb4 });
            Check(10, 2, 0, new byte[] { 0x52, 0x00 });
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Test]
        public void EqualTest()
        {
            var ga1 = new KnxThreeLevelGroupAddress(1, 2, 3);
            var ga2 = new KnxThreeLevelGroupAddress(1, 2, 3);
            var ga3 = new KnxThreeLevelGroupAddress(4, 5, 6);

            Assert.AreEqual(true, ga1.Equals(ga2));
            Assert.AreEqual(false, ga1.Equals(ga3));

            var gaTwoLevel = new KnxTwoLevelGroupAddress(ga1.GetAddress());

            Assert.AreEqual(false, ga1.Equals(gaTwoLevel));

            var pa1 = new KnxIndividualAddress(1, 2, 3);
            var pa2 = new KnxIndividualAddress(ga1.GetAddress());

            Assert.AreEqual(false, ga1.Equals(pa1));
            Assert.AreEqual(false, ga1.Equals(pa2));
        }
    }
}

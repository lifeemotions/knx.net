using NUnit.Framework;
using KNXLib.GA;
using KNXLib.Exceptions;

namespace KNXLibTests.Unit.GA
{
    [TestFixture]
    internal class KnxThreeLevelGroupAddressTest
    {
        [Category("KNXLib.Unit.GA.ThreeLevel"), Test]
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

        [Category("KNXLib.Unit.GA.ThreeLevel"), Test]
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


        [Category("KNXLib.Unit.GA.ThreeLevel"), Test]
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


        [Category("KNXLib.Unit.GA.ThreeLevel"), Test]
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

        [Category("KNXLib.Unit.GA.ThreeLevel"), Test]
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
    }
}

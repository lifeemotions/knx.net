using NUnit.Framework;
using KNXLib.GA;
using KNXLib.Exceptions;

namespace KNXLibTests.Unit.GA
{
    [TestFixture]
    internal class KnxTwoLevelGroupAddressTest
    {
        [Category("KNXLib.Unit.GA.TwoLevel"), Test]
        public void InvalidTest()
        {
            void Check(int MainGroup, int SubGroup)
            {
                var ga = new KnxTwoLevelGroupAddress(MainGroup, SubGroup);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check(0, 0);        // 0/0/0 is not allowed           
            Check(41, 5);       // Main too high           
            Check(15, 3542);    // Sub too high
        }

        [Category("KNXLib.Unit.GA.TwoLevel"), Test]
        public void ValidTest()
        {
            void Check(int MainGroup, int SubGroup)
            {
                var ga = new KnxTwoLevelGroupAddress(MainGroup, SubGroup);

                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(MainGroup, ga.MainGroup);
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check(0, 1);        // Min
            Check(31, 2047);    // Max
            Check(18, 230);
            Check(21, 1356);
        }


        [Category("KNXLib.Unit.GA.TwoLevel"), Test]
        public void ValidParserTest()
        {
            void Check(string groupAddress, int MainGroup, int SubGroup)
            {
                var ga = new KnxTwoLevelGroupAddress(groupAddress);
                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(MainGroup, ga.MainGroup);
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check("0/1", 0, 1);
            Check("31/2047", 31, 2047);
            Check("18/230", 18, 230);
            Check("21/1356", 21, 1356);
        }


        [Category("KNXLib.Unit.GA.TwoLevel"), Test]
        public void InvalidParserTest()
        {
            void Check(string groupAddress)
            {
                var ga = new KnxTwoLevelGroupAddress(groupAddress);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check("0/0/0");
            Check("0/0");
            Check("35/45");
            Check("5,6");
            Check("");
        }

        [Category("KNXLib.Unit.GA.TwoLevel"), Test]
        public void ConversionTest()
        {
            void Check(int MainGroup, int SubGroup, byte[] Expected)
            {
                var ga = new KnxTwoLevelGroupAddress(MainGroup, SubGroup);
                var address = ga.GetAddress();
                var gaNew = new KnxTwoLevelGroupAddress(address);

                Assert.AreEqual(Expected, address);
                Assert.AreEqual(ga.MainGroup, gaNew.MainGroup);
                Assert.AreEqual(ga.MiddleGroup, gaNew.MiddleGroup);
                Assert.AreEqual(ga.SubGroup, gaNew.SubGroup);
            }

            Check(20, 180, new byte[] { 0xa0, 0xb4 });
            Check(10, 512, new byte[] { 0x52, 0x00 });
        }
    }
}

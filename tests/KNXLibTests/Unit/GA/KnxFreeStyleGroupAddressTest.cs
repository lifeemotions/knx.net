using NUnit.Framework;
using KNXLib.GA;
using KNXLib.Exceptions;

namespace KNXLibTests.Unit.GA
{
    [TestFixture]
    internal class KnxFreeStyleGroupAddressTest
    {
        [Category("KNXLib.Unit.GA.FreeLevel"), Test]
        public void InvalidTest()
        {
            void Check(int SubGroup)
            {
                var ga = new KnxFreeStyleGroupAddress(SubGroup);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check(0);        // 0/0/0 is not allowed                
            Check(70565);    // Sub too high
        }

        [Category("KNXLib.Unit.GA.FreeLevel"), Test]
        public void ValidTest()
        {
            void Check(int SubGroup)
            {
                var ga = new KnxFreeStyleGroupAddress(SubGroup);

                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check(1);        // Min
            Check(65535);    // Max
            Check(18);
            Check(12045);
        }


        [Category("KNXLib.Unit.GA.FreeLevel"), Test]
        public void ValidParserTest()
        {
            void Check(string groupAddress, int SubGroup)
            {
                var ga = new KnxFreeStyleGroupAddress(groupAddress);
                Assert.AreEqual(true, ga.IsValid());
                Assert.AreEqual(SubGroup, ga.SubGroup);
            }

            Check("1", 1);
            Check("65535", 65535);
            Check("18", 18);
            Check("12045", 12045);
        }


        [Category("KNXLib.Unit.GA.FreeLevel"), Test]
        public void InvalidParserTest()
        {
            void Check(string groupAddress)
            {
                var ga = new KnxFreeStyleGroupAddress(groupAddress);
                Assert.AreEqual(false, ga.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => ga.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check("0/0/0");
            Check("0/0");
            Check("0");
            Check("70235");
            Check("5,6");
            Check("");
        }

        [Category("KNXLib.Unit.GA.FreeLevel"), Test]
        public void ConversionTest()
        {
            void Check(int SubGroup, byte[] Expected)
            {
                var ga = new KnxFreeStyleGroupAddress(SubGroup);
                var address = ga.GetAddress();
                var gaNew = new KnxFreeStyleGroupAddress(address);

                Assert.AreEqual(Expected, address);
                Assert.AreEqual(ga.MainGroup, gaNew.MainGroup);
                Assert.AreEqual(ga.MiddleGroup, gaNew.MiddleGroup);
                Assert.AreEqual(ga.SubGroup, gaNew.SubGroup);
            }

            Check(41140, new byte[] { 0xa0, 0xb4 });
            Check(20992, new byte[] { 0x52, 0x00 });
        }
    }
}

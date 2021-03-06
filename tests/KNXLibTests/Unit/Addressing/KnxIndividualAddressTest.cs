using NUnit.Framework;
using KNXLib.Addressing;
using KNXLib.Exceptions;

namespace KNXLibTests.Unit.Addressing
{
    [TestFixture]
    internal class KnxIndividualAddressTest
    {
        [Category("KNXLib.Unit.Address.Individual"), Test]
        public void InvalidTest()
        {
            void Check(int area, int line, int participant)
            {
                var pa = new KnxIndividualAddress(area, line, participant);
                Assert.AreEqual(false, pa.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => pa.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check(0, 0, 0);     // Not allowed
            Check(16, 1, 1);    // Area too high
            Check(10, 16, 1);   // Line too high
            Check(10, 10, 300); // Participant too high
        }

        [Category("KNXLib.Unit.Address.Individual"), Test]
        public void ValidTest()
        {
            void Check(int area, int line, int participant)
            {
                var pa = new KnxIndividualAddress(area, line, participant);

                Assert.AreEqual(true, pa.IsValid());
                Assert.AreEqual(area, pa.Area);
                Assert.AreEqual(line, pa.Line);
                Assert.AreEqual(participant, pa.Participant);
                Assert.AreEqual(true, pa.Equals(area, line, participant));
                Assert.AreEqual(true, pa.Equals($"{area}.{line}.{participant}"));
            }

            Check(0, 0, 1);
            Check(15, 15, 255);
            Check(10, 10, 10);
        }


        [Category("KNXLib.Unit.Address.Individual"), Test]
        public void ValidParserTest()
        {
            void Check(string address, int area, int line, int participant)
            {
                var pa = new KnxIndividualAddress(address);

                Assert.AreEqual(true, pa.IsValid());
                Assert.AreEqual(area, pa.Area);
                Assert.AreEqual(line, pa.Line);
                Assert.AreEqual(participant, pa.Participant);
                Assert.AreEqual(true, pa.Equals(area, line, participant));
                Assert.AreEqual(true, pa.Equals($"{area}.{line}.{participant}"));
            }

            Check("0.0.1", 0, 0, 1);
            Check("15.15.255", 15, 15, 255);
            Check("10.10.10", 10, 10, 10);
        }


        [Category("KNXLib.Unit.Address.Individual"), Test]
        public void InvalidParserTest()
        {
            void Check(string address)
            {
                var pa = new KnxIndividualAddress(address);
                Assert.AreEqual(false, pa.IsValid());

                // Test if exception is thrown when using an invalid GA
                TestDelegate exceptionTest = () => pa.GetAddress();
                Assert.Throws<InvalidKnxAddressException>(exceptionTest);
            }

            Check("16.16.16");
            Check("0/0");
            Check("0");
            Check("15,15,15");
            Check("5,6");
            Check("");
        }

        [Category("KNXLib.Unit.Address.Individual"), Test]
        public void ConversionTest()
        {
            void Check(int area, int line, int participant, byte[] Expected)
            {
                var pa = new KnxIndividualAddress(area, line, participant);
                var address = pa.GetAddress();
                var paNew = new KnxIndividualAddress(address);

                Assert.AreEqual(Expected, address);
                Assert.AreEqual(pa.Area, paNew.Area);
                Assert.AreEqual(pa.Line, paNew.Line);
                Assert.AreEqual(pa.Participant, paNew.Participant);
            }

            Check(1, 1, 80, new byte[] { 0x11, 0x50 });
            Check(1, 1, 127, new byte[] { 0x11, 0x7f });
            Check(15, 15, 15, new byte[] { 0xFF, 0x0F });
        }
    }
}

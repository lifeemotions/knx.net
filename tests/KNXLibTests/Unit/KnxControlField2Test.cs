using NUnit.Framework;
using KNXLib;
using KNXLib.Enums;
using KNXLib.Addressing;

namespace KNXLibTests.Unit
{
    [TestFixture]
    public class KnxControlField2Test
    {
        [Category("KNXLib.Unit.ControlField2"), Test]
        public void ConversionTest()
        {
            var cf = new KnxControlField2(KnxDestinationAddressType.Group, 5);
            Assert.AreEqual(0xd0, cf.GetValue());

            var cfNew = new KnxControlField2(cf.GetValue());
            Assert.AreEqual(KnxDestinationAddressType.Group, cfNew.DestinationAddressType);
            Assert.AreEqual(5, cfNew.HopCount);
        }

        [Category("KNXLib.Unit.ControlField2"), Test]
        public void PassAddress()
        {
            var cf = new KnxControlField2(new KnxThreeLevelGroupAddress(12, 3, 4));
            Assert.AreEqual(0xE0, cf.GetValue());

            var cfNew = new KnxControlField2(new KnxIndividualAddress(1, 2, 3));
            Assert.AreEqual(0x70, cfNew.GetValue());
        }
    }
}

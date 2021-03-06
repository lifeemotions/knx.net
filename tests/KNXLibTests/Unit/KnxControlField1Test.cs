using KNXLib;
using KNXLib.Enums;
using NUnit.Framework;

namespace KNXLibTests.Unit
{
    [TestFixture]
    public class KnxControlField1Test
    {
        [Category("KNXLib.Unit.ControlField1"), Test]
        public void ConversionTest()
        {
            var cf = new KnxControlField1(KnxTelegramType.StandardFrame, KnxTelegramRepetitionStatus.Original, KnxTelegramPriority.Low);
            Assert.AreEqual(0xbc, cf.GetValue());

            var cfNew = new KnxControlField1(cf.GetValue());
            Assert.AreEqual(KnxTelegramType.StandardFrame, cfNew.TelegramType);
            Assert.AreEqual(KnxTelegramRepetitionStatus.Original, cfNew.TelegramRepetitionStatus);
            Assert.AreEqual(KnxTelegramPriority.Low, cfNew.TelegramPriority);
        }
    }
}

using System;
using System.Threading;
using KNXLib;
using KNXLib.Addressing;
using KNXLib.Events;
using NUnit.Framework;

namespace KNXLibTests.Integration.Tunneling
{
    [TestFixture, Platform(Exclude = "Win")]
    internal class ActionFeedback
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            try
            {
                if (!Support.Eibd.DaemonManager.IsEibdAvailable())
                    throw new PlatformNotSupportedException(
                        "Can't run integration tests without eibd daemon installed on the system");
                if (!Support.Eibd.VBusMonitorManager.IsVBusMonitorAvailable())
                    throw new PlatformNotSupportedException(
                        "Can't run integration tests without vbusmonitor installed on the system");

                if (!Support.Eibd.DaemonManager.StartTunneling())
                    throw new Exception("Could not start eibd daemon");
                if (!Support.Eibd.VBusMonitorManager.Start())
                    throw new Exception("Could not start vbusmonitor");
            }
            catch (Exception)
            {
                TearDown();
                throw;
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Support.Eibd.DaemonManager.Stop();
            Support.Eibd.VBusMonitorManager.Stop();
        }

        private ManualResetEventSlim ResetEvent { get; set; }
        private const string LightOnOffAddress = "5/0/2";
        private const bool LightOnOffActionStatus = true;
        private const int Timeout = 2000;

        [Category("KNXLib.Integration.Tunneling.ActionFeedback"), Test]
        public void TunnelingActionFeedbackTest()
        {
            ResetEvent = new ManualResetEventSlim();

            KnxConnection connection = new KnxConnectionTunneling("127.0.0.1", 3671, "127.0.0.1", 3672) { Debug = false };

            connection.KnxEventDelegate += Event;

            connection.Connect();

            Thread.Sleep(50);

            connection.Action(KnxGroupAddress.Parse(LightOnOffAddress), true);

            if (!ResetEvent.Wait(Timeout))
                Assert.Fail("Didn't receive feedback from the action");
        }

        private void Event(object sender, KnxEventArgs args)
        {
            //Console.WriteLine("Received feedback from " + address + " with value " + (int) state[0]);
            if (LightOnOffAddress.Equals(args.DestinationAddress.ToString()) && args.State != null && args.State.Length == 1 && args.State[0] == 1)
                ResetEvent.Set();
        }
    }
}

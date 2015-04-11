using System;
using System.Threading;
using KNXLib;
using NUnit.Framework;

namespace KNXLibTests.Integration.Tunneling
{
    [TestFixture]
    internal class ActionFeedback
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            try
            {
                if (!Support.Eibd.DaemonManager.IsEibdAvailable())
                    throw new PlatformNotSupportedException("Can't run integration tests without eibd daemon installed on the system");
                if (!Support.Eibd.VBusMonitorManager.IsVBusMonitorAvailable())
                    throw new PlatformNotSupportedException("Can't run integration tests without vbusmonitor installed on the system");

                if (!Support.Eibd.DaemonManager.StartTunneling())
                    throw new Exception("Could not start eibd daemon");
                if (!Support.Eibd.VBusMonitorManager.Start())
                    throw new Exception("Could not start vbusmonitor");
            }
            catch (Exception e)
            {
                TearDown();
                throw e;
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Support.Eibd.DaemonManager.Stop();
            Support.Eibd.VBusMonitorManager.Stop();
        }

        private ManualResetEventSlim ResetEvent { get; set; }
        private const string LightOnOffAddress = "5/0/2";
        private const bool LightOnOffActionStatus = true;
        private const int Timeout = 500;

        [Category("KNXLib.Integration.Tunneling.ActionFeedback"), Test, Platform(Exclude = "Win")]
        public void TunnelingActionFeedbackTest()
        {
            ResetEvent = new ManualResetEventSlim();
            KnxConnection _connection;

            _connection = new KnxConnectionTunneling("127.0.0.1", 3671, "127.0.0.1", 3672) { Debug = false };

            _connection.KnxEventDelegate += Event;

            _connection.Connect();

            _connection.Action(LightOnOffAddress, true);
            if (!ResetEvent.Wait(Timeout))
                Assert.Fail("Didn't receive feedback from the action");
        }

        private void Event(string address, string state)
        {
            //Console.WriteLine("Received feedback from " + address + " with value " + (int) state[0]);
            if (LightOnOffAddress.Equals(address) && state != null && state.Length == 1 && state[0] == 1)
                ResetEvent.Set();
        }
    }
}

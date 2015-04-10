using System;
using System.Threading;
using KNXLib;
using NUnit.Framework;

namespace KNXLibTests.Unit.DataPoint
{
    [TestFixture]
    internal class ActionFeedback
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            if (!Support.Eibd.DaemonManager.IsEibdAvailable())
                throw new PlatformNotSupportedException("Can't run integration tests without eibd daemon installed on the system");

            Support.Eibd.DaemonManager.StartRouting();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        private ManualResetEventSlim ResetEvent { get; set; }
        private const string LightOnOffAddress = "5/0/2";
        private const bool LightOnOffActionStatus = true;
        private const int Timeout = 100;

        [Category("KNXLib.Integration.Routing.ActionFeedback"), Test]
        public void RoutingActionFeedbackTest()
        {
            ResetEvent = new ManualResetEventSlim();
            KnxConnection _connection;

            _connection = new KnxConnectionRouting { Debug = false };

            _connection.KnxEventDelegate += Event;
            //_connection.KnxStatusDelegate += Status;
            _connection.Connect();

            _connection.Action(LightOnOffAddress, true);
            if (!ResetEvent.Wait(Timeout))
                Assert.Fail("Didn't receive feedback from the action");
        }

        private void Event(string address, string state)
        {
            if (LightOnOffAddress.Equals(address) && LightOnOffActionStatus.Equals(state))
                ResetEvent.Set();
        }
    }
}

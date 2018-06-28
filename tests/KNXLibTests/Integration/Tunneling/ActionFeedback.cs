using System;
using System.Threading;
using KNXLib;
using KNXLibTests.Support.Os;
using Xunit;

namespace KNXLibTests.Integration.Tunneling
{
    public class ActionFeedback
    {
        private void StartEibd()
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
                StopEibd();
                throw;
            }
        }

        private void StopEibd()
        {
            Support.Eibd.DaemonManager.Stop();
            Support.Eibd.VBusMonitorManager.Stop();
        }

        private ManualResetEventSlim ResetEvent { get; set; }
        private const string LightOnOffAddress = "5/0/2";
        private const bool LightOnOffActionStatus = true;
        private const int Timeout = 2000;

        [SkippableFact]
        [Trait("Category","KNXLib.Integration.Tunneling.ActionFeedback")]
        public void TunnelingActionFeedbackTest()
        {
            Skip.IfNot(Tools.IsLinux);

            StartEibd();

            try
            {
                ResetEvent = new ManualResetEventSlim();

                KnxConnection connection = new KnxConnectionTunneling("127.0.0.1", 3671, "127.0.0.1", 3672) { Debug = false };

                connection.KnxEventDelegate += Event;

                connection.Connect();

                Thread.Sleep(50);

                connection.Action(LightOnOffAddress, true);

                Assert.True(ResetEvent.Wait(Timeout), "Didn't receive feedback from the action");
            }
            finally
            {
                StopEibd();
            }
        }

        private void Event(string address, string state)
        {
            if (LightOnOffAddress.Equals(address) && state != null && state.Length == 1 && state[0] == 1)
                ResetEvent.Set();
        }
    }
}

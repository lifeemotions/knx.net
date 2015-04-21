using System;
using System.Threading;
using KNXLib.Logging;

namespace KNXLib
{
    internal class KnxLockManager
    {
        private static readonly ILog Logger = LogProvider.For<KnxLockManager>();

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(0);
        private readonly object _connectedLock = new object();
        private bool _isConnected;

        public void LockConnection()
        {
            lock (_connectedLock)
            {
                Logger.Debug(() => string.Format("KNX is disconnected. Send locked - {0} free locks", _sendLock.CurrentCount));

                if (!_isConnected)
                    return;

                SendLock();
                _isConnected = false;
            }
        }

        public void UnlockConnection()
        {
            lock (_connectedLock)
            {
                Logger.Debug(() => string.Format("Unlocking send - {0} free locks", _sendLock.CurrentCount));

                if (_isConnected)
                    return;

                _isConnected = true;
                SendUnlock();
            }
        }

        public void PerformLockedOperation(Action action)
        {
            // TODO: Shouldn't this check if we are connected?

            try
            {
                SendLock();
                action();
            }
            finally
            {
                SendUnlockPause();
            }
        }

        private void SendLock()
        {
            _sendLock.Wait();
        }

        private void SendUnlock()
        {
            _sendLock.Release();
        }

        private void SendUnlockPause()
        {
            var t = new Thread(SendUnlockPauseThread) { IsBackground = true };
            t.Start();
        }

        private void SendUnlockPauseThread()
        {
            Thread.Sleep(200);
            _sendLock.Release();
        }
    }
}
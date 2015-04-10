using System;
using System.Net;
using System.Text;
using System.Threading;
using KNXLib.DPT;
using KNXLib.Exceptions;

namespace KNXLib
{
    public abstract class KnxConnection
    {
        public delegate void KnxConnected();
        public KnxConnected KnxConnectedDelegate = null;

        public delegate void KnxDisconnected();
        public KnxDisconnected KnxDisconnectedDelegate = null;

        public delegate void KnxEvent(string address, string state);
        public KnxEvent KnxEventDelegate = null;

        public delegate void KnxStatus(string address, string state);
        public KnxStatus KnxStatusDelegate = null;

        private readonly object _connectedLock = new object();
        private bool _isConnected;

        protected KnxConnection(string host, int port)
        {
            ConnectionConfiguration = new KnxConnectionConfiguration(host, port);

            ActionMessageCode = 0x00;
            ThreeLevelGroupAddressing = true;
            Debug = false;
        }

        protected KnxConnectionConfiguration ConnectionConfiguration { get; private set; }

        protected IPEndPoint RemoteEndpoint
        {
            get { return ConnectionConfiguration.EndPoint; }
        }

        internal KnxReceiver KnxReceiver { get; set; }

        internal KnxSender KnxSender { get; set; }

        public bool ThreeLevelGroupAddressing { get; set; }

        public bool Debug { get; set; }

        public byte ActionMessageCode { get; set; }

        public abstract void Connect();

        public abstract void Disconnect();

        // TODO: Could we refactor connection handling out, weird to have these public too, feels like only inheritors should call it
        public virtual void Connected()
        {
            try
            {
                if (KnxConnectedDelegate != null)
                    KnxConnectedDelegate();
            }
            catch
            {
                //ignore
            }

            Log("KNX is connected. Unlocking send - {0} free locks", SemaphoreCount());

            lock (_connectedLock)
            {
                if (_isConnected)
                    return;

                _isConnected = true;
                SendUnlock();
            }
        }

        public virtual void Disconnected()
        {
            lock (_connectedLock)
            {
                if (_isConnected)
                {
                    SendLock();
                    _isConnected = false;
                }
            }

            try
            {
                if (KnxDisconnectedDelegate != null)
                    KnxDisconnectedDelegate();
            }
            catch
            {
                //ignore
            }

            Log("KNX is disconnected. Send locked - {0} free locks", SemaphoreCount());
        }

        public void Event(string address, string state)
        {
            try
            {
                if (KnxEventDelegate != null)
                    KnxEventDelegate(address, state);
            }
            catch
            {
                //ignore
            }

            Log("Device {0} has status {1}", address, state);
        }

        public void Status(string address, string state)
        {
            try
            {
                if (KnxStatusDelegate != null)
                    KnxStatusDelegate(address, state);
            }
            catch
            {
                //ignore
            }

            Log("Device {0} has status {1}", address, state);
        }

        // TODO: Might be good to refactor this out
        public void Action(string address, bool data)
        {
            byte[] val;

            try
            {
                val = new[] { Convert.ToByte(data) };
            }
            catch
            {
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val);
        }

        public void Action(string address, string data)
        {
            byte[] val;
            try
            {
                val = Encoding.ASCII.GetBytes(data);
            }
            catch
            {
                throw new InvalidKnxDataException(data);
            }

            if (val == null)
                throw new InvalidKnxDataException(data);

            Action(address, val);
        }

        public void Action(string address, int data)
        {
            var val = new byte[2];
            if (data <= 255)
            {
                val[0] = 0x00;
                val[1] = (byte)data;
            }
            else if (data <= 65535)
            {
                val[0] = (byte)data;
                val[1] = (byte)(data >> 8);
            }
            else
            {
                // allowing only positive integers less than 65535 (2 bytes), maybe it is incorrect...???
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val);
        }

        public void Action(string address, byte data)
        {
            Action(address, new byte[] { 0x00, data });
        }

        public void Action(string address, byte[] data)
        {
            Log("Sending {0} to {1}.", data, address);

            try
            {
                SendLock();
                KnxSender.Action(address, data);
            }
            finally
            {
                SendUnlockPause();
            }

            Log("Sent {0} to {1}.", data, address);
        }

        // TODO: It would be good to make a type for address, to make sure not any random string can be passed in
        public void RequestStatus(string address)
        {
           Log("Sending request status to {0}.", address);

            try
            {
                SendLock();
                KnxSender.RequestStatus(address);
            }
            finally
            {
                SendUnlockPause();
            }

            Log("Sent request status to {0}.", address);
        }

        private void Log(string message, params object[] arg)
        {
            if (Debug)
                Console.WriteLine(message, arg);
        }

        // TODO: Not sure if these DPT methods make much sense on connection, unless we want to hide the helper classes
        public object FromDPT(string type, string data)
        {
            return DPTTranslator.Instance.FromDPT(type, data);
        }

        public object FromDPT(string type, byte[] data)
        {
            return DPTTranslator.Instance.FromDPT(type, data);
        }

        public byte[] ToDPT(string type, string value)
        {
            return DPTTranslator.Instance.ToDPT(type, value);
        }

        public byte[] ToDPT(string type, object value)
        {
            return DPTTranslator.Instance.ToDPT(type, value);
        }

        // TODO: Refactor this out
        private readonly SemaphoreSlim _lockSend = new SemaphoreSlim(0);

        private void SendLock()
        {
            _lockSend.Wait();
        }

        private void SendUnlock()
        {
            _lockSend.Release();
        }

        private int SemaphoreCount()
        {
            return _lockSend.CurrentCount;
        }

        private void SendUnlockPause()
        {
            var t = new Thread(SendUnlockPauseThread) { IsBackground = true };
            t.Start();
        }

        private void SendUnlockPauseThread()
        {
            Thread.Sleep(200);
            _lockSend.Release();
        }
    }
}

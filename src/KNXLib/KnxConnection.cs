using System;
using System.Net;
using System.Text;
using KNXLib.DPT;
using KNXLib.Exceptions;
using KNXLib.Logging;

namespace KNXLib
{
    public abstract class KnxConnection : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<KnxConnection>();

        public delegate void KnxConnected();
        public KnxConnected KnxConnectedDelegate = () => { };

        public delegate void KnxDisconnected();
        public KnxDisconnected KnxDisconnectedDelegate = () => { };

        public delegate void KnxEvent(string address, string state);
        public KnxEvent KnxEventDelegate = (address, state) => { };

        public delegate void KnxStatus(string address, string state);
        public KnxStatus KnxStatusDelegate = (address, state) => { };

        private readonly KnxLockManager _lockManager = new KnxLockManager();

        protected KnxConnection(string host, int port)
        {
            ConnectionConfiguration = new KnxConnectionConfiguration(host, port);

            ActionMessageCode = 0x00;
            ThreeLevelGroupAddressing = true;
        }

        internal KnxConnectionConfiguration ConnectionConfiguration { get; private set; }

        protected IPEndPoint RemoteEndpoint
        {
            get { return ConnectionConfiguration.EndPoint; }
        }

        internal KnxReceiver KnxReceiver { get; set; }

        internal KnxSender KnxSender { get; set; }

        public bool ThreeLevelGroupAddressing { get; set; }

        public byte ActionMessageCode { get; set; }

        public abstract void Connect();

        public abstract void Disconnect();

        // TODO: Could we refactor connection handling out, weird to have these public too, feels like only inheritors should call it
        public virtual void Connected()
        {
            try
            {
                Logger.Debug(() => "KNX is connected.");

                KnxConnectedDelegate();
            }
            catch
            {
                //ignore
            }

            _lockManager.UnlockConnection();
        }

        public virtual void Disconnected()
        {
            _lockManager.LockConnection();

            try
            {
                Logger.Debug(() => "KNX is disconnected.");

                KnxDisconnectedDelegate();
            }
            catch
            {
                //ignore
            }
        }

        internal void Event(string address, string state)
        {
            try
            {
                Logger.Debug(() => string.Format("Device {0} sent event {1}", address, state));

                KnxEventDelegate(address, state);
            }
            catch
            {
                //ignore
            }
        }

        internal void Status(string address, string state)
        {
            try
            {
                Logger.Debug(() => string.Format("Device {0} has status {1}", address, state));

                KnxStatusDelegate(address, state);
            }
            catch
            {
                //ignore
            }
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
            Logger.Debug(() => string.Format("Sending {0} to {1}.", data, address));

            _lockManager.PerformLockedOperation(() => KnxSender.Action(address, data));

            Logger.Debug(() => string.Format("Sent {0} to {1}.", data, address));
        }

        // TODO: It would be good to make a type for address, to make sure not any random string can be passed in
        public void RequestStatus(string address)
        {
            Logger.Debug(() => string.Format("Sending request status to {0}.", address));

            _lockManager.PerformLockedOperation(() => KnxSender.RequestStatus(address));

            Logger.Debug(() => string.Format("Sent request status to {0}.", address));
        }

        // TODO: Not sure if these DPT methods make much sense on connection, unless we want to hide the helper classes
        public object FromDataPoint(string type, string data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        public object FromDataPoint(string type, byte[] data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        public byte[] ToDataPoint(string type, string value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }

        public byte[] ToDataPoint(string type, object value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }

        public abstract void Dispose();
        protected abstract void Dispose(bool disposing);
    }
}

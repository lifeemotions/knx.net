using System;
using System.Net;
using System.Text;
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
        public KnxEvent KnxEventDelegate = (address, state) => { };

        public delegate void KnxStatus(string address, string state);
        public KnxStatus KnxStatusDelegate = (address, state) => { };

        private readonly KnxLockManager _lockManager = new KnxLockManager();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        protected KnxConnection(string host, int port)
        {
            ConnectionConfiguration = new KnxConnectionConfiguration(host, port);

            ActionMessageCode = 0x00;
            ThreeLevelGroupAddressing = true;
            Debug = false;
        }

        internal KnxConnectionConfiguration ConnectionConfiguration { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected IPEndPoint RemoteEndpoint
        {
            get { return ConnectionConfiguration.EndPoint; }
        }

        internal KnxReceiver KnxReceiver { get; set; }

        internal KnxSender KnxSender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ThreeLevelGroupAddressing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte ActionMessageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 
        /// </summary>
        internal virtual void Connected()
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

            Log("KNX is connected. Unlocking send - {0} free locks", _lockManager.LockCount);

            _lockManager.UnlockConnection();
        }

        /// <summary>
        /// 
        /// </summary>
        internal virtual void Disconnected()
        {
            _lockManager.LockConnection();

            try
            {
                if (KnxDisconnectedDelegate != null)
                    KnxDisconnectedDelegate();
            }
            catch
            {
                //ignore
            }

            Log("KNX is disconnected. Send locked - {0} free locks", _lockManager.LockCount);
        }

        internal void Event(string address, string state)
        {
            try
            {
                KnxEventDelegate(address, state);
            }
            catch
            {
                //ignore
            }

            Log("Device {0} sent event {1}", address, state);
        }

        internal void Status(string address, string state)
        {
            try
            {
                KnxStatusDelegate(address, state);
            }
            catch
            {
                //ignore
            }

            Log("Device {0} has status {1}", address, state);
        }

        // TODO: Might be good to refactor this out
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <exception cref="InvalidKnxDataException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <exception cref="InvalidKnxDataException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <exception cref="InvalidKnxDataException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void Action(string address, byte data)
        {
            Action(address, new byte[] { 0x00, data });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        public void Action(string address, byte[] data)
        {
            Log("Sending {0} to {1}.", data, address);

            _lockManager.PerformLockedOperation(() => KnxSender.Action(address, data));

            Log("Sent {0} to {1}.", data, address);
        }

        // TODO: It would be good to make a type for address, to make sure not any random string can be passed in
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        public void RequestStatus(string address)
        {
            Log("Sending request status to {0}.", address);

            _lockManager.PerformLockedOperation(() => KnxSender.RequestStatus(address));

            Log("Sent request status to {0}.", address);
        }

        private void Log(string message, params object[] arg)
        {
            if (Debug)
                Console.WriteLine(message, arg);
        }

        // TODO: Not sure if these DPT methods make much sense on connection, unless we want to hide the helper classes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public object FromDataPoint(string type, string data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public object FromDataPoint(string type, byte[] data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, string value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, object value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }
    }
}
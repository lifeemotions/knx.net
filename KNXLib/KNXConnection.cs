using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNXLib.Exceptions;
using System.Threading;

namespace KNXLib
{
    public abstract class KNXConnection
    {
        #region constructor
        public KNXConnection()
        {
            this._actionMessageCode = 0x00;
        }

        public KNXConnection(String host)
        {
        }

        public KNXConnection(String host, int port)
        {
            this.Initialize();
            this.Host = host;
            this.Port = port;

            IP = null;
            try
            {
                IP = IPAddress.Parse(host);
            }
            catch (Exception)
            {
                try
                {
                    IP = Dns.GetHostEntry(host).AddressList[0];
                }
                catch (SocketException)
                {
                }
            }
            if (IP == null)
                throw new InvalidHostException(host);
        }

        private void Initialize()
        {
            this.ThreeLevelGroupAddressing = true;
            this.Debug = false;
        }
        #endregion

        #region variables
        private string _host;
        public string Host
        {
            get
            {
                return this._host;
            }
            internal set
            {
                this._host = value;
            }
        }

        private int _port;
        public int Port
        {
            get
            {
                return this._port;
            }
            internal set
            {
                this._port = value;
            }
        }

        private IPAddress _ip = null;
        internal IPAddress IP
        {
            get
            {
                return this._ip;
            }
            set
            {
                this._ip = value;
            }
        }

        private KNXReceiver _KNXReceiver = null;
        internal KNXReceiver KNXReceiver
        {
            get
            {
                return this._KNXReceiver;
            }
            set
            {
                this._KNXReceiver = value;
            }
        }

        private KNXSender _KNXSender = null;
        internal KNXSender KNXSender
        {
            get
            {
                return this._KNXSender;
            }
            set
            {
                this._KNXSender = value;
            }
        }

        private bool _threeLevelGroupAddressing;
        public bool ThreeLevelGroupAddressing
        {
            get
            {
                return this._threeLevelGroupAddressing;
            }
            set
            {
                this._threeLevelGroupAddressing = value;
            }
        }

        private bool _debug;
        public bool Debug
        {
            get
            {
                return this._debug;
            }
            set
            {
                this._debug = value;
            }
        }

        private byte _actionMessageCode;
        public byte ActionMessageCode
        {
            get
            {
                return this._actionMessageCode;
            }
            set
            {
                this._actionMessageCode = value;
            }
        }
        #endregion

        #region connection

        public abstract void Connect();

        public abstract void Disconnect();

        #endregion

        #region events
        public delegate void KNXConnected();
        public KNXConnected KNXConnectedDelegate = null;
        public delegate void KNXDisconnected();
        public KNXDisconnected KNXDisconnectedDelegate = null;
        public delegate void KNXEvent(string address, string state);
        public KNXEvent KNXEventDelegate = null;
        public delegate void KNXStatus(string address, string state);
        public KNXStatus KNXStatusDelegate = null;

        private object _connectedKey = new object();
        private bool _connected = false;
        public virtual void Connected()
        {
            try
            {
                if (KNXConnectedDelegate != null)
                    KNXConnectedDelegate();
            }
            catch (Exception)
            {
                //ignore
            }

            if (this.Debug)
            {
                Console.WriteLine("KNX is connected. Unlocking send - " + SemaphoreCount() + " free locks");
            }

            lock (_connectedKey)
            {
                if (_connected == false)
                {
                    _connected = true;
                    this.SendUnlock();
                }
            }
        }
        public virtual void Disconnected()
        {
            lock (_connectedKey)
            {
                if (_connected == true)
                {
                    this.SendLock();
                    _connected = false;
                }
            }

            try
            {
                if (KNXDisconnectedDelegate != null)
                    KNXDisconnectedDelegate();
            }
            catch (Exception)
            {
                //ignore
            }

            if (this.Debug)
            {
                Console.WriteLine("KNX is disconnected. Send locked - " + SemaphoreCount() + " free locks");
            }
        }

        public void Event(string address, string state)
        {
            try
            {
                if (KNXEventDelegate != null)
                    KNXEventDelegate(address, state);
            }
            catch (Exception)
            {
                //ignore
            }

            if (this.Debug)
            {
                Console.WriteLine("Device " + address + " has status " + state);
            }
        }
        public void Status(string address, string state)
        {
            try
            {
                if (KNXStatusDelegate != null)
                    KNXStatusDelegate(address, state);
            }
            catch (Exception)
            {
                //ignore
            }

            if (this.Debug)
            {
                Console.WriteLine("Device " + address + " has status " + state);
            }
        }
        #endregion

        #region locks
        private SemaphoreSlim _lockSend = new SemaphoreSlim(0);
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
            Thread t = new Thread(new ThreadStart(SendUnlockPauseThread));
            t.IsBackground = true;
            t.Start();
        }
        private void SendUnlockPauseThread()
        {
            Thread.Sleep(150);
            _lockSend.Release();
        }
        #endregion

        #region actions
        public void Action(string address, bool data)
        {
            byte[] val = null;
            try
            {
                val = new byte[] { Convert.ToByte(data) };
            }
            catch (Exception)
            {
                throw new InvalidKNXDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKNXDataException(data.ToString());

            if (Debug)
                Console.WriteLine("Sending " + val.ToString() + " to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.Action(address, val);
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        public void Action(string address, string data)
        {
            byte[] val = null;
            try
            {
                val = System.Text.Encoding.ASCII.GetBytes(data);
            }
            catch (Exception)
            {
                throw new InvalidKNXDataException(data);
            }

            if (val == null)
                throw new InvalidKNXDataException(data);

            if (Debug)
                Console.WriteLine("Sending " + val.ToString() + " to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.Action(address, val);
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        public void Action(string address, int data)
        {
            byte[] val = new byte[2];
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
            else // allowing only positive integers less than 65535 (2 bytes), maybe it is incorrect...???
                throw new InvalidKNXDataException(data.ToString());

            if (val == null)
                throw new InvalidKNXDataException(data.ToString());

            if (Debug)
                Console.WriteLine("Sending " + val.ToString() + " to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.Action(address, val);
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        public void Action(string address, byte data)
        {
            if (Debug)
                Console.WriteLine("Sending " + data.ToString() + " to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.Action(address, new byte[] { 0x00, data });
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        public void Action(string address, byte[] data)
        {
            if (Debug)
                Console.WriteLine("Sending " + data.ToString() + " to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.Action(address, data);
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        #endregion

        #region status
        public void RequestStatus(string address)
        {
            if (Debug)
                Console.WriteLine("Sending request status to " + address + ".");
            try
            {
                SendLock();
                this.KNXSender.RequestStatus(address);
            }
            finally
            {
                SendUnlockPause();
            }
            if (Debug)
                Console.WriteLine("Sent");
        }
        #endregion

        #region DPT
        public object fromDPT(string type, byte[] data)
        {
            return DPT.DPTTranslator.Instance.fromDPT(type, data);
        }

        public object fromDPT(string type, String data)
        {
            return DPT.DPTTranslator.Instance.fromDPT(type, data);
        }

        public byte[] toDPT(string type, object value)
        {
            return DPT.DPTTranslator.Instance.toDPT(type, value);
        }

        public byte[] toDPT(string type, String value)
        {
            return DPT.DPTTranslator.Instance.toDPT(type, value);
        }
        #endregion
    }
}

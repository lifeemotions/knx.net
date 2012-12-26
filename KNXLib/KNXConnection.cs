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

        private UdpClient _udpClient;
        protected UdpClient UdpClient
        {
            get
            {
                return this._udpClient;
            }
            set
            {
                this._udpClient = value;
            }
        }

        private IPEndPoint _localEndpoint;
        protected IPEndPoint LocalEndpoint
        {
            get
            {
                return this._localEndpoint;
            }
            set
            {
                this._localEndpoint = value;
            }
        }

        private IPEndPoint _remoteEndpoint;
        protected IPEndPoint RemoteEndpoint
        {
            get
            {
                return this._remoteEndpoint;
            }
            set
            {
                this._remoteEndpoint = value;
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
        protected ConnectionStatus _connected = ConnectionStatus.UNKNOWN;
        public virtual void Connected()
        {
            lock (_connectedKey)
            {
                if (_connected == ConnectionStatus.CONNECTING)
                {
                    _connected = ConnectionStatus.CONNECTED;
                    this.ConnectionUnlock();
                }
            }

            if (this.Debug)
            {
                Console.WriteLine("KNX is connected. Unlocked send. " + this.SendCount() + " threads waiting to send (to connect: " + this.ConnectCount() + ")");
            }

            try
            {
                if (KNXConnectedDelegate != null)
                    KNXConnectedDelegate();
            }
            catch (Exception)
            {
                //ignore
            }
        }
        public virtual void Disconnected()
        {
            lock (_connectedKey)
            {
                if (_connected == ConnectionStatus.CONNECTED)
                {
                    this.ConnectionLock();
                    _connected = ConnectionStatus.DISCONNECTED;
                }
                else if (_connected == ConnectionStatus.CONNECTING)
                {
                    _connected = ConnectionStatus.DISCONNECTED;
                }
            }

            if (this.Debug)
            {
                Console.WriteLine("KNX is disconnected. Locked send. " + this.SendCount() + " threads waiting to send (to connect: " + this.ConnectCount() + ")");
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
        private ReaderWriterLockSlim _lockSend = new ReaderWriterLockSlim();

        private void SendLock()
        {
            _lockSend.EnterReadLock();
        }
        protected void ConnectionLock()
        {
            _lockSend.EnterWriteLock();
        }
        protected void ConnectionUnlock()
        {
            _lockSend.ExitWriteLock();
        }
        private int SendCount()
        {
            return _lockSend.WaitingReadCount;
        }
        private int ConnectCount()
        {
            return _lockSend.WaitingWriteCount;
        }
        private void SendUnlockPause()
        {
            new Thread(new ThreadStart(SendUnlockPauseThread)).Start();
        }
        private void SendUnlockPauseThread()
        {
            Thread.Sleep(50);
            _lockSend.ExitReadLock();
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
    }
    public enum ConnectionStatus
    {
        CONNECTED = 0,
        DISCONNECTED = 1,
        CONNECTING = 2,
        UNKNOWN = 3
    }
}

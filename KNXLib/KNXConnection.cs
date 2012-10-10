using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNXLib.Exceptions;

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
        internal string Host
        {
            get
            {
                return this._host;
            }
            set
            {
                this._host = value;
            }
        }

        private int _port;
        internal int Port
        {
            get
            {
                return this._port;
            }
            set
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
        #endregion

        #region connection

        public abstract void Connect();

        public abstract void Disconnect();

        #endregion

        #region events
        public delegate void KNXEvent(string address, string state);
        public KNXEvent KNXEventDelegate = null;

        public void Event(string address, string state)
        {
            if (KNXEventDelegate != null)
                KNXEventDelegate(address, state);

            if (this.Debug)
            {
                Console.WriteLine("Device " + address + " has status " + state);
            }
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

            this.KNXSender.Action(address, val);
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

            this.KNXSender.Action(address, val);
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

            this.KNXSender.Action(address, val);
        }
        public void Action(string address, byte data)
        {
            this.KNXSender.Action(address, new byte[] { 0x00, data });
        }
        public void Action(string address, byte[] data)
        {
            this.KNXSender.Action(address, data);
        }
        #endregion
    }
}

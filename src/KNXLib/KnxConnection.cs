using System;
using System.Net;
using System.Text;
using System.Linq;
using KNXLib.DPT;
using KNXLib.Exceptions;
using KNXLib.Log;
using KNXLib.Enums;
using KNXLib.Addressing;
using KNXLib.Events;

namespace KNXLib
{
    /// <summary>
    ///     Base class that controls the KNX connection, implemented by KnxConnectionRouting and KnxConnetionTunneling
    /// </summary>
    public abstract class KnxConnection
    {
        private static readonly string ClassName = typeof(KnxConnection).ToString();

        /// <summary>
        ///     Delegate function for connection established trigger
        /// </summary>
        public delegate void KnxConnected();

        /// <summary>
        ///     Event triggered when connection is established
        /// </summary>
        public KnxConnected KnxConnectedDelegate = null;

        /// <summary>
        ///     Delegate function for disconnection trigger
        /// </summary>
        public delegate void KnxDisconnected();

        /// <summary>
        ///     Event triggered when connection drops
        /// </summary>
        public KnxDisconnected KnxDisconnectedDelegate = null;

        /// <summary>
        ///     Delegate function for KNX events
        /// </summary>
        public delegate void KnxEvent(object sender, KnxEventArgs args);

        /// <summary>
        ///     Event triggered when there is a new KNX event
        /// </summary>
        public KnxEvent KnxEventDelegate = (sender, args) => { };

        /// <summary>
        ///     Delegate function for KNX status queries
        /// </summary>
        public delegate void KnxStatus(object sender, KnxStatusArgs args);

        /// <summary>
        ///     Event triggered when received a status after a query
        /// </summary>
        public KnxStatus KnxStatusDelegate = (sender, args) => { };

        private readonly KnxLockManager _lockManager = new KnxLockManager();

        /// <summary>
        ///     Create a new KNX Connection to specified host and port
        /// </summary>
        /// <param name="host">Host to connect</param>
        /// <param name="port">Port to use</param>
        protected KnxConnection(string host, int port)
        {
            ConnectionConfiguration = new KnxConnectionConfiguration(host, port);

            ActionMessageCode = 0x00;
            GroupAddressStyle = KnxGroupAddressStyle.ThreeLevel;
            Debug = false;
        }

        internal KnxConnectionConfiguration ConnectionConfiguration { get; private set; }

        /// <summary>
        ///     Get the IPEndPoint instance representing the remote KNX gateway
        /// </summary>
        public IPEndPoint RemoteEndpoint
        {
            get
            {
                return ConnectionConfiguration.EndPoint;
            }
        }

        internal KnxReceiver KnxReceiver { get; set; }

        internal KnxSender KnxSender { get; set; }

        /// <summary>
        ///     Configure this paramenter based on the KNX installation:
        ///     - ThreeLevel: 3-level group address: main/middle/sub (5/3/8 bits)
        ///     - TwoLevel: 2-level group address: main/sub (5/11 bits)
        ///     - Free: free style group address: sub (16 bits)
        ///     Default: ThreeLevel
        /// </summary>
        public KnxGroupAddressStyle GroupAddressStyle { get; set; }

        /// <summary>
        ///     Set to true to receive debug log messages
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        ///     Some KNX Routers/Interfaces might need this parameter defined, some need this to be 0x29.
        ///     Default: 0x00
        /// </summary>
        public byte ActionMessageCode { get; set; }

        /// <summary>
        ///     Start the connection
        /// </summary>
        public abstract void Connect();

        /// <summary>
        ///     Stop the connection
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        ///     Event triggered by implementing class to notify that the connection has been established
        /// </summary>
        internal virtual void Connected()
        {
            _lockManager.UnlockConnection();
            
            try
            {
                KnxConnectedDelegate?.Invoke();
            }
            catch(Exception e)
            {
                Logger.Error(ClassName, e);
            }

            Logger.Debug(ClassName, "KNX is connected. Unlocking send - {0} free locks", _lockManager.LockCount);
        }

        /// <summary>
        ///     Event triggered by implementing class to notify that the connection has been established
        /// </summary>
        internal virtual void Disconnected()
        {
            _lockManager.LockConnection();

            try
            {
                KnxDisconnectedDelegate?.Invoke();
            }
            catch (Exception e)
            {
                Logger.Error(ClassName, e);
            }

            Logger.Debug(ClassName, "KNX is disconnected");
            Logger.Debug(ClassName, "Send locked - {0} free locks", _lockManager.LockCount);
        }

        internal void Event(KnxEventArgs args)
        {
            try
            {
                KnxEventDelegate(this, args);
            }
            catch (Exception e)
            {
                Logger.Error(ClassName, e);
            }

            Logger.Debug(ClassName, $"Device {args.SourceAddress} sent event 0x{string.Join("", args.State.Select(c => ((int)c).ToString("X2")))} to {args.DestinationAddress}");
        }

        internal void Status(KnxStatusArgs args)
        {
            try
            {
                KnxStatusDelegate(this, args);
            }
            catch (Exception e)
            {
                Logger.Error(ClassName, e);
            }

            Logger.Debug(ClassName, $"Device {args.SourceAddress} has status {args.State} for {args.DestinationAddress}");
        }

        /// <summary>
        ///     Set the lock interval between requests sent to the network (in ms)
        /// </summary>
        /// <param name="interval">time in ms for the interval</param>
        public void SetLockIntervalMs(int interval)
        {
            _lockManager.IntervalMs = interval;
        }

        /// <summary>
        ///     Send a bit value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Bit value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, bool data)
        {
            byte[] val;

            try
            {
                val = new[] {Convert.ToByte(data)};
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
        ///     Send a string value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">String value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, string data)
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
        ///     Send an int value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Int value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, int data)
        {
            var val = new byte[2];
            if (data <= 255)
            {
                val[0] = 0x00;
                val[1] = (byte) data;
            }
            else if (data <= 65535)
            {
                val[0] = (byte) data;
                val[1] = (byte) (data >> 8);
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
        ///     Send a byte value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">byte value</param>
        public void Action(KnxAddress address, byte data)
        {
            Action(address, new byte[] {0x00, data});
        }

        /// <summary>
        ///     Send a byte array value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Byte array value</param>
        public void Action(KnxAddress address, byte[] data)
        {
            Logger.Debug(ClassName, "Sending 0x{0} to {1}.", BitConverter.ToString(data), address);

            _lockManager.PerformLockedOperation(() => KnxSender.Action(address, data));

            Logger.Debug(ClassName, "Sent 0x{0} to {1}.", BitConverter.ToString(data), address);
        }

        /// <summary>
        ///     Send a request to KNX asking for specified address current status
        /// </summary>
        /// <param name="address"></param>
        public void RequestStatus(KnxAddress address)
        {
            Logger.Debug(ClassName, "Sending request status to {0}.", address);

            _lockManager.PerformLockedOperation(() => KnxSender.RequestStatus(address));

            Logger.Debug(ClassName, "Sent request status to {0}.", address);
        }

        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, string data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, byte[] data)
        {
            return DataPointTranslator.Instance.FromDataPoint(type, data);
        }

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, string value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, object value)
        {
            return DataPointTranslator.Instance.ToDataPoint(type, value);
        }
    }
}

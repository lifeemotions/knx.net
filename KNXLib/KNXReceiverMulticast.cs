using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXReceiverMulticast : KNXReceiver
    {
        #region constructor
        internal KNXReceiverMulticast(KNXConnection connection, UdpClient udpClient, IPEndPoint localEndpoint) : base (connection)
        {
            this.LocalEndpoint = localEndpoint;
            this.UdpClient = udpClient;
        }
        #endregion

        #region variables
        private IPEndPoint _localEndpoint;
        private IPEndPoint LocalEndpoint
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

        private UdpClient _udpClient;
        private UdpClient UdpClient
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
        #endregion

        #region thread
        internal override void ReceiverThreadFlow()
        {
            try
            {
                byte[] dgram;
                while (true)
                {
                    dgram = UdpClient.Receive(ref this._localEndpoint);
                    ProcessDatagram(dgram);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }
        #endregion
    }
}

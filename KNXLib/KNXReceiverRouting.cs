using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KNXLib
{
    internal class KNXReceiverRouting : KNXReceiver
    {
        #region constructor
        internal KNXReceiverRouting(KNXConnectionRouting connection, UdpClient udpClient, IPEndPoint localEndpoint) : base (connection)
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


        #region datagram processing
        internal override void ProcessDatagram(byte[] dgram)
        {
            try
            {
                ProcessDatagramHeaders(dgram);
            }
            catch (Exception)
            {
                // ignore, missing warning information
            }
        }

        private void ProcessDatagramHeaders(byte[] dgram)
        {
            // HEADER
            KNXDatagram datagram = new KNXDatagram();
            datagram.header_length = (int)dgram[0];
            datagram.protocol_version = dgram[1];
            datagram.service_type = new byte[] { dgram[2], dgram[3] };
            datagram.total_length = (int)dgram[4] + (int)dgram[5];

            byte[] cemi = new byte[dgram.Length - 6];
            Array.Copy(dgram, 6, cemi, 0, dgram.Length - 6);

            base.ProcessCEMI(datagram, cemi);
        }
        #endregion
    }
}

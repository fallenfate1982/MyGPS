using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GTSBizObjects
{
    public interface IGPSTracker
    {
        void RecievedMessage(byte[] rawMessage, int length);

        /// <summary>
        /// TODO - this should not be used here! Meed something more generic
        /// </summary>
        /// <param name="client"></param>
        void SendMessages(TcpClient client);
    }

    public interface IGPSTrackerWithOutputs
    {
        void SetOutputs();
    }
}

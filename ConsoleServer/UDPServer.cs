using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

using GTSBizObjects;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace ConsoleServer
{
    public class UDPServer
    {
        private UdpClient udpListener;
        IPEndPoint groupEP;
        private Thread listenThread;

        public UDPServer()
        {
            int serverPort;
            int.TryParse(System.Configuration.ConfigurationSettings.AppSettings["Server_Port"], out serverPort);
            serverPort = serverPort == 0 ? 1111 : serverPort;

            this.udpListener = new UdpClient(serverPort);
            groupEP = new IPEndPoint(IPAddress.Any, serverPort);

            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            Utilities.writeLine("Debug 0: UDP Console application started..... starting listing!");
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
            int threadCount = 0;

            while (true)
            {
                //blocks until a client has connected to the server
                byte[] bytes = this.udpListener.Receive(ref groupEP);
                Utilities.writeLine("Debug 1: UDP Message Recieved");

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                threadCount++;
                // at this point we'll need to add code to count and record the threads better so we can debug each separatly!


                Utilities.writeLine("Debug 2: Initiating handling of client data");
                Utilities.writeLine("Debug 2.2: Thread Count: " + threadCount);

                clientThread.Start(bytes);
            }
        }

        private void HandleClientComm(object udpMessage)
        {
            // this is for testing TODO - we need to change this to work with sending back data as udp
            // only done to test UDP reciving
            TcpClient client = new TcpClient();


            byte[] bytes = (byte[])udpMessage;
          
            IGPSTracker _tracker = null;

            if (bytes.Length == 0)
            {
                Utilities.writeLine("Dubug 4: Sorry, no data read, disconnecting");
                //the client has disconnected from the server
            }
            else
            {
                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                Utilities.writeLine("Debug 5: " + encoder.GetString(bytes, 0, bytes.Length));

                try
                {
                    if (_tracker == null) _tracker = Management.GetGPSTracker(bytes, bytes.Length, client);
                    _tracker.RecievedMessage(bytes, bytes.Length);
                }
                catch (Exception e)
                {
                    Utilities.writeLine("Error 3: " + e.Message);
                    Utilities.writeLine("Error 4: " + e.StackTrace);
                    Utilities.writeLine("Error data: " + encoder.GetString(bytes, 0, bytes.Length));
                }
            }
        }
    }
}

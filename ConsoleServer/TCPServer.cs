using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

using GTSBizObjects;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

    namespace TCPServer
    {
        public class TCPServer
        {
            private TcpListener tcpListener;
            private Thread listenThread;

            public TCPServer()
            {
                int serverPort;
                int.TryParse(System.Configuration.ConfigurationSettings.AppSettings["Server_Port"], out serverPort);

                this.tcpListener = new TcpListener(IPAddress.Any, serverPort==0?1111:serverPort);
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                Utilities.writeLine("Debug 0: TCP Console application started..... starting listing!");
                this.listenThread.Start();
            }

            private void ListenForClients()
            {
                this.tcpListener.Start();
                int threadCount = 0;

                while (true)
                {
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    Utilities.writeLine("Debug 1: Client connected");
                    
                    //create a thread to handle communication 
                    //with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                threadCount++;
                    // at this point we'll need to add code to count and record the threads better so we can debug each separatly!


                    Utilities.writeLine("Debug 2: Initiating handling of client data");
                    Utilities.writeLine("Debug 2.2: Thread Count: "+threadCount);

                    clientThread.Start(client);
                }
            }

            private void HandleClientComm(object client)
            {
                TcpClient tcpClient = (TcpClient)client;
                NetworkStream clientStream = tcpClient.GetStream();


                byte[] message = new byte[4096];
                int bytesRead;

                while (true)
                {
                    bytesRead = 0;
                    IGPSTracker _tracker = null;

                    try
                    {
                        //blocks until a client sends a message
                        bytesRead = clientStream.Read(message, 0, 4096);
                        Utilities.writeLine("Debug 3: Time: "+DateTime.Now+ " data read: "+bytesRead);
                    }
                    catch(Exception e)
                    {
                        //a socket error has occured
                        Utilities.writeLine("Error 1: "+e.Message);
                        Utilities.writeLine("Error 2: "+e.StackTrace);
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        Utilities.writeLine("Dubug 4: Sorry, no data read, disconnecting");
                        //the client has disconnected from the server
                        break;
                    }

                    //message has successfully been received
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    Utilities.writeLine("Debug 5: "+encoder.GetString(message, 0, bytesRead));

                    try
                    {
                        if (_tracker == null) _tracker = Management.GetGPSTracker(message, bytesRead, client);
                        _tracker.RecievedMessage(message, bytesRead);
                        _tracker.SendMessages(tcpClient);
                    }
                    catch (Exception e)
                    {
                         Utilities.writeLine("Error 3: "+e.Message);
                        Utilities.writeLine("Error 4: "+e.StackTrace);
                        Utilities.writeLine("Error data: " + encoder.GetString(message, 0, bytesRead));
                    }

                }

            Utilities.writeLine("Debug 100: Closing TCP Client");
            tcpClient.Close();
            
            }

    }
}

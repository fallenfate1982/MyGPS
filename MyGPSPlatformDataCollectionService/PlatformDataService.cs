using GTSBizObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyGPSPlatformDataCollectionService
{
    public partial class PlatformDataService : ServiceBase
    {

        private TcpListener tcpListener;
        private Thread listenThread;

        public PlatformDataService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("MyGPS Platform Data Sink Service is starting.");

            int serverPort;
            int.TryParse(ConfigurationSettings.AppSettings["Server_Port"], out serverPort);

            tcpListener = new TcpListener(IPAddress.Any, serverPort == 0 ? 1111 : serverPort);

            if(listenThread == null)
                listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }

        protected override void OnStop()
        {
            // Add code here to perform any tear-down necessary to stop your service.
            eventLog1.WriteEntry("MyGPS Platform Data Sink Service is stopping.");

            // Stop the thread.
            listenThread.Abort();
        }


        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();
                System.Diagnostics.Debug.WriteLine("Client connected");
                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                System.Diagnostics.Debug.WriteLine("Initiating handling of client data");
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
                    System.Diagnostics.Debug.WriteLine("Time: " + DateTime.Now + " data read: " + bytesRead);
                }
                catch (Exception e)
                {
                    //a socket error has occured
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);

                    break;
                }

                if (bytesRead == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Sorry, no data read, disconnecting");
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));

                try
                {
                    if (_tracker == null) _tracker = Management.GetGPSTracker(message, bytesRead, client);
                    _tracker.RecievedMessage(message, bytesRead);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine(e.StackTrace);

                    eventLog1.WriteEntry("MyGPS Error processing received message. :"+e.Message,EventLogEntryType.Error);
                }

            }

            tcpClient.Close();
        }

    }
}

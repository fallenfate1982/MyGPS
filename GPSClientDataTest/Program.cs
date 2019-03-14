using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;
using System.Data.Objects;


using GTSDataStorage;
using GTSBizObjects;
using System.Collections.Generic;

public class ClientProgram
{
static public void Main( string[] Args )
    {
       
         
        Utilities.writeLine("Debug 0: Starting!");

        GPSTrackerEntities1 context = new GPSTrackerEntities1();
        DateTime startTime, endTime;

        TcpClient client = new TcpClient();

    // Enter information here for traker and period you want to mimic

        startTime = new DateTime(2016, 5,18, 15, 22, 00);
        endTime = new DateTime(2016, 5, 18, 15, 25, 00);

        string trackerId = "836167340";
        string myReplayId = "3333333333ffff"; // "3333333333ffff"; // for 310/300 replay 333333333333333

        string trackerType = "310"; // TODO, we can pull this directly from the DB

        List<LocationMessages> messages = 
            context.LocationMessages.Where(t => t.TrackerId == trackerId).Where(r=> r.MessageTime > startTime).Where(s=> s.MessageTime < endTime)
            .OrderBy(c=>c.MessageTime).ToList();
        Utilities.writeLine("Debug 1: Got "+ messages.Count+" to send");

        // this loop will send the data to the server 

        //157.56.160.61
        //192.168.1.106
        //IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("40.83.186.167"), 1111);
        string data = "$$\0`?ag3r???U114452.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*64|0.7|11|2000|0003,0002?\r\n$$\0`?ag3r???U114502.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*60|0.7|11|2000|0003,0002??\r\n$$\0`?ag3r???U114512.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*61|0.7|11|2000|0003,0002/?\r\n$$\0`?ag3r???U114522.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*62|0.7|11|2000|0003,0002??\r\n$$\0`?ag3r???U114532.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*63|0.7|11|2000|0003,0001#?\r\n$$\0`?ag3r???U114543.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*65|0.7|11|2000|0003,0001e)\r\n$$\0`?ag3r???U114553.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*64|0.7|11|2000|0003,0001?\r\n$$\0`?ag3r???U114603.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*62|0.7|11|2000|0004,0002y%\r\n$$\0`?ag3r???U114613.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*63|0.7|11|2000|0004,0002?\r\n$$\0`?ag3r???U114623.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*60|0.7|11|2000|0004,0002En\r\n$$\0`?ag3r???U114634.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*66|0.7|11|2000|0004,0002-\r\n$$\0`?ag3r???U114644.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*61|0.7|11|2000|0004,0002??\r\n$$\0`?ag3r???U114654.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*60|0.7|11|2000|0002,0002?\r\n$$\0`?ag3r???U114704.000,A,1018.9306,N,06127.3073,W,0.00,,051114,,,A*64|0.7|11|2000|0002,0002??\r\n";
    


        client.Connect(serverEndPoint);

        NetworkStream clientStream = client.GetStream();

        ASCIIEncoding encoder = new ASCIIEncoding();

        //byte[] buffer = encoder.GetBytes("$$ V?ag4???U015039.000,V,1012.4115,N,06132.1598,W,0.00,,090211,,,A*67|1.5|17|2000??");


        foreach (LocationMessages msg in messages)
        {
            Utilities.writeLine("Debug 3: Sending ");

            data = msg.RawTextData;
            byte[] buffer = encoder.GetBytes(data);

            if (trackerType != "340")
            {
                #region Case type is 310/300
                // 310 /300 needs to convert to replace id and some other value we can't remember why its being changed here

                

                string _hex = Utilities.ByteArrayToString(buffer, buffer.Length);

                StringBuilder abuild = new StringBuilder(_hex);
                abuild.Remove(8, 14);
                abuild.Insert(8, myReplayId);

                abuild.Remove(22, 4);
                abuild.Insert(22, "9955");

                _hex = abuild.ToString();


                // convert back to byte[]

                buffer = Utilities.StringToByteArray(_hex);
                #endregion
            }

            else {
                #region Case type is 340
                // We just do a direct swap on the string
                StringBuilder abuild = new StringBuilder(data);
                abuild.Remove(7, 15);
                abuild.Insert(7, myReplayId);

                data = abuild.ToString();

                buffer = encoder.GetBytes(data);
                #endregion
            }
            // _hex = Utilities.ByteArrayToString(buffer, buffer.Length);
            //_trackerId = _hex.Substring(8, 14);

            //VT300LocationMessage msgTest = new VT300LocationMessage(trackerId,buffer,buffer.Length);


            // Utilities.ByteArrayToString(buffer, buffer.Length);

            clientStream.Write(buffer, 0, buffer.Length);

            System.Threading.Thread.Sleep(5000);
        }


    clientStream.Close();

        #region old code
//        TcpClient socketForServer;
//try
//{
//    socketForServer = new TcpClient("127.0.0.1", 3333);
//}
//catch
//{
//System.Diagnostics.Debug.WriteLine(
//"Failed to connect to server at {0}:999", "localhost");
//return;
//}
//NetworkStream networkStream = socketForServer.GetStream();
//System.IO.StreamReader streamReader =
//new System.IO.StreamReader(networkStream);
//System.IO.StreamWriter streamWriter =
//new System.IO.StreamWriter(networkStream);
//try
//{
//string outputString;
//// read the data from the host and display it
//{
////outputString = streamReader.ReadLine();
////System.Diagnostics.Debug.WriteLine(outputString);
//streamWriter.WriteLine("Client Message");
//System.Diagnostics.Debug.WriteLine("Client Message");
//streamWriter.Flush();
//}
//}
//catch
//{
//System.Diagnostics.Debug.WriteLine("Exception reading from Server");
//}
//// tidy up
//networkStream.Close();
        #endregion
    }
}
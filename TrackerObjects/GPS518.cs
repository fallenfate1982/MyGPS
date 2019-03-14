using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GTSBizObjects
{
    public class GPS518:IGPSTracker
    {

        #region Stuff for UI side

        protected string trackerId;
        protected string name;
        protected string description;
        protected string status;// definitly needs to be updated as an enum
        protected string serial;
        protected string password;
        protected string authorizedNumbers; // xml string

        public GPS518()
        { }


        /// <summary>
        /// TODO - this should not be used here! Meed something more generic
        /// </summary>
        /// <param name="client"></param>
        public void SendMessages(TcpClient client)
        {
            Utilities.writeLine("Debug SEND DATA 1: Getting Pending Messages for " + this.trackerId);
            IQueryable<GTSDataStorage.TrackerCommandLog> messages = Management.GetTrackerCommands(this.TrackerId);

            Utilities.writeLine("Debug SEND DATA 2: There are " + messages.Count() + " Pending Messeags");

            foreach (GTSDataStorage.TrackerCommandLog m in messages)
            {
                Utilities.writeLine("Debug SEND DATA 3: Sending Message '" + m.Message + "' to tracker: " + this.trackerId);
                Utilities.SendCommand(client, m.Message);
            }
            Utilities.writeLine("Debug SEND DATA 4: Done sending Messages for " + this.trackerId);
        }

        public GPS518(string _trackerId, string _name, string _desc, string _status, 
            string _serial, string _password, string _authNumbers)
        {
            trackerId = _trackerId;
            name = _name;
            description = _desc;
            status = _status;
            serial = _serial;
            password = _password;
            authorizedNumbers = _authNumbers;
        }

        public string TrackerId
        {
            get
            {
                return trackerId;
            }
        }

        #endregion

        public void RecievedMessage(byte[] message, int length)
        {
            // get the tracker id
            ASCIIEncoding encoder = new ASCIIEncoding();
            string rawTextData = encoder.GetString(message, 0, length);
            string trackerId = rawTextData.Substring(21, 9);

            // need to update this to also record messages if there are multiple in the report.
            GTSLocationMessage locationMessage = new GPS518LocationMessage(trackerId, message, length);
            if(locationMessage.IsValid)// only store valid readings
                GTSBizObjects.Management.SaveTrackerInformation(locationMessage);

            
        }


    }
}

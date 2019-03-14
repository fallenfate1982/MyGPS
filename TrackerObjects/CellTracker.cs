using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GTSBizObjects
{
    public class CellTracker:IGPSTracker
    {
        
        #region Stuff for UI side

        protected string trackerId;
        protected string name;
        protected string description;
        protected string status;// definitly needs to be updated as an enum
        protected string phoneNumber;
        protected string password;
        protected string authorizedNumbers; // xml string
        protected DateTime? lastUpdate;
        protected double? lastLat;
        protected double? lastLong;

        public CellTracker()
        { }

        public CellTracker(string _trackerId, string _name, string _desc, string _status,
            string _phNumber, string _password, string _authNumbers, DateTime? lastUp, double? lastlat, double? lastlon)
        {
            trackerId = _trackerId;
            name = _name;
            description = _desc;
            status = _status;
            phoneNumber = _phNumber;
            password = _password;
            lastUpdate = lastUp;
            lastLat = lastlat;
            lastLong = lastlon;
        }

        public string TrackerId
        {
            get
            {
                return trackerId;
            }
        }

        public DateTime? LastUpdate
        {
            set
            {
                lastUpdate = value;
            }
            get
            {
                return lastUpdate;
            }
        }

        public double? LastLat
        {
            set
            {
                lastLat = value;
            }
            get
            {
                return lastLat;
            }
        }

        public double? LastLon
        {
            set
            {
                lastLong = value;
            }
            get
            {
                return lastLong ;
            }
        }

        #endregion

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

        public bool RecievedMessage(NameValueCollection message)
        {
            // need to update this to also record messages if there are multiple in the report.
            CellLocationMessage locationMessage = new CellLocationMessage(message);

            /// will only save if being used as a full tracker
            if(locationMessage.IsValid){// only store valid readings
                try
                {
                    GTSBizObjects.Management.UpdateTrackerInformation(locationMessage); 
                }
                catch (Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Need to Update this method to convert byte[] to NameValueCollection!!!
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        public void RecievedMessage(byte[] message, int length)
        { throw new Exception("Method CellTracker.RecievedMessage not implemented!!!!"); }
    }
}

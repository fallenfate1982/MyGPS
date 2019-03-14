using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

using GTSDataStorage;

namespace GTSBizObjects
{
    public class Management
    {


        public static CellTracker GetCellTracker(string trackerId)
        {
            CellTracker tracker = null;

            GPSTrackerEntities1 context = new GPSTrackerEntities1();


            // Need to not allow disabled statuses here
            Trackers _trackersDB =
                context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();

            if (_trackersDB != null)
            {
                tracker = new CellTracker(_trackersDB.Id, _trackersDB.Name, _trackersDB.Description, _trackersDB.Status, _trackersDB.Id,
                    _trackersDB.Password, _trackersDB.AuthorizedNumbers, _trackersDB.LastUpdate, _trackersDB.LastLat, _trackersDB.LastLon);
            }

            return tracker;
        }

        /// <summary>
        /// Checks to see if the data coming in is for an actual tracker of ours
        /// </summary>
        /// <param name="trackerId"></param>
        /// <returns></returns>
        public static bool TrackerExist(string trackerId)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            //get the Tracker (this is probably done elsewhere, so it's an extra hit, but it's less work than currently being done
            Trackers _tracker = context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();

            return _tracker !=null;
        }

        public static TrackerInfo GetTrackerInfo(string trackerId)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            //get the Tracker (this is probably done elsewhere, so it's an extra hit, but it's less work than currently being done
            Trackers _tracker = context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();

            TrackerInfo retVal = new TrackerInfo(_tracker);

            return retVal;
        }
       /// <summary>
       /// Gets the type of the GPS tracker that connected
       /// </summary>
       /// <param name="message"></param>
       /// <param name="bytesRead"></param>
       /// <param name="client"></param>
       /// <returns></returns>
        public static IGPSTracker GetGPSTracker(byte[] message, int bytesRead, object client)
        {
            Utilities.writeLine("Debug 6: Processing GPS tracker type");

            IGPSTracker _tracker = null;
            char _1stChar = Convert.ToChar(message[0]);

            // Trim leading nulls from byte array .... some versions of the firmware needs this!
            if (_1stChar == '\0')
            {
                byte[] theNew = new byte[message.Length];

                Buffer.BlockCopy(message, 1, theNew, 0, message.Length - 1);

                message = theNew;
            }


            ASCIIEncoding encoder = new ASCIIEncoding();
            Utilities.writeLine("Debug 6.2: " + encoder.GetString(message, 0, bytesRead));

            _1stChar = Convert.ToChar(message[0]);
            char _2ndChar = Convert.ToChar(message[1]);
            char _3rdChar = Convert.ToChar(message[2]);
            bool isInRange = _3rdChar >= 'A' && _3rdChar <= 'z';// range given in the spec for VT340
            Utilities.writeLine("Debug 6.5: 1st char: " + _1stChar + " 2nd char: " + _2ndChar + " 3rd char: " + _3rdChar);

            if (_1stChar == '(')
            {
                Utilities.writeLine("Debug 7: Type is GPS518");
                _tracker = new GPS518();
            }
            else
                if (_1stChar == '$' && _2ndChar == '$' && !isInRange)
            {
                Utilities.writeLine("Debug 8: Type is VT300");
                _tracker = new VT300(client);
            }
            else
                if (_1stChar == '$' && _2ndChar == '$' && isInRange)
            {
                Utilities.writeLine("Debug 9: Type is VT340");
                _tracker = new VT340(client);
            }
            else
                if (_1stChar == '$')
            {
                Utilities.writeLine("Debug 8: Type is VT300 - one of the weird ones");
                _tracker = new VT300(client);
            }
            else
            {
                Utilities.writeLine("Debug 9.5: 1st char: " + _1stChar + " 2nd char: " + _2ndChar + " 3rd char: " + _3rdChar);
                Utilities.writeLine("Debug 10: Type of tracker is unidentified");
            }
            
            return _tracker;
        }

        // this should be returning "IGPSTrackers" but its not Or another more full object
        public static List<TrackerInfo> GetGPSTrackersForUser(Guid _userId)
        {
            List<TrackerInfo> _trackers = new List<TrackerInfo>();

            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            IQueryable<TrackerUser> _trackersDB = 
                context.TrackerUser.Where(t => t.UserId == _userId);

            

            foreach (TrackerUser _t in _trackersDB)
            {
                // need to get object representing the tracker information
                Trackers _trackerInfo = context.Trackers.Where(p => p.Id == _t.TrackerId).FirstOrDefault();

                if (_trackerInfo != null)
                {
                    _trackers.Add(new TrackerInfo(_trackerInfo));
                }
            }

            return _trackers;

        }

        

        public static void SaveTrackerInformation(GTSLocationMessage message)
        {
            GTSLocationMessage prevMessage = getLastLocationMessage(message.TrackerID);
            GPSTrackerEntities1 context = new GPSTrackerEntities1();
            LocationMessages msg;
            double mileage;

                // Update total milage TODO: we need to add in the engine on check here!!!! After we put engine on as a standard on all vehicles
                if(message.Speed != 0){
                    //Calculate what the mileage on the tracker should be

                    try
                    {
                        //get the Tracker (this is probably done elsewhere, so it's an extra hit)
                        Trackers _tracker = context.Trackers.Where(t => t.Id == message.TrackerID).FirstOrDefault();

                        //if we didnt have a mileage reading before, get it from Tracker if it exists, else assume it's 0 and start counting from there.
               
                        double mileageChange;

                        if (_tracker.Mileage.HasValue && _tracker.Mileage.Value > 0.0)
                        {
                            mileageChange = Utilities.CalculateDistanceBetween(_tracker.LastLat.Value, _tracker.LastLon.Value, message.Latitude.Decimal, message.Longitude.Decimal);
                           
                            mileage = _tracker.Mileage.Value + mileageChange;
                            //update tracker mileage value too
                        }
                        else
                        {
                            mileage = 0;

                        }
                        _tracker.Mileage = mileage;

                        //saving here to make sure we don't throw off anything else later down by doing a batch save
                        // this is a wasted DB hit though.
                        context.SaveChanges();
                    
                    }
                    catch (System.Exception ex)
                    {
                        //swallow for now
                        mileage = 0.0;
                    }
            }
            else
            {
                mileage = prevMessage != null ? prevMessage.Mileage.GetValueOrDefault() : 0 ;
            }

            if (prevMessage == null || prevMessage.Speed!=0 || message.Speed != 0 || 
                Math.Abs(prevMessage.Latitude.Decimal-message.Latitude.Decimal)>0.001 || 
                Math.Abs(prevMessage.Longitude.Decimal-message.Longitude.Decimal)>0.001 ||
                prevMessage.Status != message.Status)
            {
                // add new message into messages
                msg = LocationMessages.CreateLocationMessages(1, message.RawLocationMessageData, message.Latitude.Degrees
                    , message.Latitude.Minutes, message.Latitude.Seconds, message.Longitude.Degrees, message.Longitude.Minutes
                    , message.Longitude.Seconds, message.ServerRecordedDateTime, message.TrackerID,
                    message.Latitude.Heading.ToString(), message.Longitude.Heading.ToString(), message.Speed, message.IdleTime,
                    message.Direction, message.Status, message.ClientRecordedDateTime, message.HDOP, message.VDOP,
                    message.Altitude, message.Status, message.Input1, message.Input2,
                    message.DInput1, message.DInput2, message.DInput3, message.DInput4, message.DInput5, mileage);
                
                context.AddToLocationMessages(msg);
                int affectedRows = context.SaveChanges();
                message.Id = msg.Id;
            }
            else
            {
                // update time on previous message
                //weird..... must be some better way of architechting this...... as well should be exception handeled!!!

                //vs: If it's in the same spot, we don't need to do anything with mileage

                TimeSpan span = message.ServerRecordedDateTime.Subtract(prevMessage.ServerRecordedDateTime); 
                prevMessage.IdleTime = span.Ticks;

                msg = context.LocationMessages.Where(m => m.Id == prevMessage.Id).First();
                message.IdleTime= msg.IdleTime = prevMessage.IdleTime;

                context.SaveChanges();
            }
            if (msg != null)
                UpdateTrackerInformation(msg);

            // Events pulled from information in the Location Messages
            // All Hooks so far for events are here - lets keep it so until needed
            List<Events.Event> events = new Events.Management().GetVT310eEvents(message, prevMessage);
            new Events.Management().SaveEvents(events);
            Utilities.writeLine("Debug 30: Message Saved!");
        }

        /// <summary>
        /// Needs to be updated to update general tracking information
        /// </summary>
        /// <param name="?"></param>
        public static void UpdateTrackerInformation(GTSLocationMessage message)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();
            
            Trackers _tracker =
               context.Trackers.Where(t => t.Id == message.TrackerID).FirstOrDefault();
            _tracker.LastUpdate = DateTime.Now;
            _tracker.LastLat = message.Latitude.Decimal;
            _tracker.LastLon = message.Longitude.Decimal;

            if (message.Id != 0)
                _tracker.LastLocationId = message.Id;


            context.SaveChanges();

        }

        /// <summary>
        /// Needs to be updated to update general tracking information
        /// </summary>
        /// <param name="?"></param>
        public static void UpdateTrackerInformation(LocationMessages message)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            Trackers _tracker =
               context.Trackers.Where(t => t.Id == message.TrackerId).FirstOrDefault();
            _tracker.LastUpdate = DateTime.Now;
            _tracker.LastLat = (new Latitude() { Degrees = message.LatDegrees, Minutes = message.LatMinutes, Seconds = message.LatSeconds, Heading = message.LatitudeHeading[0] }).Decimal;
            _tracker.LastLon = (new Longitude() { Degrees = message.LngDegrees, Minutes = message.LngMinutes, Seconds = message.LngSeconds, Heading = message.LongitudeHeading[0] }).Decimal;

            if (message.Id != 0)
                _tracker.LastLocationId = message.Id;


            context.SaveChanges();

        }

        private static GTSLocationMessage getLastLocationMessage(string trackerId)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            GTSLocationMessage retVal = null;
            //get the Tracker (this is probably done elsewhere, so it's an extra hit, but it's less work than currently being done
            Trackers _tracker = context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();

            LocationMessages message;

            try
            {
                //try to get the Location Message that's listed as the last known
                if (_tracker.LastLocationId.HasValue && _tracker.LastLocationId != 0)
                {
                    message = context.LocationMessages.Where(m => m.Id == _tracker.LastLocationId.Value).First();
                    retVal = CreateLocationMessage(message);
                }
                else
                {
                    message = context.LocationMessages.Where(m => m.TrackerId == trackerId)
                        .OrderByDescending(x => x.ServerTime).FirstOrDefault();
                    if (message != null)
                    {
                        retVal = CreateLocationMessage(message);
                        //Update the Tracker object so that we know the last Location Id
                        if (retVal != null)
                            UpdateTrackerInformation(retVal);
                    }
                }
            }
            catch
            {
            }

            return retVal;
        }

        public static GTSLocationMessage GetLocationMessageById(int messageId)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            GTSLocationMessage retVal = null;

            LocationMessages message = context.LocationMessages.Where(t => t.Id == messageId).FirstOrDefault();
            TrackerInfo info = null;
            if(message!=null) info = GetTrackerInfo(message.TrackerId);
          
            retVal = CreateLocationMessage(message);
            retVal.TrackerDetail = info;
            return retVal;
        }

        private static GTSLocationMessage getLastLocationMessageWithTrackerInfo(Trackers _tracker)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();
            GTSLocationMessage retVal = null;
            LocationMessages message;

            try
            {
                //try to get the Location Message that's listed as the last known
                if (_tracker.LastLocationId.HasValue)
                {
                    message = context.LocationMessages.Where(m => m.Id == _tracker.LastLocationId.Value).First();
                    retVal = CreateLocationMessage(message, _tracker);
                }
                else
                {
                    message = context.LocationMessages.Where(m => m.TrackerId == _tracker.Id)
                        .OrderByDescending(x => x.ServerTime).FirstOrDefault();
                    if (message != null)
                    {
                        retVal = CreateLocationMessage(message, _tracker);
                        //Update the Tracker object so that we know the last Location Id
                        if (retVal != null)
                        {
                            UpdateTrackerInformation(retVal);
                        }
                    }
                }
            }
            catch
            {
            }

            return retVal;
        }

        //public static GTSLocationMessage CreateLocationMessage(byte[] message, int bytesRead)
        //{
        //    GTSLocationMessage retVal = new GTSLocationMessage(message, bytesRead);

        //    return retVal;
        //}

        public static GTSLocationMessage CreateLocationMessage(LocationMessages message)
        {
            GTSLocationMessage retVal = new GTSLocationMessage(message.Id, message.LatDegrees, message.LatMinutes,
                message.LatSeconds, message.LngDegrees, message.LngMinutes, message.LngSeconds,
                message.RawTextData, message.TrackerId, message.ServerTime,
                message.LatitudeHeading[0], message.LongitudeHeading[0], message.Speed, message.IdleTime, message.MessageTime
            , message.HDOP, message.Status, message.Altitude, message.Direction, message.VDOP, message.Status, message.Input1.HasValue ? message.Input1.Value : 0.0,
            message.Input2.HasValue ? message.Input2.Value : 0.0, 
            message.DigiInput1, message.DigiInput2, message.DigiInput3, message.DigiInput4, message.DigiInput5, message.Mileage.HasValue? message.Mileage.Value: 0.0);

            return retVal;
        }


        public static GTSLocationMessage CreateLocationMessage(LocationMessages message, Trackers info)
        {
            GTSLocationMessage retVal = new GTSLocationMessage(message.Id, message.LatDegrees,message.LatMinutes,
                message.LatSeconds,message.LngDegrees,message.LngMinutes,message.LngSeconds,
                message.RawTextData,message.TrackerId,message.ServerTime,
                message.LatitudeHeading[0],message.LongitudeHeading[0],message.Speed,message.IdleTime, message.MessageTime
            , message.HDOP, message.Status, message.Altitude, message.Direction, message.VDOP, message.Status, message.Input1.HasValue ? message.Input1.Value : 0.0,
            message.Input2.HasValue ? message.Input2.Value : 0.0,
            message.DigiInput1, message.DigiInput2, message.DigiInput3, message.DigiInput4, message.DigiInput5, message.Mileage.HasValue ? message.Mileage.Value : 0.0);

            retVal.TrackerDetail = new TrackerInfo(info);

            return retVal;
        }

        public static GTSLocationMessageLite CreateLocationMessageLite(LocationMessages message)
        {
            GTSLocationMessageLite retVal = new GTSLocationMessageLite(message.Id, message.LatDegrees, message.LatMinutes,
                message.LatSeconds, message.LngDegrees, message.LngMinutes, message.LngSeconds,
                message.RawTextData, message.TrackerId, message.ServerTime,
                message.LatitudeHeading[0], message.LongitudeHeading[0], message.Speed, message.IdleTime, message.MessageTime,
                message.HDOP, message.Status, message.Altitude, message.Direction, message.VDOP, message.Status, 
                message.Input1.HasValue?message.Input1.Value:0.0,
                message.Input2.HasValue ? message.Input2.Value : 0.0, 
            message.DigiInput1, message.DigiInput2, message.DigiInput3, message.DigiInput4, message.DigiInput5);

            return retVal;
        }

        public static GTSLocationMessageCollection CreateGTSLocationMessages(
            string id,DateTime sdateTime, DateTime edateTime)
        {
            GTSLocationMessageCollection collection = new GTSLocationMessageCollection();

            GPSTrackerEntities1 dbContext = new GPSTrackerEntities1();

            DateTime defaultDateTime = new DateTime(1900,01,01);
            sdateTime = sdateTime == DateTime.MinValue? defaultDateTime :sdateTime;
            edateTime = edateTime == DateTime.MinValue? defaultDateTime:edateTime;

            // used >= sdateTime here so that the system will update time when tracking with the javascript
            // may want to implement this differently for efficiency
            IOrderedQueryable<LocationMessages> messages= 
                dbContext.LocationMessages.Where( m => m.TrackerId == id 
                    && (sdateTime==defaultDateTime|| m.ServerTime>=sdateTime)
                    && ( edateTime==defaultDateTime || m.ServerTime<edateTime))
                .OrderBy(message=>message.ServerTime);
            

            foreach(LocationMessages message in messages)
            {
                collection.Add(CreateLocationMessage(message));
            }

            return collection;
        }

        public static GTSLocationMessageCollection CreateGTSLocationMessages(Guid userId)
        {
            GTSLocationMessageCollection collection = new GTSLocationMessageCollection();

            GPSTrackerEntities1 dbContext = new GPSTrackerEntities1();

       

            IQueryable<TrackerUser> trackers = dbContext.TrackerUser.Where(m => m.UserId == userId);

            foreach(TrackerUser _tu in trackers)
            {
                IQueryable <LocationMessages> _messages =
                    dbContext.LocationMessages.Where(p => p.TrackerId == _tu.TrackerId );

                // need to get object representing the tracker information
                Trackers _trackerInfo = dbContext.Trackers.Where(p => p.Id == _tu.TrackerId).First();

                if (_messages.Count()>0)
                {
                    DateTime _maxTime = _messages.Max(p => p.ServerTime);
                    LocationMessages _message = _messages.Where(p => p.ServerTime == _maxTime).First();
                    collection.Add(CreateLocationMessage(_message,_trackerInfo));
                }
            }

            return collection;
        }

        public static List<GTSLocationMessage> CreateCurrentTrackersByUser(Guid userId)
        {
            List<GTSLocationMessage> collection = new List<GTSLocationMessage>();

            GPSTrackerEntities1 dbContext = new GPSTrackerEntities1();



            IQueryable<TrackerUser> trackers = dbContext.TrackerUser.Where(m => m.UserId == userId);

            try
            {

                // this seemes like something can be optimized here. 
                foreach (TrackerUser _tu in trackers)
                {
                    // IQueryable<LocationMessages> _messages =
                    //  dbContext.LocationMessages.Where(p => p.TrackerId == _tu.TrackerId);// are we getting all location messages for a tracker just to get the first?

                    // need to get object representing the tracker information
                    Trackers _trackerInfo = dbContext.Trackers.Where(p => p.Id == _tu.TrackerId).FirstOrDefault();// need some exception handling!!!

                    

                    // if (_messages.Count() > 0)
                    // {
                    // DateTime _maxTime = _messages.Max(p => p.ServerTime);
                    //LocationMessages _message = _messages.Where(p => p.ServerTime == _maxTime).First();

                    //Commented out below: VS 6/11/2014
                    //LocationMessages _message =
                    //    (from m in dbContext.LocationMessages
                    //    where m.TrackerId == _tu.TrackerId
                    //    orderby m.ServerTime descending
                    //    select m).FirstOrDefault();
                    //if (_message != null && _trackerInfo != null)
                    //{
                    //    collection.Add(CreateLocationMessage(_message, _trackerInfo));
                    //}
                    if (_trackerInfo != null)
                    {
                        if (!_trackerInfo.LastUpdate.HasValue) continue;
                        GTSLocationMessage gtsLM = getLastLocationMessageWithTrackerInfo(_trackerInfo);
                        if (gtsLM != null)
                            collection.Add(gtsLM);
                        else
                        {

                        }
                    }
                    // }
                }
            }
            catch (System.Exception ex)
            {
                int x = 1;
            }

            return collection;
        }




        public static List<GTSLocationMessageLite> GetTrackerInfoByDate(
            string id, DateTime sdateTime, DateTime edateTime)
        {
            List<GTSLocationMessageLite> collection = new List<GTSLocationMessageLite>();

            GPSTrackerEntities1 dbContext = new GPSTrackerEntities1();

            DateTime defaultDateTime = new DateTime(1900, 01, 01);
            sdateTime = sdateTime == DateTime.MinValue ? defaultDateTime : sdateTime;
            edateTime = edateTime == DateTime.MinValue ? defaultDateTime : edateTime;

            // used >= sdateTime here so that the system will update time when tracking with the javascript
            // may want to implement this differently for efficiency
            IOrderedQueryable<LocationMessages> messages =
                dbContext.LocationMessages.Where(m => m.TrackerId == id
                    && (sdateTime == defaultDateTime || m.ServerTime >= sdateTime)
                    && (edateTime == defaultDateTime || m.ServerTime < edateTime))
                .OrderBy(message => message.ServerTime);

            // need to get object representing the tracker information
            Trackers _trackerInfo = dbContext.Trackers.Where(p => p.Id == id).First();


            foreach (LocationMessages message in messages)
            {
                collection.Add(CreateLocationMessageLite(message));
            }

            return collection;
        }

        // Method that shares the tracker information with Security Personel Assigned
        // Currently First Response TODO: Update to assign response team based on location or something 

        public static bool AlertSecurity(string trackerId)
        {
            // First response User Id 172D2D4A-8711-48EC-9ADE-D0660D4568E0

            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            try
            {
                TrackerUser tu = TrackerUser.CreateTrackerUser(1, trackerId, new Guid("172D2D4A-8711-48EC-9ADE-D0660D4568E0"), true);

                context.AddToTrackerUser(tu);
                context.SaveChanges();
            }
            catch (System.Exception e)
            {
                ExceptionHandler.HandleGeneralException(e);
                return false;
            }
            return true;
        }

        public static GPSTrackerOutput GetTrackerOutput(string id, int number)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            IQueryable<TrackerOutput> _trackerOutputs =
                context.TrackerOutputs.Where(t => (t.TrackerId == id) && (t.Number == number));
            
            //If it's not in the DB, we'll generate an object that basically says 'leave everything as it is'. Code higher up will know
            //that we shouldn't even send commands for this.
            if ((_trackerOutputs == null)||(_trackerOutputs.Count() == 0))
            {
                return new GPSTrackerOutput(id, number, OutputState.Previous, OutputState.Previous, OutputTriggerType.Immediate);
            }

            TrackerOutput _output = _trackerOutputs.First();
            return new GPSTrackerOutput(
                id, 
                number, 
                _output.ProposedState== 1 ? OutputState.On:_output.ProposedState==2 ? OutputState.Off:OutputState.Previous,
                _output.LastVerifiedState == 1 ? OutputState.On : _output.LastVerifiedState == 2 ? OutputState.Off : OutputState.Previous,
                _output.TriggerType == 1 ? OutputTriggerType.Immediate : _output.TriggerType == 2 ? OutputTriggerType.TenK : OutputTriggerType.TwentyK );
        }

        public static IQueryable<TrackerCommandLog> GetTrackerCommands(string id)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            IQueryable<TrackerCommandLog> _trackerMessages =
                context.TrackerCommandLog.Where(t => (t.TrackerId == id) && (!t.Sent));
            
            return _trackerMessages;
        }


    }
}

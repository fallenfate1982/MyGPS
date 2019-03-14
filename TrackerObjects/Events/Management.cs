using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTSBizObjects;
using GTSDataStorage;
using GTSBizObjects.Events.TrackerEvents;
using System.Data.Objects;
using System.Web.Security;


namespace GTSBizObjects.Events
{
    public class Management
    {

        private  GPSTrackerEntities1 context = new GPSTrackerEntities1();

        /// <summary>
        /// Gets the list of events from a location Message analysis for the vt310e
        /// </summary>
        /// <returns></returns>
        public  List<Event> GetVT310eEvents(GTSLocationMessage locationMessage, GTSLocationMessage prevMessage)
        {
            List<Event> events = new List<Event>();


           // should link geofence checking in here

            // Need to check all known events 
            // Currently we have no case where an event can point to more than one event at a time - this may change when geo fencing is put in!!!! TODO
            // Wondering If there should be a separation of events by a level of priority for reporting here or in the DB? In the DB


            if (prevMessage != null && locationMessage.Id==0)
            {
                locationMessage.Id = prevMessage.Id;
            }

            Event theEvent;
            if (locationMessage.Speed > 0)
            {
                theEvent = createSpeedingEvent(locationMessage, prevMessage);
                if (theEvent != null) events.Add(theEvent);
            }
            else
            {
                theEvent = createIdleEvent(locationMessage, prevMessage);
                if (theEvent != null) events.Add(theEvent);
            }

            // Get Alarms 
            if(locationMessage.GetType() == (typeof(VT310eAlarmLocationMessage)))
            {
                VT310eAlarmLocationMessage msg = (VT310eAlarmLocationMessage)locationMessage;
                // tests for different alarm types 
                switch (msg.AlarmID)
                {
                        // Should find a better, more configurable way to get names for Inputs - TODO
                        // Configurable by client and probably type, including tempat for description

                    case "01": theEvent = new TrackerEvents.InputActive(msg, 1, "SOS Button"); break;
                    case "02": theEvent = new TrackerEvents.InputActive(msg, 2); break;
                    case "03": theEvent = new TrackerEvents.InputActive(msg, 3); break;
                    case "04": theEvent = new TrackerEvents.InputActive(msg, 4); break;
                    case "05": theEvent = new TrackerEvents.InputActive(msg, 5, "Engine"); break;
                    case "10": theEvent = new TrackerEvents.LowBattery(msg); break;
                    case "14": theEvent = new TrackerEvents.TrackerTurnedOn(msg); break;
                    case "15": theEvent = new TrackerEvents.GPSBlindAreaEntered(msg); break;
                    case "16": theEvent = new TrackerEvents.GPSBlindAreaExited(msg); break;
                    case "31": theEvent = new TrackerEvents.InputInactive(msg, 1, "SOS Button"); break;
                    case "32": theEvent = new TrackerEvents.InputInactive(msg, 2); break;
                    case "33": theEvent = new TrackerEvents.InputInactive(msg, 3); break;
                    case "34": theEvent = new TrackerEvents.InputInactive(msg, 4); break;
                    case "35": theEvent = new TrackerEvents.InputInactive(msg, 5, "Engine"); break;
                    case "50": theEvent = new TrackerEvents.ExternalPowerCut(msg); break;
                    case "53": theEvent = new TrackerEvents.GPSAntennaCut(msg); break;

                    default: theEvent = new TrackerEvents.TrackerAlarm(msg); break;

                }
                if (theEvent != null) events.Add(theEvent);
            }

            List<Event> entryExitEvents = new List<Event>();

            try
            {
                entryExitEvents = retrieveGeoFenceEventList(locationMessage, prevMessage);
            }
            catch (System.Exception ex)
            {

            }
            
            foreach (Event item in entryExitEvents)
            {
                events.Add(item);
            }

            return events;
        }

        #region Event Creation Logic

        private Event createSpeedingEvent(GTSLocationMessage locationMessage, GTSLocationMessage prevMessage)
        {
            // get speeding number from config for client/tracker
            int speedTresh = 80; // Need to pull this from config for multiple clients TODO

            Events.TrackerEvents.Speeding speed;
            // Check for speeding on both location messages
            if (locationMessage.Speed > speedTresh)
            {
                // If speeding is a contiuation - find event that links to prev location message that is speeding and modify
                if (prevMessage.Speed > speedTresh)
                {
                    // Pull event relating to previous message
                   GTSDataStorage.Event theEvent = context.Events.Where(e => e.ObjectId == locationMessage.TrackerID).
                       OrderByDescending(i=>i.Time).FirstOrDefault();
                   if (theEvent == null)
                   {
                       speed = new TrackerEvents.Speeding();
                   }
                   else speed = new TrackerEvents.Speeding(theEvent);
                }
                else
                {
                    // create new event if event now starts
                    speed = new TrackerEvents.Speeding();
                }
                speed.AddLocationMessage(locationMessage);

            }
            else return null;

            return speed;
            // return the modified event
        }

        private Event createIdleEvent(GTSLocationMessage locationMessage, GTSLocationMessage prevMessage)
        {
            int idleTresh = 15; // get idle number form config for client/tracker
            Events.TrackerEvents.ExcessiveIdling idleEvent;
            // && prevMessage.DInput5 == true
            if (new TimeSpan(prevMessage.IdleTime).TotalMinutes >= idleTresh && prevMessage.DInput5 == true)
            {
                // TODO migh be a better way to select this using Linq to xml
                 //Update or create new event
                GTSDataStorage.Event eventT = context.Events.Where(e => e.ObjectId == prevMessage.TrackerID).
                    Where(e=>e.Time == prevMessage.ClientRecordedDateTime).FirstOrDefault();

                if (eventT != null)
                {
                    // update the event
                    idleEvent = new TrackerEvents.ExcessiveIdling(eventT);
                    idleEvent.AddLocationMessage(locationMessage);
                }
                else
                {
                    // create a new one
                    idleEvent = new TrackerEvents.ExcessiveIdling();
                    idleEvent.AddLocationMessage(prevMessage,locationMessage);
                }
               
                return idleEvent;
            }
            else return null;
        }


        private static List<Event> retrieveGeoFenceEventList(GTSLocationMessage locationMessage, GTSLocationMessage prevMessage)
        {
            List<Event> geoFenceEvents = new List<Event>();

            // Get User Logged in
            // For now we'll just get all associated with user
            // TODO - Need to update this to pull form User/Account and tracker level later on
            // TODO - implemented using the "Created User" on the table, should be done with a proper mapping table

            if (locationMessage.TrackerID != null)
            {
                //get list of GeoFences for tracker
                List<GTSDataStorage.GeoFence> geoFencesFromDatabase = GTSDataStorage.Manager.FencesManager.GetFencesByTrackerId(locationMessage.TrackerID);
                List<GTSBizObjects.GeoFence> geoFences = new List<GeoFence>();


                // Why are these two for loops separate? TODO
                foreach (GTSDataStorage.GeoFence item in geoFencesFromDatabase)
                {
                    GTSBizObjects.GeoFence geoFence = new GeoFence(item.FencesId, item.FencesName, item.FencesCoordinate, item.IsPublic, item.Details, item.Status, item.Zoom, item.CreatedBy, item.CreatedDate, item.UpdatedBy, item.UpdatedDate);
                    geoFences.Add(geoFence);
                    
                }
                foreach (GTSBizObjects.GeoFence item in geoFences)
                {
                    // Check if prev message and current message are in the geofence

                    // TODO: Using the model of speed event we can track how long a vehicle spent in a geo fence and what they did there
                    // If prev message and message are in the geo fence we simply update the last geo fence event tied with location message and time 
                    // Stored in extended properties for the event.

                    try {
                        List<Location> locations = Events.Hepler.GeoFenceLocations(item.Coordinates);

                        bool prevLocationInFence = Events.Hepler.IsPointInPolygon(locations, new Location { Lt = (double)prevMessage.LatitudeDecimal, Lg = (double)prevMessage.LongitudeDecimal });
                        bool currentLocationInFence = Events.Hepler.IsPointInPolygon(locations, new Location { Lt = (double)locationMessage.LatitudeDecimal, Lg = (double)locationMessage.LongitudeDecimal });

                        //if tracker entered a Geo Fence add to list of events
                        if (!prevLocationInFence && currentLocationInFence)
                        {
                            Events.TrackerEvents.EnterLocation enterLocationEvent = new EnterLocation();
                            enterLocationEvent.AddLocationMessage(locationMessage, item);
                            geoFenceEvents.Add(enterLocationEvent);
                        }

                        //if tracker exited a Geo Fence add to list of events
                        if (prevLocationInFence && !currentLocationInFence)
                        {
                            Events.TrackerEvents.ExitLocation exitLocationEvent = new ExitLocation();
                            exitLocationEvent.AddLocationMessage(locationMessage, item);
                            geoFenceEvents.Add(exitLocationEvent);
                        }
                    }catch(System.Exception e)
                    {
                        Utilities.writeLine("Debug !: A GeoFence may be corrupted!!! " + e.Message);
                    }
                }
            }

            return geoFenceEvents;
        }
      

        #endregion


        public void SaveEvents(List<Event> events)
        {
            // if event id exist, then update
            GTSDataStorage.Event eventt;

            foreach (Event e in events)
            {
                if(e.EventId ==0)
                {
                    eventt = GTSDataStorage.Event.CreateEvent(0, e.EventDescription,e.GetTrackerEventType, e.Time, e.ObjectId);
                    eventt.ExtendedProperties= e.ExtendedProperties;
                    context.AddToEvents(eventt);
                }
                else
                {
                    eventt = context.Events.Where(t => t.Id == e.EventId).FirstOrDefault();
                    eventt.Description = e.EventDescription;
                    eventt.ExtendedProperties = e.ExtendedProperties;
                }
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Factory Method for Events
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public GTSBizObjects.Events.TrackerEvents.TrackerEvent GetEvent(int eventId)
        {
            GPSTrackerEntities1 dbContext = new GPSTrackerEntities1();

            GTSDataStorage.Event theEvent = dbContext.Events.Where(t => t.Id == eventId).FirstOrDefault();
            Events.TrackerEvents.TrackerEvent retEvent; 

              switch (theEvent.Type)
                {
                        // Should find a better, more configurable way to get names for Inputs - TODO
                        // Configurable by client and probably type, including tempat for description
                      // TODO - Exception Handling
                    case 1: retEvent = new TrackerEvents.Speeding(theEvent); 
                        
                        
                        
                        
                        break;
                    case 2: retEvent = new TrackerEvents.ExcessiveIdling(theEvent); break;
                    //case 3: retEvent = new TrackerEvents.EngineCutOn(theEvent); break;
                    //case 4: retEvent = new TrackerEvents.EngineCutOff(theEvent); break;
                    //case 5: retEvent = new TrackerEvents.EnterLocation(theEvent); break;
                    //case 6: retEvent = new TrackerEvents.ExitLocation(theEvent); break;
                    case 7: retEvent = new TrackerEvents.ExternalPowerCut(theEvent); break;
                    case 8: retEvent = new TrackerEvents.GPSBlindAreaEntered(theEvent); break;
                    case 9: retEvent = new TrackerEvents.GPSBlindAreaExited(theEvent); break;
                    case 10: retEvent = new TrackerEvents.LowBattery(theEvent); break;
                    case 11: retEvent = new TrackerEvents.TrackerTurnedOn(theEvent); break;
                    case 12: retEvent = new TrackerEvents.GPSAntennaCut(theEvent); break;
                    case 13: retEvent = new TrackerEvents.InputActive(theEvent); break;
                    case 14: retEvent = new TrackerEvents.InputInactive(theEvent); break;
                    case 15: retEvent = new TrackerEvents.InputActive(theEvent); break;
                    case 16: retEvent = new TrackerEvents.InputInactive(theEvent); break;

                    default: retEvent = new TrackerEvents.TrackerAlarm(theEvent); break;

                }

            return retEvent;
        }


        public List<Event> GetEventsCurrentUser(Guid userId)
        {
            List<Event> currentUserEvents = new List<Event>();
            //get listof Trackers for current user
            List<TrackerUser> currentUserTrackers = context.TrackerUser.Where(j => j.UserId == userId).ToList();
            var trackerIds = new List<string>();

            foreach (TrackerUser item in currentUserTrackers)
            {
                trackerIds.Add(item.TrackerId);
            }

            //get list of Events for current User for current day
            List<GTSDataStorage.Event> currentUserEventsDB = context.Events.Where(j => j.Time.Day == DateTime.Now.Day && j.Time.Month == DateTime.Now.Month && j.Time.Year == DateTime.Now.Year && trackerIds.Contains(j.ObjectId)).ToList();
            foreach (GTSDataStorage.Event item in currentUserEventsDB)
            {
                currentUserEvents.Add(new Event(item));
                //if (item.Type == (int)Enums.TrackerEventTypes.Speeding)
                //{
                //    currentUserEvents.Add(new Speeding(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.EnterLocation)
                //{
                //    currentUserEvents.Add(new EnterLocation(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.ExitLocation)
                //{
                //    currentUserEvents.Add(new ExitLocation(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.ExcessiveIdle)
                //{
                //    currentUserEvents.Add(new ExcessiveIdling(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.ExternalPowerCut)
                //{
                //    currentUserEvents.Add(new ExternalPowerCut(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.LowBattery)
                //{
                //    currentUserEvents.Add(new LowBattery(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.GPSAntennaCut)
                //{
                //    currentUserEvents.Add(new GPSAntennaCut(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.EnterGPSBlindArea)
                //{
                //    currentUserEvents.Add(new GPSBlindAreaEntered(item));
                //}
                //else if (item.Type == (int)Enums.TrackerEventTypes.ExitGPSBlindArea)
                //{
                //    currentUserEvents.Add(new GPSBlindAreaExited(item));
                //}
                //else
                //{
                //    currentUserEvents.Add(new Event(item));
                //}
                
            }


            return currentUserEvents;
        }

    }
}

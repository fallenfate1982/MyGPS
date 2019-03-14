using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTSBizObjects;
using System.Xml.Linq;
using System.Xml;

namespace GTSBizObjects.Events.TrackerEvents
{
    class EnterLocation:TrackerEvent
    {
        protected DateTime _entryTime;
        protected GTSLocationMessage _locationMessage;
        protected int _locMsgID;
        protected GeoFence _geoFence;
        protected int _geoFenceId;

        protected string _eventDescriptionTemplate = "{0} entered Geo Fence {1} at, {2}, on {3} at {4}.";


        public EnterLocation() 
        {
            
        }

        public EnterLocation(GTSDataStorage.Event eventt): base(eventt)
        {
            if (eventt.ExtendedProperties != null)
            {
                XDocument doc = XDocument.Parse(eventt.ExtendedProperties);

                XElement geoFenceId = doc.Descendants("GeoFenceId").FirstOrDefault();
                if (geoFenceId != null) _geoFenceId = int.Parse(geoFenceId.Value);

                XElement entryTime = doc.Descendants("EntryTime").FirstOrDefault();
                if (entryTime != null) _entryTime = XmlConvert.ToDateTime(entryTime.Value);
            }
        }

        public void AddLocationMessage(GTSLocationMessage msg, GeoFence geoFence)
        {
            base.SetTrackerInfo(msg);
            Time = msg.ClientRecordedDateTime;
            _entryTime = msg.ClientRecordedDateTime;
            _locationMessage = msg;
            _locMsgID = _locationMessage.Id;
            _geoFence = geoFence;
            _geoFenceId = geoFence.Id;
        }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, _trackerName, _geoFence.Name, _locationMessage.LatitudeDecimal + ", " + _locationMessage.LongitudeDecimal, Time.ToString("MMMM dd, yyyy"),
               Time.ToString("hh:mm tt"));
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement = base.getXMLProperties();

            // add all the location messages IDs
            XElement locMessageID = new XElement("LocationMessagesID", _locationMessage.Id);

            XElement geoFenceID = new XElement("GeoFenceId", _geoFence.Id);
            XElement endTime = new XElement("EntryTime", _entryTime);

            theElement.Add(locMessageID);
            theElement.Add(geoFenceID);
            theElement.Add(endTime);

            return theElement;
        }

        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.EnterLocation; }
        }
    }
}

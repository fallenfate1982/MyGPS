using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GTSBizObjects.Events.TrackerEvents
{

    public class Speeding:TrackerEvent
    {
        protected List<GTSLocationMessage> _messages;
        protected DateTime _startTime;
        protected DateTime _endTime;
        protected string _location;
        protected int _maxSpeed;
        protected DateTime _maxSpeedTime;


        public List<GTSLocationMessage> Messages
        {
            get { return _messages; }
        }


        protected string _eventDescriptionTemplate = "{0} was speeding from {1} to {2} on {3} around location {4} reaching speeds of {5} at {6}";
 
        public Speeding()
        {
            _messages = new List<GTSLocationMessage>();
            _maxSpeed = 0;
        }

        public Speeding(GTSDataStorage.Event eventt):base(eventt)
        {// TODO - Stuff needs to go in try catch and try parse and use exception handler
            _messages = new List<GTSLocationMessage>();

            if (eventt.ExtendedProperties != null)
            {
                XDocument doc = XDocument.Parse(eventt.ExtendedProperties);
                XElement ids = doc.Descendants("LocationMessages").FirstOrDefault();
                foreach(XElement n in ids.Descendants("ID")){

                   GTSLocationMessage msg = GTSBizObjects.Management.GetLocationMessageById(int.Parse(n.Value));
                  // if (_messages.Count == 0) SetTrackerInfo(msg);
                   if (msg != null) _messages.Add(msg);
                }


                XElement maxSpeed = doc.Descendants("MaxSpeed").FirstOrDefault();
                if (maxSpeed != null) _maxSpeed = int.Parse(maxSpeed.Value);

                XElement speedStart = doc.Descendants("SpeedStartTime").FirstOrDefault();
                if (speedStart != null) _startTime = XmlConvert.ToDateTime(speedStart.Value);

                XElement speedEnd = doc.Descendants("SpeedEndTime").FirstOrDefault();
                if (speedEnd != null) _endTime = XmlConvert.ToDateTime(speedEnd.Value);

                XElement speedLoc = doc.Descendants("SpeedLocation").FirstOrDefault();
                if (speedLoc != null) _location = speedLoc.Value;

                XElement maxSpeedTime = doc.Descendants("MaxDateTime").FirstOrDefault();
                if (maxSpeedTime != null) _maxSpeedTime = XmlConvert.ToDateTime(maxSpeedTime.Value);
            }


        }

        /// <summary>
        /// Accepts a location message to point to for further details
        /// </summary>
        public void AddLocationMessage(GTSLocationMessage locationMessage)
        {
            if (_messages.Count == 0) SetTrackerInfo(locationMessage);
            _messages.Add(locationMessage);
           

            // Probably will need to do a reverse lookup here!!! TODO
            GTSLocationMessage theFirst = _messages.First();
            _location = theFirst.LatitudeDecimal + " " + theFirst.LongitudeDecimal; // Don't know if this gives correct stuff - need to check on Heading

            if (_messages.Count == 1) Time = _startTime = locationMessage.ClientRecordedDateTime;
            _endTime = locationMessage.ClientRecordedDateTime;

            if (locationMessage.Speed > _maxSpeed)
            {
                _maxSpeed = Convert.ToInt32(locationMessage.Speed);
                _maxSpeedTime = locationMessage.ClientRecordedDateTime;
            }
            this.generateEventDescription();
        }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, _trackerName, _startTime.ToString("hh:mm tt"), _endTime.ToString("hh:mm tt"), Time.ToString("MMMM dd, yyyy"),
               _location, _maxSpeed, _maxSpeedTime.ToString("hh:mm tt"));
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement = base.getXMLProperties();

            // add all the location messages IDs
            XElement locMessages = new XElement("LocationMessages");
            foreach (GTSLocationMessage msg in _messages)
            {
                XElement aMsg = new XElement("ID",msg.Id);
                locMessages.Add(aMsg);
            }

            XElement maxSpeed = new XElement("MaxSpeed", _maxSpeed);
            XElement maxDateTime = new XElement("MaxDateTime", _maxSpeedTime);
            
            // Not needed as they can be retrieved easily from the location messages, storing them anyhow.
            XElement speedStartTime = new XElement("SpeedStartTime", _startTime);
            XElement speedEndTime = new XElement("SpeedEndTime", _endTime);
            XElement speedLocation = new XElement("SpeedLocation", _location);

            theElement.Add(locMessages);
            theElement.Add(maxSpeed);
            theElement.Add(maxDateTime);
            theElement.Add(speedEndTime);
            theElement.Add(speedStartTime);
            theElement.Add(speedLocation);

            return theElement;
        }

        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.Speeding; }
        }

    }
}

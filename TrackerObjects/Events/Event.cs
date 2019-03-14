using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events
{
    public class Event
    {
        protected DateTime _time;
        protected int _eventId;
        protected string _objectid;
       // protected ConnectObject _cobject; will need to implement this soon
        protected string _eventDescription ="";
        protected string _eventDescriptionTemplate = "A General Event Happened at {0} and involved the Object {1}";// we should be pulling this from the DB per client for each type? TODO

        protected GTSDataStorage.Event _event;


        protected int _eventType;

        public Event()
        { }

        public Event(GTSDataStorage.Event eventt)
        {
            _event = eventt;
            _time = eventt.Time;
            _eventId = eventt.Id;
            _objectid = eventt.ObjectId;
            _eventDescription = eventt.Description;
            _eventType = eventt.Type;
        }

        public string ExtendedProperties
        {
            get { return getXMLProperties().ToString(); }
        }
        public DateTime Time
        {
            get { return _time; }
            set{_time = value;}
        }

        public int EventId
        {
            get { return _eventId; }
            set { _eventId = value; }
        }

        public string ObjectId
        {
            get { return _objectid; }
            set{_objectid = value;}
        }

        public string EventDescription
        {
            get {
                generateEventDescription();
                return _eventDescription; 
            }
        }

        protected virtual void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, Time.ToString("MMMM dd, yyyy"), "Unknown");
        }

        protected virtual XElement getXMLProperties()
        {

            XElement theElement = new XElement("Properties");

            return theElement;
        }

        public virtual int GetTrackerEventType
        {
            get
            {
                return _eventType;
            }
        }
    }
}

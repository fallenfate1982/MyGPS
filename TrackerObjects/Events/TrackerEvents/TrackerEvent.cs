using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class TrackerEvent:Event
    {
        protected TrackerInfo _tracker;
        protected string _trackerName;
        protected GTSLocationMessage _locationMessage;
        protected int _locationMessageId;

        public int LocationMessageID
        {
            get { return _locationMessageId; }
        }

        public GTSLocationMessage LocationMessage
        {
            get { return _locationMessage; }
        }

        public TrackerInfo TrackerDetail
        {
            get {
                return _tracker;
                }
        }


        public TrackerEvent()
        {
        }

        public TrackerEvent(GTSDataStorage.Event eventt): base(eventt)
        {
            if (eventt.ExtendedProperties != null)
            {
                XDocument doc = XDocument.Parse(eventt.ExtendedProperties);
                XElement name = doc.Descendants("TrackerName").FirstOrDefault();
                if (name != null) _trackerName = name.Value;
                XElement locmsg = doc.Descendants("LocationMessageID").FirstOrDefault();
                if (locmsg != null)
                {
                    int.TryParse(locmsg.Value, out _locationMessageId);
                    this._locationMessage = GTSBizObjects.Management.GetLocationMessageById(_locationMessageId);
                    this._tracker = _locationMessage.TrackerDetail;
                }
            }
        }

        public void SetTrackerInfo(GTSLocationMessage theMsg)
        {
            TrackerInfo tracker = theMsg.TrackerDetail;
            _locationMessage = theMsg;
            _locationMessageId = theMsg.Id;
            _tracker = tracker;
            _trackerName = _tracker.Name;
            _objectid = tracker.Id;
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement =  base.getXMLProperties();

            XElement name = new XElement("TrackerName", _trackerName);
            XElement locmsg = new XElement("LocationMessageID", _locationMessageId);

            theElement.Add(name);
            theElement.Add(locmsg);

            return theElement;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class ExcessiveIdling:TrackerEvent
    {
        protected long _idleTime;
        protected DateTime _endTime;
        protected GTSLocationMessage _locationMessage;
        protected int _locMsgID;

        protected string _eventDescriptionTemplate = "Excessive Idling detected by {0} for {1} minuites from {2}";
 

        public ExcessiveIdling()
        {
        }

        public ExcessiveIdling(GTSDataStorage.Event eventT):base(eventT)
        {

            if (eventT.ExtendedProperties != null)
              {
                  XDocument doc = XDocument.Parse(eventT.ExtendedProperties);

                  XElement ids = doc.Descendants("LocationMessagesID").FirstOrDefault();
                  if (ids != null) _locMsgID = int.Parse(ids.Value);
                  _locationMessage = GTSBizObjects.Management.GetLocationMessageById(_locMsgID);

                  XElement endTime = doc.Descendants("EndTime").FirstOrDefault();
                  if (endTime != null) _endTime = XmlConvert.ToDateTime(endTime.Value);

                  XElement idleTime = doc.Descendants("IdleTime").FirstOrDefault();
                  if (idleTime != null) _idleTime = long.Parse(idleTime.Value);

              }
        }

        public void AddLocationMessage(GTSLocationMessage prevmsg,GTSLocationMessage msg)
        {
            base.SetTrackerInfo(msg);
            Time = prevmsg.ClientRecordedDateTime;
            _endTime = msg.ClientRecordedDateTime;
            _idleTime = prevmsg.IdleTime;
            _locationMessage = prevmsg;
            _locMsgID = _locationMessage.Id;
        }

        public void AddLocationMessage(GTSLocationMessage msg)
        {
            _endTime = msg.ClientRecordedDateTime;
            _idleTime = msg.IdleTime;
        }

        public TimeSpan TimeSpan
        {
            get
            {
                TimeSpan span = new TimeSpan(_idleTime);
                return span;
            }
        }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, _trackerName, Convert.ToInt32(TimeSpan.TotalMinutes), _time.ToString("hh:mm tt"));
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement = base.getXMLProperties();

            // add all the location messages IDs
            XElement locMessageID = new XElement("LocationMessagesID",_locationMessage.Id);


            XElement endTime = new XElement("EndTime", _endTime);
            XElement idleTime = new XElement("IdleTime", _idleTime);

            theElement.Add(locMessageID);
            theElement.Add(endTime);
            theElement.Add(idleTime);

            return theElement;
        }

        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.ExcessiveIdle; }
        }
    }
}

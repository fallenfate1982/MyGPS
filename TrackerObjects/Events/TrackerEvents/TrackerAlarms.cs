using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class TrackerAlarm:TrackerEvent
    {
        protected VT310eAlarmLocationMessage _msg;
        protected string _eventDescriptionTemplate = "An Alarm was triggered on {0} with ID {1}";// we should be pulling this from the DB per client for each type? TODO
        protected string _alarmId = "";

        public TrackerAlarm(VT310eAlarmLocationMessage msg)
        {
            _msg = msg;
            base.SetTrackerInfo(msg);
            Time = msg.ClientRecordedDateTime;
            _alarmId = _msg.AlarmID;
        }

        public TrackerAlarm(GTSDataStorage.Event eventT)
            : base(eventT)
        {
            if (eventT.ExtendedProperties != null)
            {
                XDocument doc = XDocument.Parse(eventT.ExtendedProperties);
                XElement name = doc.Descendants("AlarmID").FirstOrDefault();
                if (name != null) _alarmId = name.Value;
            }
        }

         protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate,this._trackerName,_alarmId);
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement = base.getXMLProperties();

            XElement locMessages = new XElement("AlarmID",_alarmId);
           

            theElement.Add(locMessages);

            return theElement;
        }
        
    }
}

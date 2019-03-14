using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class TrackerTurnedOn:TrackerAlarm
    {
        protected string _eventDescriptionTemplate = "GPS Tracking for {0} has been turned on";// we should be pulling this from the DB per client for each type? TODO

        public TrackerTurnedOn(VT310eAlarmLocationMessage msg):base(msg)
        {
        }

        public TrackerTurnedOn(GTSDataStorage.Event eventT) : base(eventT) { }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, this._trackerName);
        }

        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.TrackerTurnedOn; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events.TrackerEvents
{
    public  class LowBattery:TrackerAlarm
    {
        protected string _eventDescriptionTemplate = "Battery is critically low on {0}";// we should be pulling this from the DB per client for each type? TODO

        public LowBattery(VT310eAlarmLocationMessage msg):base(msg)
        {
        }

        public LowBattery(GTSDataStorage.Event eventT)
            : base(eventT)
        {

        }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, this._trackerName);
        }
        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.LowBattery; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class GPSBlindAreaExited:TrackerAlarm
    {
        protected string _eventDescriptionTemplate = "{0} has exited a GPS blind area";// we should be pulling this from the DB per client for each type? TODO

        public GPSBlindAreaExited(VT310eAlarmLocationMessage msg)
            : base(msg)
        {
        }

        public GPSBlindAreaExited(GTSDataStorage.Event eventT) : base(eventT) { }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, this._trackerName);
        }

        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.ExitGPSBlindArea; }
        }
    }
}

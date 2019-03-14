using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class ExternalPowerCut:TrackerAlarm
    {
        protected string _eventDescriptionTemplate = "External Power has been cut to {0}!";// we should be pulling this from the DB per client for each type? TODO

        public ExternalPowerCut(VT310eAlarmLocationMessage msg)
            : base(msg)
        {
        }

        public ExternalPowerCut(GTSDataStorage.Event eventT) : base(eventT) { }

        protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, this._trackerName);
        }
        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.ExternalPowerCut; }
        }
    }
}

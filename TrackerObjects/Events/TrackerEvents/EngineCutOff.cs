using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects.Events.TrackerEvents
{
    class EngineCutOff:TrackerEvent
    {
        public override int GetTrackerEventType
        {
            get { return (int)Enums.TrackerEventTypes.EngineCutOff; }
        }
    }
}

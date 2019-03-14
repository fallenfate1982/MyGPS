using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    public class GPSTrackerOutput
    {
        public string TrackerId { get; set; }
        public OutputType Type { get; set; }
        public int Number { get; set; }
        public OutputState ProposedState { get; set; }
        public OutputState LastVerifiedState { get; set; }
        public OutputTriggerType TriggerType { get; set; }


        public GPSTrackerOutput(string trackerId, int number, OutputState newState, OutputState lastState, OutputTriggerType triggerType)
        {
            this.TrackerId = trackerId;
            this.Number = number;
            //don't need to know the type from Server POV
            this.Type = OutputType.Unknown;
            this.ProposedState = newState;
            this.LastVerifiedState = lastState;
            this.TriggerType = triggerType;
        }
    }


    public enum OutputType
    { Engine, Alarm, Door, Horn, Headlight, Trunk, Unknown }


    public enum OutputState
    { On, Off, Previous }

    public enum OutputTriggerType
    { Immediate, TenK, TwentyK }
}

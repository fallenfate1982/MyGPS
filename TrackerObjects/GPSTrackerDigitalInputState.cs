using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    class GPSTrackerDigitalInputState
    {
        public string TrackerId { get; set; }
        public InputType Type { get; set; }
        public int Number { get; set; }
        public InputState CurrentState { get; set; }
        public DateTime DateTime { get; set; }

        public GPSTrackerDigitalInputState(string trackerId, int number, InputState newState, DateTime dt)
        {
            this.TrackerId = trackerId;
            this.Number = number;
            //don't need to know the type from Server POV
            this.Type = InputType.Unknown;
            this.CurrentState = newState;
            this.DateTime = dt;
        }
    }
}

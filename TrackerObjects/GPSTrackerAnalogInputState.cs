using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    class GPSTrackerAnalogInputState
    {
        public string TrackerId { get; set; }
        public InputType Type { get; set; }
        public int Number { get; set; }
        public double Voltage { get; set; }
        public DateTime DateTime { get; set; }

        public GPSTrackerAnalogInputState(string trackerId, int number, InputState newState, double voltage, DateTime dt)
        {
            this.TrackerId = trackerId;
            this.Number = number;
            //don't need to know the type from Server POV
            this.Type = InputType.Unknown;
            this.Voltage = voltage;
            this.DateTime = dt;
        }
    }

    public enum InputType
    { Engine, Alarm, Door, Horn, Headlight, Trunk, Unknown, SOS, Fuel, Temp }


    public enum InputState
    { On, Off }

}

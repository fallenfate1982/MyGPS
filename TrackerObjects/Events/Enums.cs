using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects.Events.Enums
{
        public enum TrackerEventTypes
        {
            Speeding =1,
            ExcessiveIdle,
            EngineCutOn,
            EngineCutOff,
            EnterLocation,
            ExitLocation,
            ExternalPowerCut,
            EnterGPSBlindArea,
            ExitGPSBlindArea,
            LowBattery,
            TrackerTurnedOn,
            GPSAntennaCut,
            EngineOn,
            EngineOff,
            InputOn,
            InputOff
        };
}

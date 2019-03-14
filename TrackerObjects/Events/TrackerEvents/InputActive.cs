using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GTSBizObjects.Events.TrackerEvents
{
    public class InputActive:TrackerAlarm
    {
         protected int _inputNum;
        protected string _inputName = "";
        protected string _eventDescriptionTemplate = "{0} was made active on {1}";// we should be pulling this from the DB per client for each type? TODO

        public InputActive(VT310eAlarmLocationMessage msg, int input, string name):base(msg)
        {
            _inputNum = input;
            _inputName = name;
        }

        public InputActive(GTSDataStorage.Event eventT):base(eventT)
        {
            if (eventT.ExtendedProperties != null)
            {
                XDocument doc = XDocument.Parse(eventT.ExtendedProperties);
                XElement number = doc.Descendants("InputNumber").FirstOrDefault();
                XElement name = doc.Descendants("InputName").FirstOrDefault();
                if (number != null) int.TryParse(number.Value,out _inputNum);
                if (name != null) _inputName = name.Value;
            }
        }

        public InputActive(VT310eAlarmLocationMessage msg, int input)
            : base(msg)
        {
            _inputNum = input;
            _inputName = "Input "+ input;
        }

         protected override void generateEventDescription()
        {
            _eventDescription = String.Format(_eventDescriptionTemplate, _inputName,this._trackerName);
        }

        protected override XElement getXMLProperties()
        {
            XElement theElement = base.getXMLProperties();

            XElement inputNum = new XElement("InputNumber", _inputNum);
            XElement inputName = new XElement("InputName", _inputName);

            theElement.Add(inputNum);
            theElement.Add(inputName);

            return theElement;
        }

        public override int GetTrackerEventType
        {
            get {
                // TODO - architect this so that inputs and there types/uses are configurable
                switch (_inputNum)
                {
                    case 5: return (int)Enums.TrackerEventTypes.EngineOn;
                    default: return (int)Enums.TrackerEventTypes.InputOn;
                }
            }
        }
    }
}

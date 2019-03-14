using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    public class GTSLocationMessageHistory
    {
        private TrackerInfo _trackerInfo;
        private List<GTSLocationMessageLite> _messages;
        public GTSLocationMessageHistory(List<GTSLocationMessageLite> messages, TrackerInfo trackerInfo)
        {
            _trackerInfo = trackerInfo;
            _messages = messages;

        }

        public TrackerInfo TrackerDetail
        { 
            get { return _trackerInfo; }
            set { _trackerInfo = value; }
        
        }

        public List<GTSLocationMessageLite> Markers
        {
            get { return _messages; }
            set { _messages = value; }

        }
    }
}

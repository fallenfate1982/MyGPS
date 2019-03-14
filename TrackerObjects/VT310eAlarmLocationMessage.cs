using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    /// <summary>
    /// Should be Inheriting from a VT310 but does not exist yet
    /// </summary>
    public class VT310eAlarmLocationMessage:VT300LocationMessage
    {
        protected string _alarmId = "";

        public VT310eAlarmLocationMessage(string tid, byte[] message, int bytesRead)
            : base(tid, message, bytesRead) 
        {
            string _hex = Utilities.ByteArrayToString(message, bytesRead);
            _alarmId = _hex.Substring(26, 2);


        }

        public string AlarmID
        {
            get { return _alarmId; }
        }
    }
}

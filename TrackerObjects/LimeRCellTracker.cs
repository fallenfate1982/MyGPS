using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace GTSBizObjects
{
    public class LimeRCellTracker : CellTracker
    {
        private int _vibe;

        public int Vibe
        {
            get { return _vibe; }
            set { _vibe = value; }
        }
        private int _statusLR;

        public int StatusLR
        {
            get { return _statusLR; }
            set { _statusLR = value; }
        }
        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public LimeRCellTracker(string _trackerId, string _name, string _desc, string _status,
            string _phNumber, string _password, string _authNumbers, DateTime? lastUp, double? lastlat, double? lastlon, int _v, int _sLR, string _msg)
            : base(_trackerId, _name, _desc, _status, _phNumber, _password, _authNumbers, lastUp, lastlat, lastlon)
        {
            this._message = _msg;
            this._statusLR = _sLR;
            this._vibe = _v;
        }

        public new bool RecievedMessage(NameValueCollection message)
        {
            // need to update this to also record messages if there are multiple in the report.
            LimeRCellLocationMessage locationMessage = new LimeRCellLocationMessage(message);

            /// will only save if being used as a full tracker
            if (locationMessage.IsValid)
            {// only store valid readings
                try
                {
                    GTSBizObjects.CellManager.UpdateTrackerInformation(locationMessage);
                }
                catch (Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool RecievedMessage(LimeRCellLocationMessage locationMessage)
        {
            // need to update this to also record messages if there are multiple in the report.
            
            /// will only save if being used as a full tracker
            if (locationMessage.IsValid)
            {// only store valid readings
                try
                {
                    GTSBizObjects.CellManager.UpdateTrackerInformation(locationMessage);
                }
                catch (Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}

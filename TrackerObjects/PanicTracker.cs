using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace GTSBizObjects
{
    public class PanicTracker : CellTracker
    {
        public PanicTracker(string _trackerId, string _name, string _desc, string _status,
            string _phNumber, string _password, string _authNumbers, DateTime? lastUp, double? lastlat, double? lastlon)
            : base(_trackerId, _name, _desc, _status, _phNumber, _password, _authNumbers, lastUp, lastlat, lastlon)
        {
        }

        public new bool RecievedMessage(NameValueCollection message)
        {
            // need to update this to also record messages if there are multiple in the report.
            PanicLocationMessage locationMessage = new PanicLocationMessage(message);

            /// will only save if being used as a full tracker
            if (locationMessage.IsValid)
            {// only store valid readings
                try
                {
                    // save history
                    GTSBizObjects.Management.SaveTrackerInformation(locationMessage);
                    // save the last location
                    GTSBizObjects.CellManager.UpdateTrackerInformation(locationMessage);
                    // ensure that First response is now tracking this tracker
                    GTSBizObjects.Management.AlertSecurity(locationMessage.TrackerID);
                    // if this tracker was a phone, then a "trackerId" would have been sent as the default vehicle
                    // this needs to show up on First Response's system as well
                    string _tid = message["trackerId"];

                    if (_tid != "") GTSBizObjects.Management.AlertSecurity(_tid);

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

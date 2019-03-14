using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web;
using GTSBizObjects;

namespace MyGPS
{
    public class UIGTSTracer
    {
        /// <summary>
        /// Retrieve current information for the trackers of the current user
        /// </summary>
        /// <returns></returns>
        public static List<GTSLocationMessage> GetCurrentTrackersInfo()
        {
            MembershipUser _currentUser = Membership.GetUser();

            List<GTSLocationMessage> _messages =
                Management.CreateCurrentTrackersByUser((Guid)_currentUser.ProviderUserKey);

            return _messages;
        }

        public static GTSLocationMessageHistory GetTrackerInfoByDate(string trackerId, DateTime startDate, DateTime endDate)
        {
            List<GTSLocationMessageLite> _messages =
                Management.GetTrackerInfoByDate(trackerId, startDate, endDate);

            TrackerInfo _info = Management.GetTrackerInfo(trackerId);

            GTSLocationMessageHistory history = new GTSLocationMessageHistory(_messages, _info);

            return history;
        }
    }
}
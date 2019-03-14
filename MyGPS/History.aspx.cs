using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

using GTSBizObjects;

namespace MyGPS
{
    public partial class History : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            datepicker.Value = Request.QueryString["histDate"];
            trackerId.Value = Request.QueryString["currTrackerId"];
            time1.Value = Request.QueryString["timeSlice"];

        }

        [WebMethod()]
        public static string GetTrackers()
        {
            MembershipUser m = Membership.GetUser();

            List<TrackerInfo> _trackers = Management.GetGPSTrackersForUser((Guid)m.ProviderUserKey);

            string _serialedMessages = JsonConvert.SerializeObject(_trackers);
            return "{\"Trackers\":" + _serialedMessages + "}";
        }
    
       [WebMethod()]
        public static string GetTrackerHistory(string trackerId, string selectedDate, int timeInterval)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Date is being passed in as mm/dd/yyyy
                string[] _parseDateString = selectedDate.Split('/');
                DateTime _startDate = new DateTime(int.Parse(_parseDateString[2]), int.Parse(_parseDateString[0]), int.Parse(_parseDateString[1]), (timeInterval - 1) *6, 0, 0);
                DateTime _endDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, (timeInterval * 6) - 1, 59, 59);
                GTSLocationMessageHistory _history = UIGTSTracer.GetTrackerInfoByDate(trackerId, _startDate, _endDate);
                string _serialedMessages = JsonConvert.SerializeObject(_history);
                return "{\"History\":" + _serialedMessages + "}";
            }
            return "{\"History\": []}";
        }
    }
}
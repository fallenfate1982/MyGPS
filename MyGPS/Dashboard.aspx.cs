using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSBizObjects;
using System.Web.UI.HtmlControls;

namespace MyGPS
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private const string SPEED_EVENT_CSS_CLASS = "badge badge-danger";
        private const string SPEED_NORMAL_CSS_CLASS = "badge badge-success";

        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser _currentUser = Membership.GetUser();

            List<GTSLocationMessage> userCurrentTrackerInfo = UIGTSTracer.GetCurrentTrackersInfo();
            List<GTSBizObjects.Events.Event> currentUserEvents = new GTSBizObjects.Events.Management().GetEventsCurrentUser((Guid)_currentUser.ProviderUserKey);

            int speedEventCount = 0;
            int idleEventCount = 0;
            int attentionEventCount = 0;

            List<string> trackersWithSpeedEvent = new List<string>();
            List<string> trackersWithIdleEvent = new List<string>();
            List<string> trackersWithAttentionEvent = new List<string>();
            foreach (GTSBizObjects.Events.Event item in currentUserEvents)
            {
                if (item.GetTrackerEventType == (int)GTSBizObjects.Events.Enums.TrackerEventTypes.Speeding)
                {
                    speedEventCount += 1;
                    if (!(trackersWithSpeedEvent.Contains(item.ObjectId)))
                    {
                        trackersWithSpeedEvent.Add(item.ObjectId);
                    }
                    
                }
                if (item.GetTrackerEventType == (int)GTSBizObjects.Events.Enums.TrackerEventTypes.ExcessiveIdle)
                {
                    idleEventCount += 1;
                    if (!(trackersWithIdleEvent.Contains(item.ObjectId)))
                    {
                        trackersWithIdleEvent.Add(item.ObjectId);
                    }
                }
                if (item.GetTrackerEventType == (int)GTSBizObjects.Events.Enums.TrackerEventTypes.LowBattery ||
                    item.GetTrackerEventType == (int)GTSBizObjects.Events.Enums.TrackerEventTypes.GPSAntennaCut ||
                    item.GetTrackerEventType == (int)GTSBizObjects.Events.Enums.TrackerEventTypes.EnterGPSBlindArea)
                {
                    attentionEventCount += 1;
                    if (!(trackersWithAttentionEvent.Contains(item.ObjectId)))
                    {
                        trackersWithAttentionEvent.Add(item.ObjectId);
                    }
                    
                }

            }

            List<DashboardViewModel> dashboardDataList = new List<DashboardViewModel>();
            foreach (GTSLocationMessage item in userCurrentTrackerInfo)
            {
                DashboardViewModel dashboardRecord = new DashboardViewModel();
                dashboardRecord.TrackerName = item.TrackerDetail.Name;
                dashboardRecord.TrackerId = item.TrackerID;
                dashboardRecord.DetailsNavigationURL = "MyTrack.aspx?Id=" + item.TrackerID;

                int timeSlice = 1;
                TimeSpan slice1Start = new TimeSpan(0, 0, 0);
                TimeSpan slice2Start = new TimeSpan(6, 0, 0);
                TimeSpan slice3Start = new TimeSpan(12, 0, 0);
                TimeSpan slice4Start = new TimeSpan(18, 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;

                if(now >= slice1Start && now < slice2Start) {
                    timeSlice = 1;
                }
                else if(now >= slice2Start && now < slice3Start) {
                    timeSlice = 2;
                }
                else if(now >= slice3Start && now < slice4Start) {
                    timeSlice = 3;
                }
                else {
                    timeSlice = 4;
                }

                dashboardRecord.HistoryNavigationURL = "History.aspx?timeSlice=" + timeSlice.ToString() + "&histDate=" + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year + "&currTrackerId=" + item.TrackerID;

                dashboardRecord.TrackerMileage = Math.Floor(item.Mileage.HasValue? item.Mileage.Value : 0.0);
                dashboardRecord.TrackerSpeed = Math.Round(item.Speed, 2).ToString() + " KPH";
                if (trackersWithSpeedEvent.Contains(item.TrackerID))
                {
                    dashboardRecord.TrackerSpeedClass = SPEED_EVENT_CSS_CLASS;
                }
                else
                {
                    dashboardRecord.TrackerSpeedClass = SPEED_NORMAL_CSS_CLASS;
                }

                if (trackersWithAttentionEvent.Contains(item.TrackerID))
                {
                    dashboardRecord.NeedsAttention = true;
                }
                else
                {
                    dashboardRecord.NeedsAttention = false;
                }

                dashboardDataList.Add(dashboardRecord);
            }

            this.txtEventCount.InnerText = currentUserEvents.Count.ToString();
            this.txtIdleEventCount.InnerText = trackersWithIdleEvent.Count.ToString();
            this.txtSpeedEventCount.InnerText = trackersWithSpeedEvent.Count.ToString();

            this.trackerList.DataSource = dashboardDataList;
            this.trackerList.DataBind();
        }

    }

    public class DashboardViewModel {
        public string TrackerName { get; set; }

        public string TrackerId { get; set; }

        public string DetailsNavigationURL { get; set; }

        public string HistoryNavigationURL { get; set; }

        public double TrackerMileage { get; set; }
        public string TrackerSpeed { get; set; }

        public string TrackerSpeedClass { get; set; }

        public bool NeedsAttention { get; set; }
    }
}
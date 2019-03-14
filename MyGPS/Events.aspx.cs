using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSBizObjects;
using Newtonsoft.Json;
using System.Web.Services;


namespace MyGPS
{
    public partial class Events : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            this.ReportViewer1.ReportServiceUrl = VirtualPathUtility.ToAbsolute("~/api/RDLReport");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //SetReportParam("1905004768","10/13/2014");
                getTrackersByUserLoggedIn();
            }
        }

        protected void getTrackersByUserLoggedIn()
        {
            MembershipUser currentUser = Membership.GetUser(User.Identity.Name);
            string username = currentUser.UserName; //** get UserName
            Guid userID = (Guid)currentUser.ProviderUserKey; //** get user ID

            List<TrackerInfo> trackers = Management.GetGPSTrackersForUser(userID);

            foreach (TrackerInfo t in trackers)
            {
                ListItem l = new ListItem(t.Name, t.Id);
                dropDown.Items.Add(l);
            }
        }

        protected void SetReportParam(string id, string date)
        {
            Syncfusion.JavaScript.Models.ReportViewer.ReportParameter paramDate = new Syncfusion.JavaScript.Models.ReportViewer.ReportParameter();
            paramDate.Name = "Date";
            paramDate.Values.Add(date);

            Syncfusion.JavaScript.Models.ReportViewer.ReportParameter paramObjId = new Syncfusion.JavaScript.Models.ReportViewer.ReportParameter();
            paramObjId.Name = "ObjectID";
            paramObjId.Values.Add(id);

            this.ReportViewer1.Parameters.Add(paramDate);
            this.ReportViewer1.Parameters.Add(paramObjId);

            

        }
        protected void btnUpdateReport_Click(object i, EventArgs n)
        {
            SetReportParam(dropDown.SelectedValue,txtdate.Text);
        }

        [WebMethod()]
        public static string GetEvent(int eventId)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                GTSBizObjects.Events.TrackerEvents.TrackerEvent _event = new GTSBizObjects.Events.Management().GetEvent(eventId);
                JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                string _serialedMessages = JsonConvert.SerializeObject(_event,setting);
                return "{\"Event\":" + _serialedMessages + "}";
            }
            return "{\"Event\": []}";
        }

    }
}
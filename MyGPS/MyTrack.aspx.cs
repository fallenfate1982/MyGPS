using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Services;
using Newtonsoft.Json;

using GTSBizObjects;

namespace MyGPS
{
    public partial class MyTrack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.txtDefaultTrackerId.Text = string.Empty;
            try
            {
                
                string trackerId = Request.QueryString["Id"];
                if (trackerId != null)
                {
                    this.txtDefaultTrackerId.Text = trackerId;
                }
            }
            catch { Exception ex; } { }
            
        }

       

        /// <summary>
        /// Retrieve the data required for plotting curret position for a trackers assoicated with the current user
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public static string GetMarkers()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string _serialedMessages = JsonConvert.SerializeObject(UIGTSTracer.GetCurrentTrackersInfo());
                return "{\"Markers\":" + _serialedMessages + "}";
            }
            return "{\"Markers\": []}";
        }

    }
}
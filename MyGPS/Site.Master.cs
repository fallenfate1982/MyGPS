using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Security.Principal;

namespace MyGPS
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.logOut.Click += new EventHandler(logOut_Click);

            if (!IsPostBack)
            {
                setUpUserInfo();
            }

            // Bit that stops SBC from seeing the geofence option
            if (HttpContext.Current.User.Identity.Name =="SBC-Michael")
            { 
                funGeoFence.Visible = false;
            }

            // we need To detect if any toast notifications will need to be shown to the client

            string _message = Request.QueryString["message"];
            string _toast = Request.QueryString["toast"];

            if(_toast == "true")
            {
                // Stuff this information into some hidden inputs so that the client can detect it and act accordingly
                doToast.Value = "true";
                message.Value = "Your password has successfully been updated!";
            }
            else
            {
                doToast.Value = "false";
            }
        }

        protected void logOut_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx");
        }

        protected void setUpUserInfo()
        {
            string userName = HttpContext.Current.User.Identity.Name;

            MembershipUser theUser = Membership.GetUser(userName);

            userNameTopNavA.InnerText = userNameSideNav.InnerText = userNameTopNav.InnerText = theUser.UserName;
            userEmail.InnerText = theUser.Email;

            System.Web.Profile.ProfileBase profile = HttpContext.Current.Profile;

            //Use this to set profile picture
            //profile.SetPropertyValue("ProfilePicture", "img/SSSL.JPG");
            //string defaultpic = "img/user-default.jpg";
            
            string pic = profile.GetPropertyValue("ProfilePicture").ToString();
            if (pic == "")
            {
                pic = "img/user-default.jpg";
            }

            userImgSideNav.Src = pic;
            userImgTopNav.Src = pic;

        }

        public void setNavigation(Constants.TopNavOption option)
        {
            //if (option == Constants.TopNavOption.Login)
            //{
            //    this.navigation.Visible = false;
            //}
            //else
            //{
            //    this.mapOption.Attributes["class"] = string.Empty;
            //    this.historyOption.Attributes["class"] = string.Empty;

            //    if (option == Constants.TopNavOption.Map)
            //    {
            //        this.mapOption.Attributes["class"] = "active";
            //    }
            //    else if (option == Constants.TopNavOption.History)
            //    {
            //        this.historyOption.Attributes["class"] = "active";
            //    }

            //    this.navigation.Visible = true;
            //}
        }
    }
}

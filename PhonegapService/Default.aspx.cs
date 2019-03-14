using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSBizObjects;
using Newtonsoft.Json;
using System.Web.Services;

namespace PhonegapService
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Expected information id, action

            // action: checkin - id, location information, vibe #, personal message
            // action: 

            //string _state = "ERROR";
            string _id = Request.QueryString["phnum"];

            if(_id !=null && _id != "") Panic();

           /* CellTracker tracker = GTSBizObjects.CellManager.GetLRCellTracker(_id);


            if (tracker.GetType() == typeof(LimeRCellTracker))
            {
                LimeRCellTracker ltracker = (LimeRCellTracker)tracker;
                if (tracker != null && CheckIn(ltracker))
                    _state = "HELLO";
                else
                    _state = "ERROR";

                _state = GetLimersAround(4, ltracker);

                Response.Write(_state);
            }

            if(tracker.GetType() == typeof(LimeRCellTracker))
            {

            }*/
        }


        //TODO : make these webmethods! string phnum, string head, string speed, string lat, string lon,string time, string trackerId
        public bool Panic() {

            // Logic is off there TODO: need to create a panic CellTracker from the factory method
            //CellTracker tracker = GTSBizObjects.CellManager.GetLRCellTracker(Request.QueryString["phnum"]);

            bool retVal = true;
            //retVal = retVal && tracker.RecievedMessage(Request.QueryString);

            // hack
           
            // ensure that First response is now tracking this tracker
            GTSBizObjects.Management.AlertSecurity(Request.QueryString["phnum"]);
            // if this tracker was a phone, then a "trackerId" would have been sent as the default vehicle
            // this needs to show up on First Response's system as well
            string _tid = Request.QueryString["trackerId"];

            if (_tid != "") GTSBizObjects.Management.AlertSecurity(_tid);

            return retVal;
        }

        protected bool CheckIn(LimeRCellTracker tracker)
        {
            bool retVal = true;
            retVal = retVal && tracker.RecievedMessage(Request.QueryString);

            return retVal;
        }

        protected string GetLimersAround(int distance, LimeRCellTracker tracker)
        {
            string _serialedMessages = JsonConvert.SerializeObject(LinkManager.GetTrackersinDistance(distance, tracker.LastLon, tracker.LastLat,tracker.TrackerId,tracker.Vibe));
            return "{\"Trackers\":" + _serialedMessages + "}";
             
        }


        ////////////// Contact Management/////////

        /// <summary>
        ///Add a contact to the given Limer's list  
        /// </summary>
        /// <returns></returns>
        protected string AddContact()
        {
            return "";
        }

        /// <summary>
        /// Remove Contact from Limer's List
        /// </summary>
        /// <returns></returns>
        protected string RemoveContact()
        {
            return "";
        }


        /// <summary>
        /// Retrieve a list of current contacts associated with the limer. Will be used to sync contacts or retrieve contacts if lost
        /// </summary>
        /// <returns></returns>
        protected string GetContacts()
        {
            return "";
        }

        /// <summary>
        /// Add a bulk of contacts. Will send request to contacts that have LimeRadar
        /// </summary>
        /// <returns>Returns a list of contacts that do not have lime radar</returns>
        protected string AddContacts()
        {
            return "";
        }

        /// <summary>
        /// Accepts all contacts awaiting confirmation
        /// </summary>
        /// <returns>list of contacts that were request was granted for</returns>
        protected string CheckRequests()
        {
            return "";
        }

        /// <summary>
        /// Checks if there are any notifications waiting
        /// </summary>
        /// <returns>String of notifications</returns>
        protected string CheckNotifcations()
        {
            return "";
        }

        /// <summary>
        /// Accepts a contact request
        /// </summary>
        /// <returns></returns>
        protected string AcceptRequest()
        {
            return "";
        }

        /// <summary>
        /// Get settings such as vibes, status, link levels, description
        /// </summary>
        /// <returns></returns>
        protected string GetSettings()
        {
            return "";
        }


        /// <summary>
        /// Register user. Accept Id and Name save info and send sms
        /// </summary>
        /// <returns></returns>
        protected string RegisterUser()
        {
            return "";
        }

        /// <summary>
        /// Accept ID and activation code. Compare to DB and move to activated.
        /// </summary>
        /// <returns></returns>
        protected string ActivateUser()
        {
            return "";
        }

    }
}
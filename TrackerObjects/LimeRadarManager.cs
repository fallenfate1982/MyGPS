using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.Objects;
using GTSDataStorage;
using System.Net;
using System.IO;
using System.Web;

namespace GTSBizObjects
{
    public class CellManager
    {

        public bool StartSMSRegistration(string phonenumber)
        {
            GPSTrackerEntities1 context = new GPSTrackerEntities1();
            bool retval = true;

           
            // Get 4 digit random pass code
            string passcode = getPasscode();

            // Send pascode to phone number
            retval = retval && sendSMS(phonenumber,"This is your passcode: "+passcode);

            // save pass code and phone number to table. Check to see if exist already


            LR_Passcode passCode =context.LR_Passcode.Where(p=> p.phonenumber == phonenumber).FirstOrDefault();

            if (passCode == null)
            {
                passCode = new LR_Passcode();
                context.AddToLR_Passcode(passCode);
            }

            passCode.Passcode = passcode;
            passCode.phonenumber = phonenumber;

            try
            {
                context.SaveChanges();
            }
            catch (System.Exception e)
            {
                retval = false;
            }

            return retval;
        }

        public bool CompleteRegistration(string fname, string lname, string phonenumber, string passcode)
        {
            bool retval = true;
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            LR_Passcode pass = context.LR_Passcode.Where(p => p.phonenumber == phonenumber).FirstOrDefault();
            retval = pass != null && pass.Passcode.Trim() == passcode.Trim();

            if (retval)
            {
                Trackers newTracker = context.Trackers.Where(t => t.Id == phonenumber).FirstOrDefault();
                // test that this works if tracker does not exist and if it does exist
                if (newTracker == null)
                {
                    newTracker = new Trackers();
                    context.Trackers.AddObject(newTracker);
                    newTracker.Id = phonenumber;
                }

                newTracker.Name = fname +" "+ lname;
                newTracker.Type = "TYPMB";
                newTracker.Status = "LR002";

                // Create or overwrite LimerInfo for user
                LR_LimerInfo _info = context.LR_LimerInfo.Where(p => p.TrackerId == phonenumber).FirstOrDefault();

                if (_info == null)
                {
                    _info = new LR_LimerInfo();
                    context.LR_LimerInfo.AddObject(_info);
                    _info.TrackerId = phonenumber;
                }

                _info.Vibe = 1;
                _info.Status = 3;
                _info.Message = "Hey, I'm on LimeR!!!";

                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    retval = false;
                }
            }
            return retval;
        }

        // Need to be refactered as part of a utility class

        public string getPasscode()
        {

            // need to update to generate random number
            string retval= "1234";

            Random rgen = new Random();
            retval = rgen.Next(1000, 9999).ToString();

            return retval;
        }

        // need to be refactered as part of an SMS alert or general notification class / objects
        // Need to factor out configurable stuff
        public bool sendSMS(string id, string message)
        {
            bool retval = true;

            String Server = "121.241.242.114";
            String Port = "8080";
            String UserName = "sap-atsl";
            String Password = "atsl2013";
            int type = 0;
            String Message = HttpUtility.UrlEncode(message);
            int DLR = 1;
            String Source = "LimeR";
            String Destination = "1"+id;
            String WebResponseString = "";
            String URL = "http://" + Server + ":" + Port + "/bulksms/bulksms?username=" + UserName + "&password=" + Password + "&type=" + type
                        + "&dlr=" + DLR + "&destination=" + Destination + "&source=" + Source + "&message=" + Message + "";
            WebRequest webrequest = HttpWebRequest.Create(URL);
            webrequest.Timeout = 25000;

            try
            {
                WebResponse webresponse = webrequest.GetResponse();
                StreamReader reader = new StreamReader(webresponse.GetResponseStream());
                WebResponseString = reader.ReadToEnd();
                webresponse.Close();
            }

            catch (System.Exception e)
            {
                ExceptionHandler.HandleGeneralException(e);
            }

            retval = WebResponseString.Substring(0, 4) == "1701";
            return retval;
        }

        // Will add profile pic, and name later on
        public bool UpdateSettings(string message, string phonenumber)
        {
            bool retval = true;
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            // Create or overwrite LimerInfo for user
            LR_LimerInfo _info = context.LR_LimerInfo.Where(p => p.TrackerId == phonenumber).FirstOrDefault();

            if (_info == null)
            {
                retval = false;
            }
            else
            {
                _info.Message = message;

                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    retval = false;
                }
            }
            
            return retval;
        }

        public bool UpdateStatus( int stat, string phonenumber)
        {
            bool retval = true;
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            // Create or overwrite LimerInfo for user
            LR_LimerInfo _info = context.LR_LimerInfo.Where(p => p.TrackerId == phonenumber).FirstOrDefault();

            if (_info == null)
            {
                retval = false;
            }
            else
            {
                _info.Status = stat;

                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    retval = false;
                }
            }

            return retval;
        }

        public bool UpdateVibe(int vibe, string phonenumber)
        {
            bool retval = true;
            GPSTrackerEntities1 context = new GPSTrackerEntities1();

            // Create or overwrite LimerInfo for user
            LR_LimerInfo _info = context.LR_LimerInfo.Where(p => p.TrackerId == phonenumber).FirstOrDefault();

            if (_info == null)
            {
                retval = false;
            }
            else
            {

                _info.Vibe = vibe;

                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception e)
                {
                    ExceptionHandler.HandleGeneralException(e);
                    retval = false;
                }
            }

            return retval;
        }

        public static bool UpdateTrackerInformation(CellLocationMessage message)
        {
            bool retval = true;

            GPSTrackerEntities1 context = new GPSTrackerEntities1();
            
            Trackers _tracker =
               context.Trackers.Where(t => t.Id == message.TrackerID).FirstOrDefault();
            _tracker.LastUpdate = DateTime.Now;
            _tracker.LastLat = message.Latitude.Decimal;
            _tracker.LastLon = message.Longitude.Decimal;


            context.SaveChanges();


            return retval;

        }

        public static CellTracker GetLRCellTracker(string trackerId)
        {
            LimeRCellTracker tracker = null;

            GPSTrackerEntities1 context = new GPSTrackerEntities1();


            // Need to not allow disabled statuses here
            Trackers _trackersDB =
                context.Trackers.Where(t => t.Id == trackerId).FirstOrDefault();

            LR_LimerInfo _trackersInfo =
                context.LR_LimerInfo.Where(t => t.TrackerId == trackerId).FirstOrDefault();

            if (_trackersDB != null && _trackersDB.Status == "LR002")
            {
                tracker = new LimeRCellTracker(_trackersDB.Id, _trackersDB.Name, _trackersDB.Description, _trackersDB.Status, _trackersDB.Id,
                    _trackersDB.Password, _trackersDB.AuthorizedNumbers, _trackersDB.LastUpdate, _trackersDB.LastLat, _trackersDB.LastLon, _trackersInfo.Vibe,_trackersInfo.Status,_trackersInfo.Message);
            }

            return tracker;
        }

    }
}

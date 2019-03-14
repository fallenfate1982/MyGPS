using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTSDataStorage;

namespace GTSBizObjects
{
    public class TrackerInfo
    {
        private string _name;
        private string _description;
        private DateTime _lastUpdate;
        private string _id;
        private string _lastUpdateFormattedString;

        // Last Reported 

        private int rptHrs;
        private int rptMin;
        private int rptSec;
        private int rptDays;

        private double? _mileage;

        private string trackerPicture;

        

       

        #region Properties


        public string TrackerPicture
        {
            get { return trackerPicture; }
            set { trackerPicture = value; }
        }

        public int RptMin
        {
            get { return rptMin; }
            set { rptMin = value; }
        }

        public int RptDays
        {
            get { return rptDays; }
            set { rptDays = value; }
        }
        public int RptHrs
        {
            get { return rptHrs; }
            set { rptHrs = value; }
        }
        public int RptSec
        {
            get { return rptSec; }
            set { rptSec = value; }
        }

        public string LastUpdateFormattedString
        {
            get { return _lastUpdateFormattedString; }
            set { _lastUpdateFormattedString = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }


        public double? Mileage
        {
            get { return _mileage; }
            set { _mileage = value; }
        }

        #endregion

        public TrackerInfo(Trackers info)
        {

            _name = info.Name;
            _description = info.Description;
            _lastUpdate = info.LastUpdate??DateTime.MinValue;
            _id = info.Id;
            _lastUpdateFormattedString = _lastUpdate.ToString("ddd d MMM, yyyy @ HH:mm:ss");
            _mileage = info.Mileage;

            TimeSpan _dur = new TimeSpan(DateTime.Now.Ticks - _lastUpdate.Ticks);
            rptHrs = _dur.Hours;
            rptMin = _dur.Minutes;
            rptSec = _dur.Seconds;
            rptDays = _dur.Days;



            trackerPicture = info.TrackerPicture;
        }

    }
}

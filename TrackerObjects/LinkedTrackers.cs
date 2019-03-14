using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSBizObjects
{
    /// <summary>
    /// This class should be devided into two classes - Linked Trackers and Lime Radar links or something like that
    /// </summary>
    public class LinkedTrackers
    {
        private string _id;
        private string _name;
        private string _desc;
        private string _type;
        private int _status; // LR status of user
        private DateTime _lastUpdate;
        private int _vibe;
        private string _message;
        private int _hoursPassed;
        private int _sameVibe;

        
        //not used atm
        private double _lastLat;
        private double _lastlon;

        private double _distance;

        public LinkedTrackers() { }


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
            get { return _desc; }
            set { _desc = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }

        }
       
        public DateTime? LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value.HasValue?value.Value:DateTime.MinValue; }
        }
        //public double LastLat
        //{
        //    get { return _lastLat; }
        //    set { _lastLat = value; }
        //}
        //public double LastLon
        //{
        //    get { return _lastlon; }
        //    set { _lastlon = value; }
        //}
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }


        public int Vibe
        {
            get { return _vibe; }
            set { _vibe = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public int HoursPassed
        {
            get { return _hoursPassed; }
            set { _hoursPassed = value; }
        }

        //public int SameVibe
        //{
        //    get { return _sameVibe; }
        //    set { _sameVibe = value; }
        //}

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GTSBizObjects
{

#region GTSLocationMessage

    public class GTSLocationMessage
    {
        protected Latitude latitude;
        protected Longitude longitude;
        protected Double latitudeDecimal;
        protected Double longitudeDecimal;

        protected string rawTextData;
        protected string trackerId;

        protected DateTime serverDateTime;
        protected DateTime clientDateTime;

        protected double speed;
        protected long idleTime;
        protected int id;
        protected int altitude;
        protected double hdop;
        protected double vdop;
        protected double direction;

        protected string status;
        protected string extendedFunctions;
        protected double input1;
        protected double input2;

        protected bool? dInput1;
        protected bool? dInput2;
        protected bool? dInput3;
        protected bool? dInput4;
        protected bool? dInput5;

        protected double? mileage;


        protected bool isValid;

        private TrackerInfo trackerDetail;

        private int idleHrs;
        private int idleMin;
        private int idleSec;
        private int idleDays;

        // Ideally Inputs will need to be listed based on the model of the device.
        // When saving, the correct method on the object will be fired and will know how to save by either doing it itself or calling the correct method on
        // GTSBManagementObjects. Currently the generic save is called on the static Manger class. Will need to update accordingly

     

        internal GTSLocationMessage(string tid, byte[] message, int bytesRead)
        {
            isValid = false; // validity is not known until objectification is completed so assume false in case of exception
            objectify(message, bytesRead);
            serverDateTime = DateTime.Now;
            trackerId = tid;

            // Fill Tracker Info
            this.TrackerDetail = Management.GetTrackerInfo(this.TrackerID);

            Debug.WriteLine(">>>>>>>>>>>>>TrackerId: "+trackerId);
        }

        internal GTSLocationMessage() { }


        
        public GTSLocationMessage(int id, int latDeg, int latMin, double latSec,
            int lngDeg, int lngMin, double lngSec, string raw, string trackId,DateTime servDate,
            char latHeading, char lngHeading,double spd, long idle, DateTime clientTime, double hdopval, string statusXml, int alt, double dir,
            double vdop, string extendedFunctions, double input1, double input2,
            bool? dIn1, bool? dIn2, bool? dIn3, bool? dIn4, bool? dIn5, double mileage)
        {
            isValid = true;// this constructor will be access to recreate a valid message that was saved

            latitude.Degrees = latDeg;
            latitude.Minutes = latMin;
            latitude.Seconds = latSec;
            latitude.Heading = latHeading;
            longitude.Degrees = lngDeg;
            longitude.Minutes = lngMin;
            longitude.Seconds = lngSec;
            longitude.Heading = lngHeading;

            rawTextData = raw;
            trackerId = trackId;
            serverDateTime = servDate;

            clientDateTime = clientTime;
            hdop = hdopval;
            status = statusXml;
            this.input1 = input1;
            this.input2 = input2;
            this.dInput1 = dIn1;
            this.dInput2 = dIn2;
            this.dInput3 = dIn3;
            this.dInput4 = dIn4;
            this.dInput5 = dIn5;

            this.mileage = mileage;

            altitude = alt;

            speed = spd;
            
            // Idle Time
            idleTime = idle;
            setTime();

            direction = dir;
            this.id = id;
        }

        protected virtual void objectify(byte[] message, int bytesRead) { }

        protected void setTime()
        {
            TimeSpan time = new TimeSpan(this.idleTime);
            idleHrs = time.Hours;
            idleMin = time.Minutes;
            idleSec = time.Seconds;
            idleDays = time.Days;
        }

        public TrackerInfo TrackerDetail
        {
            get { return trackerDetail; }
            set { trackerDetail = value; }
        }


        public int IdleHrs
        {
            get { return idleHrs; }
            set { idleHrs = value; }
        }
        public int IdleMin
        {
            get { return idleMin; }
            set { idleMin = value; }
        }
        public int IdleSec
        {
            get { return idleSec; }
            set { idleSec = value; }
        }

        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
            set { id = value; }
        }

        public Latitude Latitude
        {
            get
            {
                return latitude;
            }
        }
        public Longitude Longitude
        {
            get
            {
                return longitude;
            }
        }

        public Double LatitudeDecimal
        {
            get
            {
                this.latitudeDecimal = latitude.Decimal;
                return latitude.Decimal;
            }
        }
        public Double LongitudeDecimal
        {
            get
            {
                this.longitudeDecimal = longitude.Decimal;
                return longitude.Decimal;
            }
        }

        public string TrackerID
        {
            get
            {
                return trackerId;
            }
        }

        public string ExtendedFunctions
        {
            get
            {
                return extendedFunctions;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
        }

        public double Direction
        {
            get
            {
                return direction;
            }
        }

        public double HDOP
        {
            get
            {
                return hdop;
            }
        }

        public double VDOP
        {
            get
            {
                return vdop;
            }
        }

        public double IdleDays
        {
            get
            {
                return idleDays;
            }
        }

        public long IdleTime
        {
            get
            {
                return idleTime;
            }

            set
            {
                idleTime = value;
                setTime();
            }
        }

        public DateTime ServerRecordedDateTime
        {
            get
            {
                return serverDateTime;
            }
        }

        public string FormattedServerRecordedDateTime
        {
            get
            {
                return serverDateTime.ToString("dd MMM, yyyy hh:mm:ss tt");
            }
        }

        public string FormattedClientRecordedDateTime
        {
            get
            {
                return clientDateTime.ToString("dd MMM, yyyy hh:mm:ss tt");
            }
        }

        public DateTime ClientRecordedDateTime
        {
            get
            {
                return clientDateTime;
            }
        }

        public double Speed
        {
            get
            {
                return speed;
            }
        }

        public int Altitude
        {
            get
            {
                return altitude;
            }
        }

        public double Input1
        {
            get
            {
                return input1;
            }
        }

        public double Input2
        {
            get
            {
                return input2;
            }
        }

        public bool? DInput1
        {
            get
            {
                return dInput1;
            }
        }

        public bool? DInput2
        {
            get
            {
                return dInput2;
            }
        }

        public bool? DInput3
        {
            get
            {
                return dInput3;
            }
        }

        public bool? DInput4
        {
            get
            {
                return dInput4;
            }
        }

        public bool? DInput5
        {
            get
            {
                return dInput5;
            }
        }

        public double? Mileage
        {
            get
            {
                return mileage;
            }
            set
            {
                this.mileage = value;
            }
        }

        public string RawLocationMessageData
        {
            get
            {
                return rawTextData;
            }
        }


        public override string ToString()
        {
            return "Lat: " + this.latitude.Decimal + " Lon:" + this.longitude.Decimal;
        }
    }

    /// <summary>
    /// TODO: We need to figure out why there is a lite version of this object as it seems like its the same as the actual object!
    /// </summary>
    public class GTSLocationMessageLite
    {
        protected Latitude latitude;
        protected Longitude longitude;
        protected Double latitudeDecimal;
        protected Double longitudeDecimal;

        protected string rawTextData;
        protected string trackerId;

        protected DateTime serverDateTime;
        protected DateTime clientDateTime;

        protected double speed;
        protected long idleTime;
        protected int idleHrs;
        protected int idleMin;
        protected int idleSec;
        private int idleDays;

        protected int id;
        protected int altitude;
        protected double hdop;
        protected double vdop;
        protected double direction;

        protected string status;
        protected string extendedFunctions;

        protected double input1;
        protected double input2;

        protected bool? dInput1;
        protected bool? dInput2;
        protected bool? dInput3;
        protected bool? dInput4;
        protected bool? dInput5;

        protected bool isValid;

        internal GTSLocationMessageLite(string tid, byte[] message, int bytesRead)
        {
            isValid = false; // validity is not known until objectification is completed so assume false in case of exception
            serverDateTime = DateTime.Now;
            trackerId = tid;

            Debug.WriteLine(">>>>>>>>>>>>>TrackerId: " + trackerId);
        }

        public GTSLocationMessageLite(int id, int latDeg, int latMin, double latSec,
            int lngDeg, int lngMin, double lngSec, string raw, string trackId, DateTime servDate,
            char latHeading, char lngHeading, double spd, long idle, DateTime clientTime, double hdopval, string statusXml, int alt, double dir,
            double vdop, string extendedFunctions, double input1, double input2,
            bool? dIn1, bool? dIn2, bool? dIn3, bool? dIn4, bool? dIn5)
        {
            isValid = true;// this constructor will be access to recreate a valid message that was saved

            latitude.Degrees = latDeg;
            latitude.Minutes = latMin;
            latitude.Seconds = latSec;
            latitude.Heading = latHeading;
            longitude.Degrees = lngDeg;
            longitude.Minutes = lngMin;
            longitude.Seconds = lngSec;
            longitude.Heading = lngHeading;

            rawTextData = raw;
            trackerId = trackId;
            serverDateTime = servDate;

            clientDateTime = clientTime;
            hdop = hdopval;
            status = statusXml;
            this.input1 = input1;
            this.input2 = input2;
            this.dInput1 = dIn1;
            this.dInput2 = dIn2;
            this.dInput3 = dIn3;
            this.dInput4 = dIn4;
            this.dInput5 = dIn5;
            altitude = alt;

            speed = spd;
            
            // Idle Time
            idleTime = idle;
            TimeSpan time = new TimeSpan(idle);
            idleHrs = time.Hours;
            idleMin = time.Minutes;
            idleSec = time.Seconds;
            idleDays = time.Days;

            direction = dir;
            this.id = id;
        }

        public string FormattedServerRecordedDateTime
        {
            get
            {
                return serverDateTime.ToString("dd MMM, yyyy hh:mm:ss tt");
            }
        }

        public string FormattedClientRecordedDateTime
        {
            get
            {
                return clientDateTime.ToString("dd MMM, yyyy hh:mm:ss tt");
            }
        }

        public Double Lat
        {
            get
            {
                this.latitudeDecimal = latitude.Decimal;
                return latitude.Decimal;
            }
        }
        public Double Lng
        {
            get
            {
                this.longitudeDecimal = longitude.Decimal;
                return longitude.Decimal;
            }
        }

        public double Dir
        {
            get
            {
                return direction;
            }
        }

        public string MarkerDate
        {
            get
            {
                return serverDateTime.ToString("dd MMM, yyyy hh:mm:ss tt");
            }
        }

        public double Speed
        {
            get
            {
                return speed;
            }
        }

        public int IdleHrs
        {
            get
            {
                return idleHrs;
            }
        }

        public int IdleMin
        {
            get
            {
                return idleMin;
            }
        }

        public int IdleDays
        {
            get
            {
                return idleDays;
            }
        }

        public int IdleSec
        {
            get
            {
                return idleSec;
            }
        }

        public double Input1
        {
            get
            {
                return input1;
            }
        }

        public double Input2
        {
            get
            {
                return input2;
            }
        }

        public bool? DInput1
        {
            get
            {
                return dInput1;
            }
        }

        public bool? DInput2
        {
            get
            {
                return dInput2;
            }
        }

        public bool? DInput3
        {
            get
            {
                return dInput3;
            }
        }

        public bool? DInput4
        {
            get
            {
                return dInput4;
            }
        }

        public bool? DInput5
        {
            get
            {
                return dInput5;
            }
        }
    }
#endregion 

#region Latitude
    public struct Latitude
    {
        private int degrees;
        private int minutes;
        private double seconds;
        private char heading;

        public int Degrees
        {
            set{
                degrees = value;
            }
            get{
                return degrees;
            }
        }

        public int Minutes
        {
            set{
                minutes = value;
            }
            get{
                return minutes;
            }
        }

        public double Seconds
        {
            set{
                seconds = value;
            }
            get{
                return seconds;
            }
        }

        public char Heading
        {
            set
            {
                heading = value;
            }
            get
            {
                return heading;
            }
        }

        public double Decimal
        {
            get
            {
                return (this.degrees + (this.minutes / 60.0) + (this.seconds / 3600))
                    * (this.heading=='S'?-1:1);
            }
            set
            {
                GeoAngle conv = GeoAngle.FromDouble(value);
                this.heading = conv.IsNegative ? 'S' : 'N';
                this.degrees = conv.Degrees;
                this.minutes = conv.Minutes;
                this.seconds = conv.Seconds;
            }
        }
    }
#endregion

#region Longitude
    public struct Longitude
    {
        private int degrees;
        private int minutes;
        private double seconds;
        private char heading;

        public int Degrees
        {
            set{
                degrees = value;
            }
            get{
                return degrees;
            }
        }

        public int Minutes
        {
            set{
                minutes = value;
            }
            get{
                return minutes;
            }
        }

        public double Seconds
        {
            set{
                seconds = value;
            }
            get{
                return seconds;
            }
        }

        public char Heading
        {
            set
            {
                heading = value;
            }
            get

            {
                return heading;
            }
        }

        public double Decimal
        {
            get
            {
                return (this.degrees + (this.minutes / 60.0) + (this.seconds / 3600)) 
                    * (this.heading == 'W' ? -1 : 1);
            }
            set
            {
                GeoAngle conv = GeoAngle.FromDouble(value);
                this.heading = conv.IsNegative ? 'W' : 'E';
                this.degrees = conv.Degrees;
                this.minutes = conv.Minutes;
                this.seconds = conv.Seconds;
            }
        }
    }
    #endregion

    public class GeoAngle
    {
        public bool IsNegative { get; set; }
        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public double Seconds { get; set; }
        public int Milliseconds { get; set; }



        public static GeoAngle FromDouble(double angleInDegrees)
        {
            //ensure the value will fall within the primary range [-180.0..+180.0]
            while (angleInDegrees < -180.0)
                angleInDegrees += 360.0;

            while (angleInDegrees > 180.0)
                angleInDegrees -= 360.0;

            var result = new GeoAngle();

            //switch the value to positive
            result.IsNegative = angleInDegrees < 0;
            angleInDegrees = Math.Abs(angleInDegrees);

            //gets the degree
            result.Degrees = (int)Math.Floor(angleInDegrees);
            var delta = angleInDegrees - result.Degrees;

            //gets minutes and seconds
            var seconds = (int)Math.Floor(3600.0 * delta);
            result.Seconds = seconds % 60;
            result.Minutes = (int)Math.Floor(seconds / 60.0);
            delta = delta * 3600.0 - seconds;

            //gets fractions
            //result.Milliseconds = (int)(1000.0 * delta);

            // This is done as we did not cater for milliseconds in the DB BUT we do accept seconds with
            // a decimal.
            result.Seconds += delta;

            return result;
        }
    }

}

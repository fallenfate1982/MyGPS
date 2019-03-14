using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GTSBizObjects
{
    public class CellLocationMessage:GTSLocationMessage
    {

        public CellLocationMessage(NameValueCollection data)
        {
            isValid = true;

            try
            {
                this.trackerId = data["id"];
                clientDateTime = DateTime.Now;
                serverDateTime = DateTime.Now;
                double.TryParse(data["head"], out direction);

                double.TryParse(data["speed"], out speed);
                Debug.WriteLine("Speed: " + speed);

                // Data Conversion

                double latAbs = double.Parse(data["lat"]);

                latitude.Degrees = (int)Math.Truncate(latAbs);
                Debug.WriteLine("latitude.Degrees: " + latitude.Degrees);

                double minsDouble = (double)((latAbs - latitude.Degrees) * 60);
                double secDouble = (double)(minsDouble - Math.Truncate(minsDouble));

                latitude.Minutes = (int)Math.Truncate(minsDouble);
                Debug.WriteLine("latitude.Minutes: " + latitude.Minutes);

                latitude.Seconds = secDouble*60;
                Debug.WriteLine("latitude.Seconds: " + latitude.Seconds);



                latitude.Heading = 'x';
                Debug.WriteLine("latitude.Heading: " + latitude.Heading);


                double lonAbs = double.Parse(data["lon"]);

                longitude.Degrees = (int)Math.Truncate(lonAbs);
                Debug.WriteLine("longitude.Degrees: " + longitude.Degrees);

                double minsDoublelon = (double)((lonAbs - longitude.Degrees) * 60);
                double secDoublelon = (double)(minsDoublelon - Math.Truncate(minsDoublelon));

                longitude.Minutes = (int)Math.Truncate(minsDoublelon);
                Debug.WriteLine("longitude.Minutes: " + longitude.Minutes);

                longitude.Seconds = secDoublelon * 60;
                Debug.WriteLine("longitude.Seconds: " + longitude.Seconds);

                longitude.Heading = 'x';
                Debug.WriteLine("longitude.Heading: " + longitude.Heading);

                if (double.TryParse(data["accu"], out hdop)) { hdop = 0.0; };

                if(int.TryParse(data["alt"],out altitude)){altitude=0;};

                status = "";

                rawTextData = "";

            }
            catch (Exception e) { 
                
                isValid = false;
                ExceptionHandler.HandleGeneralException(e);
            }

            
        }


        public CellLocationMessage(string phnum, string lat, string lon, string heading, string speed1, string clientTime)
        {
            isValid = true;

            try
            {
                this.trackerId = phnum;
                clientDateTime = DateTime.Now; // Need to replace this with "Client Time"
                serverDateTime = DateTime.Now;
                double.TryParse(heading, out direction);

                double.TryParse(speed1, out speed);
                Debug.WriteLine("Speed: " + speed);

                // Data Conversion

                double latAbs = double.Parse(lat);

                latitude.Degrees = (int)Math.Truncate(latAbs);
                Debug.WriteLine("latitude.Degrees: " + latitude.Degrees);

                double minsDouble = (double)((latAbs - latitude.Degrees) * 60);
                double secDouble = (double)(minsDouble - Math.Truncate(minsDouble));

                latitude.Minutes = (int)Math.Truncate(minsDouble);
                Debug.WriteLine("latitude.Minutes: " + latitude.Minutes);

                latitude.Seconds = secDouble * 60;
                Debug.WriteLine("latitude.Seconds: " + latitude.Seconds);



                latitude.Heading = 'x';
                Debug.WriteLine("latitude.Heading: " + latitude.Heading);


                double lonAbs = double.Parse(lon);

                longitude.Degrees = (int)Math.Truncate(lonAbs);
                Debug.WriteLine("longitude.Degrees: " + longitude.Degrees);

                double minsDoublelon = (double)((lonAbs - longitude.Degrees) * 60);
                double secDoublelon = (double)(minsDoublelon - Math.Truncate(minsDoublelon));

                longitude.Minutes = (int)Math.Truncate(minsDoublelon);
                Debug.WriteLine("longitude.Minutes: " + longitude.Minutes);

                longitude.Seconds = secDoublelon * 60;
                Debug.WriteLine("longitude.Seconds: " + longitude.Seconds);

                longitude.Heading = 'x';
                Debug.WriteLine("longitude.Heading: " + longitude.Heading);

                hdop = 0.0; ;

                altitude = 0;

                status = "";

                rawTextData = "";

            }
            catch (Exception e)
            {

                isValid = false;
                ExceptionHandler.HandleGeneralException(e);
            }


        }
    }
}

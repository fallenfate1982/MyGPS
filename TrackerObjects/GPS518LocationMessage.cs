using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GTSBizObjects
{
    class GPS518LocationMessage:GTSLocationMessage
    {
        internal GPS518LocationMessage(string tid, byte[] message, int bytesRead):base(tid,message,bytesRead)
         {
         }

        protected override void objectify(byte[] message, int bytesRead)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            rawTextData = encoder.GetString(message, 0, bytesRead);

            Debug.WriteLine("Objectification Report for GPS518");
            Debug.WriteLine("Raw Text: " + rawTextData);

            try
            {

                Debug.WriteLine("Validity: " +rawTextData.Substring(38,1));
                if (rawTextData.Substring(38, 1) == "V") 
                    throw new Exception("Reading is not valid, halting saving process");
                isValid = true;
                // need to update so that if not valid the thread will stop processing and wait for next message

                // need to update code to pull and process date time
                clientDateTime = DateTime.Now;

                double.TryParse(rawTextData.Substring(71, 3),out direction);
                Debug.WriteLine("Direction: " + direction);

                double.TryParse(rawTextData.Substring(60, 3), out speed);
                Debug.WriteLine("Speed: " + speed);

                // Data Conversion
                latitude.Degrees = int.Parse(rawTextData.Substring(39, 2));
                Debug.WriteLine("latitude.Degrees: " + latitude.Degrees);

                latitude.Minutes = int.Parse(rawTextData.Substring(41, 2));
                Debug.WriteLine("latitude.Minutes: " + latitude.Minutes);

                latitude.Seconds = float.Parse(rawTextData.Substring(44, 4)) / 10000 * 60;
                Debug.WriteLine("latitude.Seconds: " + latitude.Seconds);

                latitude.Heading = rawTextData[48];
                Debug.WriteLine("latitude.Heading: " + latitude.Heading);


                longitude.Degrees = int.Parse(rawTextData.Substring(49, 3));
                Debug.WriteLine("longitude.Degrees: " + longitude.Degrees);

                longitude.Minutes = int.Parse(rawTextData.Substring(52, 2));
                Debug.WriteLine("longitude.Minutes: " + longitude.Minutes);

                longitude.Seconds = float.Parse(rawTextData.Substring(55, 4)) / 10000 * 60;
                Debug.WriteLine("longitude.Seconds: " + longitude.Seconds);

                longitude.Heading = rawTextData[59];
                Debug.WriteLine("longitude.Heading: " + longitude.Heading);

                hdop = 0;

                altitude = 0;

                status = "";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

        }

    }
}

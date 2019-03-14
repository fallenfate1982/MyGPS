using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

namespace GTSBizObjects
{
    class ReverseGeo
    {
        public static string lookUpURL = "http://open.mapquestapi.com/geocoding/v1/reverse?key=Fmjtd%7Cluur2ha7nu%2Cb0%3Do5-9wb01f&location=";

        public static string GetAddress(string latitude, string longitude)
        {
            var request = (HttpWebRequest)WebRequest.Create(lookUpURL + latitude + "," + longitude);
            request.Method = "GET";
            //request.Credentials = new NetworkCredential("demo@leankit.com", "demopassword");
            //request.PreAuthenticate = true;

            var requestBody = Encoding.UTF8.GetBytes(string.Empty);
            request.ContentLength = requestBody.Length;
            request.ContentType = "application/json";
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBody, 0, requestBody.Length);
            }

            request.Timeout = 5000;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            string output = string.Empty;
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        //output = stream.ReadToEnd();
                        //JObject jsonObj = JObject.Parse(output);

                        //var formattedAddress = jsonObj["results"][0].["locations"][0];

                        var address = "";
                        //if (formattedAddress.street != null || formattedAddress.street != "") {
                        //    address += formattedAddress.street;
                        //}
                        //if (formattedAddress.adminArea5) {
                        //    address += ", " + formattedAddress.adminArea5;
                        //}
                        //if (formattedAddress.adminArea4) {
                        //    address += ", " + formattedAddress.adminArea4;
                        //}
                        //if (formattedAddress.adminArea3) {
                        //    address += ", " + formattedAddress.adminArea3;
                        //}
                        //if (formattedAddress.adminArea1) {
                        //    address += ", " + formattedAddress.adminArea1;
                        //}
                        return address.ToString().ToUpper();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    using (var stream = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        output = stream.ReadToEnd();
                    }
                }
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    output = "Request timeout is expired.";
                }
            }
            Console.WriteLine(output);

            return output;
        }

    }   
}

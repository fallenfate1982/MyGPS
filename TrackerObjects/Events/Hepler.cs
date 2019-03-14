using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace GTSBizObjects.Events
{
    public class Hepler
    {

        /*
         Cases we need to take care of
         * Vehicle enters geofence
         * Vehicle exits geofence
         * Vehicle leave a geofence and goes into another geofence
         * Vehicle is in geofence (for x time)
         
         
         */


        public static bool IsPointInPolygon(List<Location> poly, Location point)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                if ((((poly[i].Lt <= point.Lt) && (point.Lt < poly[j].Lt)) ||
                    ((poly[j].Lt <= point.Lt) && (point.Lt < poly[i].Lt))) &&
                    (point.Lg < (poly[j].Lg - poly[i].Lg) * (point.Lt - poly[i].Lt) / (poly[j].Lt - poly[i].Lt) + poly[i].Lg))
                    c = !c;
            }
            return c;
        }

        //public static bool IsPointInPolygon(List<Location> polygon, Location point)
        //{
        //    bool isInside = false;
        //    for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        //    {
        //        if (((polygon[i].Lt > point.Lt) != (polygon[j].Lt > point.Lt)) &&
        //        (point.Lg < (polygon[j].Lg - polygon[i].Lg) * (point.Lg - polygon[i].Lg) / (polygon[j].Lg - polygon[i].Lg) + polygon[i].Lg))
        //        {
        //            isInside = !isInside;
        //        }
        //    }
        //    return isInside;
        //}

        public static List<Location> GeoFenceLocations(string coordinates)
        {
            // Comment Data for points are stored poorly, resulting in this inefficient way of breaking it out
            // TODO - this method needs some properexception handling
            var locations = new List<Location>();
            string[] separateCoords = coordinates.Split('}');

            foreach (string coord in separateCoords)
            {
                if (coord.Trim() == "") break;
                string coord2 = coord.Substring(1);

                // Split into lat an lon and then try to pull out the full numeric part
                string[] latlon = coord2.Split(',');

                string lat = latlon[0].Substring(latlon[0].IndexOf(':') + 1);
                string lon = latlon[1].Substring(latlon[1].IndexOf(':') + 1);

                locations.Add(new Location { Lt = Convert.ToDouble(lat), Lg = Convert.ToDouble(lon) });
            }
       
            return locations;
        }
    }

    public class Location
    {
        public double Lt { get; set; }
        public double Lg { get; set; }
    }
}

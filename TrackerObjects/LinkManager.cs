using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GTSDataStorage;

namespace GTSBizObjects
{
    public class LinkManager
    {
        public static List<LinkedTrackers> GetTrackersinDistance(int distance, double? lng, double? lat, string id, int vibe)
        {
            using (GPSTrackerEntities1 context = new GPSTrackerEntities1())
            {
                /// distnce in KM?
                /// Note. TrackerId is primary and TrackerId2 is secondary. LinkLevel defines how much access TrackerId2 has allowed TrackerId
                /// Vibe set to 0 to disable sorting by vibe

                ///////////////NEED TO UPDATE SOON - need to update to pass parameters properly and to cut of excess data///////////////////////
                string command = @"SELECT a.TrackerId2 Id, ISNull(Name,'') Name,ISNull([Description],'') [Description],LastUpdate, LastLat, LastLon, ISNull(Vibe,-1) Vibe,ISNull([Message],'') [Message],ISNull(c.[Status],-1) [Status],    
                                ISNull(( 6371 * acos( cos( radians( {0} ) ) *
                                  cos( radians( LastLat ) ) * cos( radians(  LastLon  ) - radians( {1} ) ) +
                                  sin( radians( {0} ) ) * sin( radians(  LastLat  ) ) ) ),-1) AS Distance,
                                  ISNull(datediff(hh,LastUpdate, IsNull(GETDATE(), LastUpdate)),-1) as HoursPassed,
                                  NullIf({3},Vibe) as SameVibe 
                                FROM ((select * from TrackerLink where TrackerId = '{2}' 
                                and LinkLevel >9) a Left Join Trackers b
                                On a.TrackerId2 = b.Id) Left Join LR_LimerInfo c On c.TrackerId = a.TrackerId2 

                                ORDER BY SameVibe ASC, HoursPassed ASC, Distance ASC";

                command = String.Format(command, lat, lng, id, vibe);

                List<LinkedTrackers> _linked = context.ExecuteStoreQuery<LinkedTrackers>(command).ToList();

                return _linked;
            }
        }

        public static List<LinkedTrackers> GetTrackersinDistancePublic(int distance, double? lng, double? lat, string id, int vibe)
        {
            using (GPSTrackerEntities1 context = new GPSTrackerEntities1())
            {
                /// distnce in KM?
                /// Note. TrackerId is primary and TrackerId2 is secondary. LinkLevel defines how much access TrackerId2 has allowed TrackerId
                /// Vibe set to 0 to disable sorting by vibe

                ///////////////NEED TO UPDATE SOON - need to update to pass parameters properly and to cut of excess data///////////////////////
                string command = @"SELECT Id, ISNull(Name,'') Name,ISNull([Description],'') [Description],LastUpdate, LastLat, LastLon, ISNull(Vibe,-1) Vibe,ISNull([Message],'') [Message],ISNull(b.[Status],-1) [Status],    
                                ISNull(( 6371 * acos( cos( radians( {0} ) ) *
                                  cos( radians( LastLat ) ) * cos( radians(  LastLon  ) - radians( {1} ) ) +
                                  sin( radians( {0} ) ) * sin( radians(  LastLat  ) ) ) ),-1) AS Distance,
                                  ISNull(datediff(hh,LastUpdate, IsNull(GETDATE(), LastUpdate)),-1) as HoursPassed,
                                  NullIf({2},Vibe) as SameVibe 
                                FROM (select * from Trackers where Status = 'LR002' and Id != '{3}') a  Left Join LR_LimerInfo b On b.TrackerId = a.Id 

                                ORDER BY SameVibe ASC, HoursPassed ASC, Distance ASC";

                command = String.Format(command, lat, lng, vibe,id);

                List<LinkedTrackers> _linked = context.ExecuteStoreQuery<LinkedTrackers>(command).ToList();

                return _linked;
            }
        }
    }
}

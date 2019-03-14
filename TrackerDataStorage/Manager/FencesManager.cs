using System;
using System.Collections.Generic;
using System.Linq;

namespace GTSDataStorage.Manager
{
  public class FencesManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    public static List<GeoFence> GetAll()
    {
      using (var dbContext = new GPSTrackerEntities1())
      {
        return dbContext.GeoFences.ToList();
      }
    }

    public static List<GeoFence> GetFencesByTrackerId(string trackerId)
    {
        List<GeoFence> retList = new List<GeoFence>();

        // get the users tied to the tracker
        var q = from x in db.TrackerUser
                where x.TrackerId == trackerId 
                select x;

       
        // get all tied geofences for each user

        foreach (var p in q)
        {
            string id = p.UserId.ToString();
            var m = from x in db.GeoFences
                    where x.CreatedBy == id
                    select x;


            retList.AddRange(m);
        }

        return retList.ToList();
    }

    public static List<GeoFenceType> GetFencesType()
    {
      return db.GeoFenceTypes.OrderBy(x => x.FencesTypeName).ToList();
    }

    public static GeoFenceType GetFencesTypeById(int typeId)
    {
      var q = from x in db.GeoFenceTypes
              where x.FencesTypeId == typeId
              select x;
      return q.Any() ? q.First() : new GeoFenceType();
    }

    public static List<GeoFence> GetByUserId(string userId)
    {
      var q = from x in db.GeoFences
              where x.CreatedBy == userId && (x.Status ?? false) 
              orderby x.CreatedDate descending
              select x;

      return q.ToList();
    }

    public static List<GeoFence> GetByTrackerId(string trackerId)
    {
        //get list of GeoFence Ids that are current associated with the current tracker
        /*List<TrackerFenceMap> mapTrackerFences = db.TrackerFenceMaps.Where(j => j.TrackerId == trackerId).ToList();
        var geofenceIds = new List<string>();
 
        foreach (TrackerFenceMap item in mapTrackerFences)
        {
            geofenceIds.Add(item.FencesId.ToString());
        }
        //get list of GeoFence records
        var geofencesForTracker = db.GeoFences.Where(x => geofenceIds.Contains(x.FencesId.ToString())).ToList();
 
        return geofencesForTracker;*/

        var q = from t in db.TrackerFenceMaps
                join f in db.GeoFences on t.FencesId equals f.FencesId
                where t.TrackerId == trackerId
                select f;

        return q.ToList();
    }

    public static GeoFence GetById(int id )
    {
      var q = db.GeoFences.Where(x => x.FencesId == id);
      return q.Any() ? q.First() : new GeoFence();
    }

    public static void DeActivate(int id, string userName)
    {
      var q = GetById(id);
      q.Status = false;
      q.UpdatedBy = userName;
      Save(q);
    }

    public static void Save(GeoFence geoFence)
    {
      if (geoFence.FencesId == 0)
      {
        geoFence.CreatedDate = DateTime.Now;
        db.GeoFences.AddObject(geoFence);
      }

      geoFence.UpdatedDate = DateTime.Now;
      db.SaveChanges();
    }
  }
}

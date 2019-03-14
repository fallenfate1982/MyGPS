using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
  public class TrackerFenceMapManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    public static List<TrackerFenceMap> GetByFenceId(int fenceId)
    {
      return db.TrackerFenceMaps.Where(x => x.FencesId == fenceId).ToList();
    }
   
    public static void Add(TrackerFenceMap trackerFenceMap)
    {
      db.AddToTrackerFenceMaps(trackerFenceMap);
      db.SaveChanges();
    }

    public static void RemoveTrackerByFenceId(int fenceId)
    {
      var trackers = db.TrackerFenceMaps.Where(x => x.FencesId == fenceId);
      foreach (var item in trackers)
      {
        db.TrackerFenceMaps.DeleteObject(item);
      }
      db.SaveChanges();
    }


  }
}

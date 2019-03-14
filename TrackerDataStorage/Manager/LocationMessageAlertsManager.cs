using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
  public class LocationMessageAlertsManager
  {

    public static List<LocationMessageAlert> GetLogs(int top = 5)
    {
      var db = new GPSTrackerEntities1();
      var data = db.LocationMessageAlerts.Where(x => x.IsProcessed == false).OrderBy(m => m.LocationMessageId).Take(top).ToList();
      return data;
    }

    public static int GetMaxLocationMessageId()
    {
      var db = new GPSTrackerEntities1();
      return db.LocationMessageAlerts.Max(x => x.LocationMessageId) ?? 0;
    }

    public static void Save(LocationMessageAlert locationMessageAlert)
    {
      var db = new GPSTrackerEntities1();
      db.LocationMessageAlerts.AddObject(locationMessageAlert);
      db.SaveChanges();
    }

    public static void UpdateByTrackerId(string trackerId)
    {
      var db = new GPSTrackerEntities1();
      var data = db.LocationMessageAlerts.FirstOrDefault(x => x.TrackerId == trackerId && x.IsProcessed==false);
      data.IsProcessed = true;
      db.SaveChanges();
    }
  }
}

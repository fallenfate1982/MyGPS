using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
 public  class AlertFiredManager
  {
   public static void Save(AlertFired alertFired)
   {
     var db = new GPSTrackerEntities1();
     db.AlertFireds.AddObject(alertFired);
     db.SaveChanges();
   }

   public static bool IsAlreadyEnterdInFences(string trackerId)
   {
     var db = new GPSTrackerEntities1();
     var q = from x in db.AlertFireds
             where x.TrackerId == trackerId && (x.Active ?? false) && (x.IsEnter ?? false)
             select x;

     return q.Any();
   }

   public static List<AlertFired> GetByTrackerId(string trackerId)
   {
     var db = new GPSTrackerEntities1();
     var data = db.AlertFireds.Where(x => x.TrackerId == trackerId).ToList();
     return data;
   }

   public static AlertFired GetActiveEnterByTrackerId(string trackerId)
   {
     var db = new GPSTrackerEntities1();
     var q = from x in db.AlertFireds
             where x.TrackerId == trackerId && (x.Active ?? false)
             select x;

     return q.Any() ? q.First() : null;
   }
  
   public static void ProcessedAlertExitFired(int id)
    {
      var db = new GPSTrackerEntities1();
      var data = db.AlertFireds.FirstOrDefault(x => x.Id == id);
     // data.IsEnter = true;
      data.IsExit = true;
      data.Active = false;
      db.SaveChanges();
    }

  }
}

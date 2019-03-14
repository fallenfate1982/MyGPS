using System;
using System.Collections.Generic;
using System.Linq;

namespace GTSDataStorage.Manager
{
  public class AlertManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    public static List<Alert> GetAll()
    {
      return db.Alerts.ToList();
    }

    public static Alert GetById(int id )
    {
      var q = db.Alerts.Where(x => x.AlertId == id);
      return q.Any() ? q.First() : new Alert();
    }

    public static void DeActivate(int id)
    {
      var q = db.Alerts.First(x => x.AlertId == id);
      q.Status = false;
      db.SaveChanges();
    }

    public static void Save(Alert alert)
    {
      if (alert.AlertId == 0)
        db.Alerts.AddObject(alert);

      db.SaveChanges();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
 public class AlertLogManager
  {
   public static void Save(AlertLog alertlog)
   {
     var db = new GPSTrackerEntities1();
     db.AlertLogs.AddObject(alertlog);
     db.SaveChanges();
   }
  }
}

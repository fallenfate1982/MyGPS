using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
  public class LocationMessageManager
  {
    public static List<LocationMessages> GetByMaxLocMessageId(int maxLocMessageId)
    {
      var db = new GPSTrackerEntities1();
      return db.LocationMessages.Where(x => x.Id > maxLocMessageId).Take(5).ToList();
    }
  }
}

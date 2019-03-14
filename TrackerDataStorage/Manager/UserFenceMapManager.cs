using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
  public class UserFenceMapManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
   
    public static List<UserFenceMap> GetByFenceId(int fenceId)
    {
      return db.UserFenceMaps.Where(x => x.FencesId == fenceId).ToList();
    }

    public static void Add(UserFenceMap userFenceMap)
    {
      db.AddToUserFenceMaps(userFenceMap);
      db.SaveChanges();
    }

    public static void RemoveUserByFenceId(int fenceId)
    {
      var users = db.UserFenceMaps.Where(x => x.FencesId == fenceId);
      foreach (var item in users)
      {
        db.UserFenceMaps.DeleteObject(item);
      }
      db.SaveChanges();
    }


  }
}

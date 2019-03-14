using System;
using System.Collections.Generic;
using System.Linq;

namespace GTSDataStorage.Manager
{
  public class GroupAlertManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();

    public static GroupAlert GetById(int id)
    {
      var q = db.GroupAlerts.Where(x => x.GroupAlertId == id);
      return q.Any() ? q.First() : new GroupAlert();
    }
    public static List<GroupAlert> GetAll()
    {
      return db.GroupAlerts.ToList();
    }
    public static List<GroupAlert> GetAllByFencesId(int FencesId)
    {
      return db.GroupAlerts.Where(m => m.FencesId == FencesId).ToList();
    }
    public static List<GroupAlert> GetByGroupId(int groupId)
    {
      return db.GroupAlerts.Where(x => x.GroupId== groupId).ToList();
    }

    public static void RemoveGroupAlertsByGroupId(int groupId)
    {
      var gAlerts = db.GroupAlerts.Where(x => x.GroupId == groupId);
      foreach (var item in gAlerts)
      {
        db.GroupAlerts.DeleteObject(item);
      }
      db.SaveChanges();
    }

    public static void Add(GroupAlert groupAlert)
    {
      db.AddToGroupAlerts(groupAlert);
      db.SaveChanges();
    }
  }
}

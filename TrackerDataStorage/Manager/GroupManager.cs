using System;
using System.Collections.Generic;
using System.Linq;


namespace GTSDataStorage.Manager
{
  public class GroupManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    public static List<Group> GetAll()
    {
      return db.Groups.ToList();
    }

    public static Group GetById(int id)
    {
      var q = db.Groups.Where(x => x.GroupId == id);
      return q.Any() ? q.First() : new Group();
    }

    public static void DeActivate(int id)
    {
      var q = db.Groups.First(x => x.GroupId == id);
      q.Status = false;
      db.SaveChanges();
    }

    public static void Save(Group group)
    {
      if (group.GroupId == 0)
        db.Groups.AddObject(group);

      db.SaveChanges();
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
  public class Aspnet_UserManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    
    public static List<aspnet_Users> GetAll()
    {
      return db.aspnet_Users.ToList();
    }

    public static aspnet_Users GetByUserName(string userName)
    {
      var q = db.aspnet_Users.Where(x => x.UserName == userName);
      return q.Any() ? q.First() : new aspnet_Users();
    }

    
  }
}

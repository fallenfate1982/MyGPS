using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTSDataStorage.Manager
{
 public class TrackerManager
  {
    private static GPSTrackerEntities1 db = new GPSTrackerEntities1();
    public static List<Trackers> GetAll()
    {
      return db.Trackers.ToList();
    }

    public static Trackers GetById(string id)
    {
      var q = db.Trackers.Where(x => x.Id == id);
      return q.Any() ? q.First() : new Trackers();
    }

    public static List<Trackers> GetByUserId(string userId)
    {
        Guid userGUID = new Guid(userId);
        var trackersForUser = db.TrackerUser.Where(j => j.UserId == userGUID).ToList();
        var userTrackerIds = new List<string>();

        foreach(TrackerUser item in trackersForUser) {
            userTrackerIds.Add(item.TrackerId);
        }
        var trackerDetails = db.Trackers.Where(x => userTrackerIds.Contains(x.Id)).ToList();

        return trackerDetails;
    }
  }
}

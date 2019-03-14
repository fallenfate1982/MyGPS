using System;
using System.Collections.Generic;
using System.Web.Http;
using GTSDataStorage;
using GTSDataStorage.Manager;
using System.Web.Security;
using System.Linq;

namespace MyGPS.Api
{
  public class FencesController : ApiController
  {
    
    [HttpGet]
    public List<FencesModel> GetAllFances()
    {
      var lstFences = new List<FencesModel>();
      var getFences = FencesManager.GetByUserId(Membership.GetUser().ProviderUserKey.ToString());
      foreach (var item in getFences)
      {
        lstFences.Add(new FencesModel
        {
          //FencesTypeId= item.TypeId?? 0,
          FencesId = item.FencesId,
          FencesName = item.FencesName,
          FencesCoordinate = item.FencesCoordinate,
          IsPublic = item.IsPublic,
          Details = item.Details,
          Status = item.Status,
          Zoom = item.Zoom,
          CreatedBy = item.CreatedBy,
          CreatedDate = item.CreatedDate,
          UpdatedBy = item.UpdatedBy,
          UpdatedDate = item.UpdatedDate,
        });
      }
      return lstFences;//FencesManager.GetByUserName(User.Identity.Name);
    }

    [HttpGet]
    public FenceTrackerModel GetTrackersByFenceId(int id)
    {
        var lstFences = new List<FencesModel>();
        var trackers = TrackerManager.GetByUserId(Membership.GetUser().ProviderUserKey.ToString());

        var trackerFences = TrackerFenceMapManager.GetByFenceId(id);

        var tr = new FenceTrackerModel();
        tr.fenceId = id;
        tr.TrackersIn = (from x in trackers
                        where trackerFences.Any(t => t.TrackerId == x.Id)
                        select new TrackerModel { Id = x.Id, Name = x.Name }).ToList();

        tr.TrackersOut = (from x in trackers
                         where !trackerFences.Any(t => t.TrackerId == x.Id)
                         select new TrackerModel { Id = x.Id, Name = x.Name }).ToList();

        return tr;
    }

    public GeoFence GetById(int id)
    {
      return FencesManager.GetById(id);
    }

    [HttpPost]
    public int AddFence(GeoFence fence)
    {
      var f = FencesManager.GetById(fence.FencesId);
      f.FencesName = fence.FencesName;
      f.FencesCoordinate = fence.FencesCoordinate;
      
      //f.GeoFenceTypes = FencesManager.GetFencesTypeById(Convert.ToInt32(fence.TempTypeId)) ;
      f.Details = fence.Details;
      f.IsPublic = fence.IsPublic;
      f.Zoom = fence.Zoom;
      if (f.FencesId == 0)
      {
          f.CreatedBy = Membership.GetUser().ProviderUserKey.ToString();
        f.Status = true;
      }

      f.UpdatedBy = Membership.GetUser().ProviderUserKey.ToString();
      FencesManager.Save(f);
      return f.FencesId; //????
    }

    [HttpGet]
    public bool AddFenceTracker(string fenceId,string trackers)
    {
        int id= 0;
        if (!int.TryParse(fenceId, out id)) return false ;

        TrackerFenceMapManager.RemoveTrackerByFenceId(id);
        var tr = (trackers + "").Split(',');
        foreach (var item in tr)
        {
            if (String.IsNullOrWhiteSpace(item)) continue;

            var trackerFenceMap = new TrackerFenceMap()
            {
                TrackerId = item,
                FencesId = id
            };
            TrackerFenceMapManager.Add(trackerFenceMap);
        }
        return true;
    }


    [HttpDelete]
    public void DeleteFences(int id)
    {
      var item = FencesManager.GetById(id);
      item.Status = false;
      item.UpdatedBy = Membership.GetUser().ProviderUserKey.ToString();
      FencesManager.Save(item);
    }

   
  }

  public class FenceTrackerModel
  {
      public int fenceId {get;set;}
      public List<TrackerModel> TrackersIn { get; set; }
      public List<TrackerModel> TrackersOut { get; set; }
  }
  public class TrackerModel
  {
      public string Id { get; set; }
      public string Name { get; set; }
  }
  public class FencesModel
  {
    public int FencesId { get; set; }
    public string FencesName { get; set; }
    public string FencesCoordinate { get; set; }
    public bool? IsPublic { get; set; }
    public string Details { get; set; }
    public int FencesTypeId { get; set; }
    public bool? Status { get; set; }
    public int? Zoom { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<GeoFenceType> GeoFencesType { get; set; }
  }
}
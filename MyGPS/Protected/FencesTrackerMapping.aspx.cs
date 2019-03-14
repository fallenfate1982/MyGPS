using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSDataStorage;
using GTSDataStorage.Manager;
using System.Web.Security;

namespace MyGPS.Protected
{
  public partial class FencestrackerMapping : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      lblError.Visible = false;
      if (!IsPostBack)
      {
        FencesBind();        
      }
    }

    public void FencesBind()
    {
        var fences = FencesManager.GetByUserId(Membership.GetUser().ProviderUserKey.ToString());
      CollectionHelper.PopulateListControl(dlFences, "FencesId", "FencesName", fences);
    }

    public void LoadTrackers(int id)
    {
      lbTrackers.Items.Clear();
      lbNotTrackers.Items.Clear();

      int selectedFence = Convert.ToInt32(dlFences.SelectedValue);

      var ids = new List<long>();
      var trackers = TrackerManager.GetByUserId(Membership.GetUser().ProviderUserKey.ToString());

      var trackerFences = TrackerFenceMapManager.GetByFenceId(selectedFence);
      
      foreach (var item in trackerFences)
      {
        if (item.FencesId == selectedFence)
        {
          var tracker = TrackerManager.GetById(Convert.ToString(item.TrackerId));
          if(!String.IsNullOrWhiteSpace(tracker.Id))
          {
              lbTrackers.Items.Add(new ListItem(tracker.Name, tracker.Id.ToString()));
              ids.Add(Convert.ToInt64(item.TrackerId));
          }          
        }
      }

      foreach (var trackr in trackers)
      {
        if (!ids.Contains(Convert.ToInt64(trackr.Id)))
        {
          lbNotTrackers.Items.Add(new ListItem(trackr.Name, trackr.Id.ToString()));
        }
      }
    }

    protected void dlTrackers_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (dlFences.SelectedIndex == 0)
        {
            lbTrackers.Items.Clear();
            lbNotTrackers.Items.Clear();
            return;
        }
      int id = Convert.ToInt32(dlFences.SelectedValue);
      LoadTrackers(id); 
    }

    protected void DoAdd(object sender, EventArgs e)
    {
      if (lbNotTrackers.SelectedIndex < 0) return;

      var selecteItem = lbNotTrackers.SelectedItem;
      lbTrackers.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbNotTrackers.Items.Remove(selecteItem);
    }

    protected void DoRemove(object sender, EventArgs e)
    {
      if (lbTrackers.SelectedIndex < 0) return;

      var selecteItem = lbTrackers.SelectedItem;
      lbNotTrackers.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbTrackers.Items.Remove(selecteItem);
    }

    protected void DoAddAll(object sender, EventArgs e)
    {
        if (lbNotTrackers.Items.Count == 0) return;

        lbTrackers.Items.Clear();
        foreach (ListItem item in lbNotTrackers.Items)
        {
            lbTrackers.Items.Add(new ListItem(item.Text, item.Value));    
        }

        lbNotTrackers.Items.Clear();
    }

    protected void DoRemoveAll(object sender, EventArgs e)
    {
        if (lbTrackers.Items.Count == 0) return;

        lbNotTrackers.Items.Clear();
        foreach (ListItem item in lbTrackers.Items)
        {
            lbNotTrackers.Items.Add(new ListItem(item.Text, item.Value));
        }

        lbTrackers.Items.Clear();
    }

    public void RemoveTrackers(int fenceId)
    {
      TrackerFenceMapManager.RemoveTrackerByFenceId(fenceId);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        if (dlFences.SelectedIndex == 0) return;
        int fenceId = Convert.ToInt32(dlFences.SelectedValue);
        RemoveTrackers(fenceId);
        
        int fenceID = Convert.ToInt32(dlFences.SelectedValue);
        
        foreach (ListItem item in lbTrackers.Items)
        {
          var trackerId = Convert.ToInt64(item.Value);
          var trackerFenceMap = new TrackerFenceMap()
          {
            TrackerId = Convert.ToString(trackerId),
            FencesId = fenceID
          };
          TrackerFenceMapManager.Add(trackerFenceMap);
          //lblError.Text = "Record Saved Successfully";
          //lblError.ForeColor = System.Drawing.Color.Red;
          //lblError.Visible = true;
        }
      }
      catch (System.Exception ex)
      {
        lblError.Text = ex.ToString();
        lblError.Visible = true;
      }      
    }

  }
}
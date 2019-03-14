using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using GTSDataStorage;
using GTSDataStorage.Manager;

namespace MyGPS.Protected
{
  public partial class ManageGroupsAlert : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      lblError.Visible = false;
      if (!IsPostBack)
      {
        DoBind();
        DoBindFances();
      }
    }

    public void DoBind()
    {
      var groups = GroupManager.GetAll();
      CollectionHelper.PopulateListControl(dlGroups, "GroupId", "GroupName", groups);
    }
    public void DoBindFances()
    {
        var Geo_Fance = FencesManager.GetAll();
        CollectionHelper.PopulateListControl(ddGeoFences, "FencesId", "FencesName", Geo_Fance);
        
    }
    public void LoadAlerts(int id)
    {
      lbAlerts.Items.Clear();
      lbNotAlerts.Items.Clear();

      int selectedGroup = Convert.ToInt32(dlGroups.SelectedValue);

      var ids = new List<int>();
      var alerts = AlertManager.GetAll();
      var groupAlerts = GroupAlertManager.GetByGroupId(selectedGroup);
      foreach (var item in groupAlerts)
      {
        if (item.GroupId == selectedGroup)
        {
          var alert = AlertManager.GetById(item.AlertId);
          lbAlerts.Items.Add(new ListItem(alert.AlertName, alert.AlertId.ToString()));
          ids.Add(item.AlertId);
          ddGeoFences.SelectedValue = item.FencesId.ToString();
          
        }
      }

      foreach (var alert in alerts)
      {
        if (!ids.Contains(alert.AlertId) )
        {
          lbNotAlerts.Items.Add(new ListItem(alert.AlertName, alert.AlertId.ToString()));
        }
      }  
    }

    protected void DoAdd(object sender, EventArgs e)
    {
      if (lbNotAlerts.SelectedIndex < 0) return;

      var selecteItem = lbNotAlerts.SelectedItem;
      lbAlerts.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbNotAlerts.Items.Remove(selecteItem);
    }

    protected void DoRemove(object sender, EventArgs e)
    {
      if (lbAlerts.SelectedIndex < 0) return;

      var selecteItem = lbAlerts.SelectedItem;
      lbNotAlerts.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbAlerts.Items.Remove(selecteItem);
    }

    protected void dlGroups_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dlGroups.SelectedIndex == 0) return;
      int id =  Convert.ToInt32(dlGroups.SelectedValue);
      LoadAlerts(id);      
    }

    public void RemoveGroupAlerts(int id)
    {
      GroupAlertManager.RemoveGroupAlertsByGroupId(id);
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        if (dlGroups.SelectedIndex == 0) return;
        if (ddGeoFences.SelectedIndex == 0) return;
        int id = Convert.ToInt32(dlGroups.SelectedValue);
        RemoveGroupAlerts(id);
        int FanceID = Convert.ToInt32(ddGeoFences.SelectedValue);
        

        foreach (ListItem item in lbAlerts.Items)
        {
          var alertId = Convert.ToInt32(item.Value);
          var gAlert = new GroupAlert()
            {
              AlertId = alertId,
              GroupId= id,
              FencesId=FanceID
              
            };
          GroupAlertManager.Add(gAlert);
          lblError.Text = "Record Saved Successfully";
          //lblError.ForeColor = System.Drawing.Color.Red;
          lblError.Visible = true;
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
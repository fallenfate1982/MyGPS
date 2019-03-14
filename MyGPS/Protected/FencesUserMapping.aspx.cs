using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSDataStorage.Manager;
using GTSDataStorage;

namespace MyGPS.Protected
{
  public partial class FencesUserMapping : System.Web.UI.Page
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
      var fences = FencesManager.GetAll();
      CollectionHelper.PopulateListControl(dlFences, "FencesId", "FencesName", fences);
    }

    public void LoadUsers(int id)
    {
      lbUsers.Items.Clear();
      lbNotUsers.Items.Clear();

      int selectedFence = Convert.ToInt32(dlFences.SelectedValue);

      var ids = new List<string>();
      var users = Aspnet_UserManager.GetAll();

      var userFences = UserFenceMapManager.GetByFenceId(selectedFence);

      foreach (var item in userFences)
      {
        if (item.FencesId == selectedFence)
        {
          var user = Aspnet_UserManager.GetByUserName(item.UserName);
          lbUsers.Items.Add(new ListItem(user.UserName,Convert.ToString(user.UserId)));
          
          ids.Add(item.UserName);
        }
      }

      foreach (var user in users)
      {
        if (!ids.Contains(user.UserName))
        {
          lbNotUsers.Items.Add(new ListItem(user.UserName, Convert.ToString(user.UserId)));
        }
      }
    }

    protected void dlFences_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (dlFences.SelectedIndex == 0) return;
      int id = Convert.ToInt32(dlFences.SelectedValue);
      LoadUsers(id); 
    }

    protected void DoAdd(object sender, EventArgs e)
    {
      if (lbNotUsers.SelectedIndex < 0) return;

      var selecteItem = lbNotUsers.SelectedItem;
      lbUsers.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbNotUsers.Items.Remove(selecteItem);
    }

    protected void DoRemove(object sender, EventArgs e)
    {
      if (lbUsers.SelectedIndex < 0) return;

      var selecteItem = lbUsers.SelectedItem;
      lbNotUsers.Items.Add(new ListItem(selecteItem.Text, selecteItem.Value));
      lbUsers.Items.Remove(selecteItem);
    }

    public void RemoveUsers(int fenceId)
    {
      UserFenceMapManager.RemoveUserByFenceId(fenceId);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        if (dlFences.SelectedIndex == 0) return;
        int fenceId = Convert.ToInt32(dlFences.SelectedValue);
        RemoveUsers(fenceId);
        
        int fenceID = Convert.ToInt32(dlFences.SelectedValue);
        
        foreach (ListItem item in lbUsers.Items)
        {
          var userName = item.Text;
          var userFenceMap = new UserFenceMap()
          {
            UserName = userName,
            FencesId = fenceID
          };
          UserFenceMapManager.Add(userFenceMap);
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

using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSDataStorage.Manager;

namespace MyGPS.Protected
{
  public partial class ManageGroups : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        DoBind();
      }
    }

    public void DoBind()
    {
      gvGroup.DataSource = GroupManager.GetAll();
      gvGroup.DataBind();
    }

    protected void gridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
      gvGroup.PageIndex = e.NewPageIndex;
      DoBind();
    }

    protected void gvGroup_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "XEdit":
          DoEdit(e.CommandArgument);
          break;
        //case "XDelete":
        //  DoDelete(e.CommandArgument);
        //  break;
      }
    }

    //private void DoDelete(object id)
    //{
    //  try
    //  {
    //    var GroupId = ParseId(id);
    //    var GroupList = db.Groups.First(x => x.GroupId == GroupId);

    //    GroupList.Status = false;
    //    db.SaveChanges();
    //    lblSuccess.Text = "Record deleted successfully!";
    //    divSuccess.Visible = true;

    //    DoBind();
    //  }
    //  catch (Exception)
    //  {
    //  }
    //}

    protected void gvGroup_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
    {
    }

    protected void gvGroup_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
    {
      SortedColumn = e.SortExpression;
      Direction = Direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;

      DoSort(Direction, e.SortExpression);
    }

    private void DoSort(SortDirection direction, string columnName)
    {
      var list = GroupManager.GetAll();
      switch (columnName)
      {
        case "Name":
          list = direction == SortDirection.Ascending ? list.OrderBy(x => x.GroupName).ToList() : list.OrderByDescending(x => x.GroupName).ToList();
          break; 
        case "Status":
          list = direction == SortDirection.Ascending ? list.OrderBy(x => x.Status).ToList() : list.OrderByDescending(x => x.Status).ToList();
          break; 
      }
      gvGroup.DataSource = list;
      gvGroup.DataBind();
    }

    public SortDirection Direction
    {
      get
      {
        if (ViewState["dirState"] == null)
        {
          ViewState["dirState"] = System.Web.UI.WebControls.SortDirection.Ascending;
        }
        return (SortDirection)ViewState["dirState"];
      }
      set
      {
        ViewState["dirState"] = value;
      }
    }

    public string SortedColumn
    {
      get
      {
        if (ViewState["colName"] == null)
        {
          ViewState["colName"] = "Name";
        }
        return (string)ViewState["colName"];
      }
      set
      {
        ViewState["colName"] = value;
      }
    }

    protected void gvGroup_RowCreated(object sender, GridViewRowEventArgs e)
    {
      if (e.Row.RowType == DataControlRowType.Header)
      {
        foreach (TableCell tc in e.Row.Cells)
        {
          if (tc.HasControls())
          {
            var lnk = (LinkButton)tc.Controls[0];
            var img = new System.Web.UI.WebControls.Image();
            if (SortedColumn == lnk.Text)
            {
              img.ImageUrl = "~/images/" + (Direction == SortDirection.Ascending ? "asc" : "desc") + ".gif";
            }
            else
            {
              img.ImageUrl = "~/images/asc.gif";
            }

            tc.Controls.Add(new LiteralControl(" "));
            tc.Controls.Add(img);
          }
        }
      }
    }

    protected void DoAdd(object sender, EventArgs e)
    {
      DoEdit(0);
    }

    private int ParseId(object id)
    {
      int tempId;
      int.TryParse(Convert.ToString(id), out tempId);
      return tempId;
    }

    private void DoEdit(object id)
    {
      ClearForm();
      mv1.ActiveViewIndex = 1;

      var group = GroupManager.GetById(ParseId(id));
      if (group.GroupId == 0) return;

      hdId.Value = Convert.ToString(group.GroupId);
      txtGroupName.Text = group.GroupName;
      chkIsActive.Checked = group.Status ?? false;

    }

    public void ClearForm()
    {
      hdId.Value = "0";
      txtGroupName.Text = "";  
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        var group = GroupManager.GetById(ParseId(hdId.Value));

        group.GroupName = txtGroupName.Text.Trim();
        group.Status = chkIsActive.Checked;

        GroupManager.Save(group);

        mv1.ActiveViewIndex = 0;
        lblSuccess.Text = "Record saved successfully!";
        divSuccess.Visible = true;

        DoBind();
      }
      catch (Exception)
      {
        divSuccess.Visible = false;
      }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
      ClearForm();
      mv1.ActiveViewIndex = 0;
    }

    protected void btnAddMenu_Click(object sender, EventArgs e)
    {
        var tet = "klsjdf";
    }

    protected void tet_Click(object sender, EventArgs e)
    {
        var test = "sjdkfjsdf";
    }
  }
}
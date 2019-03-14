using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using GTSDataStorage.Manager;

namespace MyGPS.Protected
{
  public partial class AlertsEdit : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      divSuccess.Visible = false;
      if (!IsPostBack)
      {
        DoBind();
      }
    }

    public void DoBind()
    {
      gvAlerts.DataSource =AlertManager.GetAll() ;
      gvAlerts.DataBind();
    }

    protected void gridView_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
    {
      gvAlerts.PageIndex = e.NewPageIndex;
      DoBind();
    }

    protected void gvAlerts_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "XEdit":
          DoEdit(e.CommandArgument);
          break;
        case "XDelete":
          DoDelete(e.CommandArgument);
          break;
      }
    }

    protected void gvAlerts_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
    {
    }

    protected void gvAlerts_Sorting(object sender, System.Web.UI.WebControls.GridViewSortEventArgs e)
    {
      SortedColumn = e.SortExpression;
      Direction = Direction == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;

      DoSort(Direction, e.SortExpression);
    }

    private void DoSort(SortDirection direction, string columnName)
    {
      var list = AlertManager.GetAll();
      switch (columnName)
      {
        case "Name":
          list = direction == SortDirection.Ascending ? list.OrderBy(x => x.AlertName).ToList() : list.OrderByDescending(x => x.AlertName).ToList();
          break;
        case "Messages":
          list = direction == SortDirection.Ascending ? list.OrderBy(x => x.Message).ToList() : list.OrderByDescending(x => x.Message).ToList();
          break;
        case "Status":
          list = direction == SortDirection.Ascending ? list.OrderBy(x => x.Status).ToList() : list.OrderByDescending(x => x.Status).ToList();
          break;
      }
      gvAlerts.DataSource = list;
      gvAlerts.DataBind();
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

    protected void gvAlerts_RowCreated(object sender, GridViewRowEventArgs e)
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

      var alerts = AlertManager.GetById(ParseId(id));
      if (alerts.AlertId == 0) return;

      hdId.Value = Convert.ToString(alerts.AlertId);
      txtAlertName.Text = alerts.AlertName;
      txtMessage.Text = alerts.Message;
      //chkAlarm.Checked = Convert.ToBoolean(alerts.AlertOnAlarm); 
      chkGeofenceEnter.Checked = Convert.ToBoolean(alerts.AlertOnGeofenceEnter); 
      chkGeofenceLeave.Checked = Convert.ToBoolean(alerts.AlertOnGeofenceLeave); 
      //chkLowBattery.Checked = Convert.ToBoolean(alerts.AlertOnLowBattery);
      chkSmsAlert.Checked = Convert.ToBoolean(alerts.SmsAlert);
      chkEmailAlert.Checked = Convert.ToBoolean(alerts.EmailAlert);
      chkIsActive.Checked = alerts.Status ?? false;
      chkPushnotification.Checked = alerts.PushNotification ?? false;
    }

    private void DoDelete(object id)
    {
      try
      {
        AlertManager.DeActivate(ParseId(id));

        lblSuccess.Text = "Record deleted successfully!";
        divSuccess.Visible = true;

        DoBind(); 
      }
      catch (Exception)
      {
      }
    }

    public void ClearForm()
    {
      hdId.Value = "0";
      txtAlertName.Text = "";
      txtMessage.Text = "";
      //chkAlarm.Checked = false; 
      chkGeofenceEnter.Checked = false; 
      chkGeofenceLeave.Checked = false;
      chkPushnotification.Checked = false;
      //chkLowBattery.Checked = false;
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        var alert = AlertManager.GetById(ParseId(hdId.Value));

        alert.AlertName = txtAlertName.Text.Trim();
        alert.Message = txtMessage.Text.Trim();
        //alerts.AlertOnAlarm = chkAlarm.Checked;
        alert.AlertOnGeofenceEnter = chkGeofenceEnter.Checked;
        alert.AlertOnGeofenceLeave = chkGeofenceLeave.Checked;
        //alerts.AlertOnLowBattery = chkLowBattery.Checked;
        alert.Status = chkIsActive.Checked;
        alert.SmsAlert = chkSmsAlert.Checked;
        alert.EmailAlert = chkEmailAlert.Checked;
        alert.Status = chkIsActive.Checked;
        alert.PushNotification = chkPushnotification.Checked;

        AlertManager.Save(alert);

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
      mv1.ActiveViewIndex = 0;
      ClearForm();
    }
  }
}
using GTSDataStorage;
using GTSDataStorage.Manager;
using System;
using System.Web.Security;
using System.Web.UI.WebControls;
namespace MyGPS.Protected
{
  public partial class _ManageGeoFences : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DoBind();

            hdnLat.Value = Request.QueryString["lat"];
            hdnLng.Value = Request.QueryString["lng"];
            hdnIcon.Value = Request.QueryString["icon"];
        }
        else
        {
            hdnLat.Value = "";
            hdnLng.Value = "";
            hdnIcon.Value = "";
        }
    }

    private void DoBind()
    {
      //CollectionHelper.PopulateListControl(dlCategory, "FencesTypeId", "FencesTypeName", FencesManager.GetFencesType());

    }
  }
}
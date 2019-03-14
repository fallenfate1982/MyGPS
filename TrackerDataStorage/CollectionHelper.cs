using System.Web.UI.WebControls;

namespace GTSDataStorage
{
  public class CollectionHelper
  {
    public static void PopulateListControl(ListControl list, string value, string text, object data, bool isSelectOption = true)
    {
      list.Items.Clear();
      list.DataSource = data;
      list.DataValueField = value;
      list.DataTextField = text;
      list.DataBind();
      if (data == null)
        list.Enabled = false;
      else
        list.Enabled = true;

      //DropDownList is a special case, which needs an empty first item
      if (isSelectOption)
      {
        if (list.GetType().ToString().IndexOf("DropDownList") > -1)
          list.Items.Insert(0, new ListItem("--- select ---", "0")); //TODO: ML
      }
    }
  }
}

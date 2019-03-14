<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageGroupsAlert.aspx.cs" Inherits="MyGPS.Protected.ManageGroupsAlert" %>

<%@ Register Src="../UserControls/ucLeftNavAlerts.ascx" TagName="ucLeftNavAlerts" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="padding-sm text-left">
        <uc1:ucLeftNavAlerts ID="ucLeftNavAlerts1" runat="server" />
        <h4 class="headline">Manage Group Alerts</h4>
        <div class="seperator"></div>        
         <table class="table" style="width:70%">
                    <tr>
                        <td><b>Groups</b></td>
                        <td colspan="2"><asp:DropDownList runat="server" ID="dlGroups" AutoPostBack="True" style="width:70%"
                            CssClass="form-control" OnSelectedIndexChanged="dlGroups_SelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Geo Fences</b></td>
                        <td colspan="2"><asp:DropDownList ID="ddGeoFences" CssClass="form-control" runat="server" style="width:70%"/></td>
                    </tr>
                    <tr >
                        <th>Alerts In Group</th>
                        <th></th>
                        <th>Not In Groups</th>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox runat="server" ID="lbAlerts" CssClass="select-box pull-left form-control width-100"/>
                        </td>
                        <td align="center">&nbsp;&nbsp;&nbsp;&nbsp;<asp:ImageButton runat="server" ImageUrl="~/images/arrow-left.png"
                            OnClick="DoAdd" Width="45px" />
                            <br />
                            <asp:ImageButton runat="server" ImageUrl="~/images/arrow-right.png"
                                OnClick="DoRemove" Width="45px" />
                        </td>
                        <td>
                            <asp:ListBox ID="lbNotAlerts"  runat="server" CssClass="select-box pull-left form-control width-100" /></td>
                    </tr>
                    <tr>
                        <td >
                            <asp:Button runat="server" Text="Save" ID="Button1" CssClass="btn btn-success" OnClientClick="selectValue();" OnClick="btnSave_Click" />
                        </td>
                        <td colspan="2">
                            <asp:Label runat="server" ID="lblError" CssClass="alert alert-success"/>
                        </td>
                    </tr>
                </table>
    </div>
    <script>
        function selectValue() {
            var ids = $("#dlGroups").val();
            if (ids == 0) {
                alert("Kindly select a Group");
                return false;
            }
        }
    </script>
</asp:Content>

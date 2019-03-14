<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FencesUserMapping.aspx.cs" Inherits="MyGPS.Protected.FencesUserMapping" %>

<%@ Register Src="../UserControls/ucLeftNavAlerts.ascx" TagName="ucLeftNavAlerts" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="padding-sm">
        <h4 class="headline">Map Fence and User</h4>
        <div class="seperator"></div>
        <table class="table" style="width: 70%">
            <tr>
                <td>Fences<b>&nbsp;</b></td>
                <td>
                    <asp:DropDownList runat="server" ID="dlFences" AutoPostBack="True" 
                        CssClass="form-control" OnSelectedIndexChanged="dlFences_SelectedIndexChanged" /></td>
                <td>&nbsp;</td>
                <asp:Label runat="server" ID="lblError" />
            </tr>
            <tr>
                <th>Users In Fences</th>
                <th></th>
                <th>Not In Fences</th>
            </tr>
            <tr>
                <td>
                    <asp:ListBox runat="server" ID="lbUsers" CssClass="select-box pull-left form-control width-100"  /></td>
                <td align="center">&nbsp;&nbsp;&nbsp;&nbsp;<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/arrow-left.png"
                    OnClick="DoAdd" Width="45px" />
                    <br />
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/arrow-right.png"
                        OnClick="DoRemove" Width="45px" />
                </td>
                <td>
                    <asp:ListBox ID="lbNotUsers" runat="server" CssClass="select-box pull-left form-control width-100"  /></td>
            </tr>
            <tr>
                <td colspan="3">
                    <asp:Button runat="server" Text="Save" ID="Button1" CssClass="btn btn-success" OnClientClick="selectValue();" OnClick="btnSave_Click" Width="90px" />
                </td>
            </tr>
        </table>
    </div>

    <script>
        function selectValue() {
            var ids = $("#dlTrackers").val();
            if (ids == 0) {
                alert("Kindly select a Tracker");
                return false;
            }
        }
    </script>
</asp:Content>

<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageAlerts.aspx.cs" Inherits="MyGPS.Protected.ManageAlerts" %>

<%@ Register src="../UserControls/ucLeftNavAlerts.ascx" tagname="ucLeftNavAlerts" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <uc1:ucLeftNavAlerts ID="ucLeftNavAlerts1" runat="server" />
</asp:Content>

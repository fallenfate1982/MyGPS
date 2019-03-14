<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AlertsEdit.aspx.cs" Inherits="MyGPS.Protected.AlertsEdit" %>

<%@ Register Src="../UserControls/ucLeftNavAlerts.ascx" TagName="ucLeftNavAlerts" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:ucLeftNavAlerts ID="ucLeftNavAlerts1" runat="server" />
    <div class="padding-sm">
        <h4 class="headline">Manage Alerts</h4>
        <asp:MultiView runat="server" ID="mv1" ActiveViewIndex="0">
            <asp:View runat="server" ID="list">
                <asp:Button runat="server" ID="btnAddMenu" CssClass="btn btn-success" OnClick="DoAdd" Text="Add Alert" />
                <div class="seperator"></div>
                <div class="alert alert-success" id="divSuccess" runat="server" visible="false">
                    <p>
                        <asp:Label runat="server" ID="lblSuccess" />
                    </p>
                </div>

                <asp:GridView ID="gvAlerts" runat="server" AutoGenerateColumns="False" GridLines="None"
                    AllowPaging="True" CssClass="table table-hover table-striped" PagerStyle-CssClass="gridview_pager"
                    AlternatingRowStyle-CssClass="gridview_alter" PageSize="15"
                    OnPageIndexChanging="gridView_PageIndexChanging" OnRowCommand="gvAlerts_RowCommand" OnRowDeleting="gvAlerts_RowDeleting"
                    AllowSorting="True" OnSorting="gvAlerts_Sorting" OnRowCreated="gvAlerts_RowCreated">
                    <Columns>
                        <asp:BoundField DataField="AlertName" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Message" HeaderText="Message" SortExpression="Message" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status"
                            HeaderStyle-Width="8%" ItemStyle-Width="8%" />
                        <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ImageUrl="~/Images/edit.png"
                                    CommandArgument='<%# Eval("AlertId") %>' CommandName="XEdit" />
                                &nbsp;&nbsp;
                                    <asp:ImageButton runat="server" ImageUrl="~/Images/del.png"
                                        CommandArgument='<%# Eval("AlertId") %>' CommandName="XDelete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="gridview_pager"></PagerStyle>
                </asp:GridView>

            </asp:View>
            <asp:View runat="server" ID="edit">
                <asp:HiddenField runat="server" ID="hdId" Value="0" />
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="form-horizontal">
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox runat="server" ID="chkIsActive" />
                                        <span class="custom-checkbox"></span>
                                        Active
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <div class="form-group">
                                <label for="inputEmail1" class="col-lg-2 control-label">Name</label>
                                <div class="col-lg-10">
                                    <asp:TextBox runat="server" ID="txtAlertName" class="form-control input-sm" />
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <label for="inputPassword1" class="col-lg-2 control-label">Details</label>
                                <div class="col-lg-10">
                                    <asp:TextBox runat="server" ID="txtMessage" TextMode="MultiLine" class="form-control input-sm"
                                        Height="100px" />
                                </div>
                                <!-- /.col -->
                            </div>                            
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox ID="chkGeofenceEnter" runat="server" />
                                        <span class="custom-checkbox"></span>
                                        Alert on Geofence Enter
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox ID="chkGeofenceLeave" runat="server" />
                                        <span class="custom-checkbox"></span>
                                        Alert on Geofence Leave
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox ID="chkSmsAlert" runat="server" />
                                        <span class="custom-checkbox"></span>
                                        Sms Alert
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox ID="chkEmailAlert" runat="server" />
                                        <span class="custom-checkbox"></span>
                                        Email Alarm
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <label class="label-checkbox">
                                        <asp:CheckBox runat="server" ID="chkPushnotification" />
                                        <span class="custom-checkbox"></span>
                                        Push  notification
                                    </label>
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                    <asp:Button runat="server" ID="btnSave" Text=" Save " CssClass="btn btn-success" OnClick="btnSave_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                </div>
                                <!-- /.col -->
                            </div>
                            <!-- /form-group -->
                        </div>
                    </div>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>

</asp:Content>

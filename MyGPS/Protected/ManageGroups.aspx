<%@ Page Title="Manage Group" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="ManageGroups.aspx.cs" Inherits="MyGPS.Protected.ManageGroups"  %>

<%@ Register Src="../UserControls/ucLeftNavAlerts.ascx" TagName="ucLeftNavAlerts" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:ucLeftNavAlerts ID="ucLeftNavAlerts1" runat="server" />
    <div class="padding-sm">
        <h3 class="headline">Manage Groups</h3>        
        <asp:MultiView runat="server" ID="mv1" ActiveViewIndex="0">
            <asp:View runat="server" ID="list">
                <asp:Button CssClass="btn btn-success" runat="server" ID="btnAddMenu"  Text="Add Group" OnClick="DoAdd" />
               <div class="seperator"></div>
                <div class="alert alert-success" id="divSuccess" runat="server" visible="false">
                    <p>
                        <asp:Label runat="server" ID="lblSuccess" />
                    </p>
                </div>

                <asp:GridView ID="gvGroup" runat="server" AutoGenerateColumns="False" GridLines="None"
                    AllowPaging="True" CssClass="table table-hover table-striped" PagerStyle-CssClass="gridview_pager" 
                    AlternatingRowStyle-CssClass="gridview_alter" PageSize="15"
                    OnPageIndexChanging="gridView_PageIndexChanging" OnRowCommand="gvGroup_RowCommand" OnRowDeleting="gvGroup_RowDeleting"
                    AllowSorting="True" OnSorting="gvGroup_Sorting" OnRowCreated="gvGroup_RowCreated">                    
                    <Columns>
                        <asp:BoundField DataField="GroupName" HeaderText="Name" SortExpression="Name" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ImageUrl="~/Images/edit.png"
                                    CommandArgument='<%# Eval("GroupId") %>' CommandName="XEdit" />
                                &nbsp;&nbsp;
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
                            <div class="form-group">
                                <label for="inputEmail1" class="col-lg-2 control-label">Group Name</label>
                                <div class="col-lg-6">
                                    <asp:TextBox runat="server" ID="txtGroupName" class="form-control input-sm" />
                                </div>
                                <!-- /.col -->
                            </div>
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
                            <!-- /form-group -->
                            <div class="form-group">
                                <div class="col-lg-offset-2 col-lg-10">
                                       <asp:Button runat="server" ID="btnSave" Text=" Save " CssClass="btn btn-success" OnClick="btnSave_Click" />
                                    &nbsp;&nbsp;
                                    <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                                 </div>
                            </div>
                        </div>
                    </div>
                </div>                                 
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
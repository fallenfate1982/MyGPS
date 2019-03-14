<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FencesTrackerMapping.aspx.cs" Inherits="MyGPS.Protected.FencestrackerMapping" %>

<%@ Register Src="../UserControls/ucLeftNavAlerts.ascx" TagName="ucLeftNavAlerts" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="padding-sm">
        <h4 class="headline">Geo Fence/Tracker Mapper</h4>
        <div class="seperator"></div>
        <div class="panel panel-default">
            <div class="panel-heading" style="padding-bottom:30px;">
			    <div class="form-group">
			        <label class="col-lg-2 control-label">Select Geo Fence</label>
			        <div class="col-lg-10">
				        <asp:DropDownList runat="server" ID="dlFences" AutoPostBack="True" 
                                CssClass="form-control" OnSelectedIndexChanged="dlTrackers_SelectedIndexChanged" />
			        </div><!-- /.col -->
		        </div>
		    </div>
			<div class="panel-body relative">
                <div class="col-lg-5">
                    <div class="form-group">
			            <label class="control-label block">Trackers linked to selected Geo Fence</label>
                        <asp:ListBox runat="server" ID="lbTrackers" CssClass="tracker-select-box  pull-left form-control" />
		            </div>
                </div>
                <div class="col-lg-2">
                    <div class="row">
                        <div class="select-box-option tracker-select-box-nav">
					        <a class="btn btn-sm btn-default" id="btnSelect" runat="server" onserverclick="DoAdd">
						        &nbsp;<i class="fa fa-angle-left"></i>&nbsp;
					        </a>
					        <a class="btn btn-sm btn-default" id="btnRemove" runat="server" onserverclick="DoRemove">
						        &nbsp;<i class="fa fa-angle-right"></i>&nbsp;
					        </a>
                            <br /><br />
                            <a class="btn btn-sm btn-default" id="btnSelectAll" runat="server" onserverclick="DoAddAll">
						        <i class="fa fa-angle-left"></i>
                                <i class="fa fa-angle-left"></i>
					        </a>
					        <a class="btn btn-sm btn-default" id="btnRemoveAll" runat="server" onserverclick="DoRemoveAll">
						        <i class="fa fa-angle-right"></i>
                                <i class="fa fa-angle-right"></i>
					        </a>
				        </div>
                    </div>
                </div> 	
                <div class="col-lg-5">
                    <div class="form-group">
			            <label class="control-label block">Trackers NOT linked to selected Geo Fence</label>
                        <asp:ListBox ID="lbNotTrackers" runat="server" CssClass="tracker-select-box  pull-right form-control" />
		            </div>
                </div>	
			</div>
            <div class="panel-heading">
			    <div class="form-group">
			        <asp:Label CssClass="alert alert-success" runat="server" ID="lblError" />
                    <asp:Button runat="server" Text="Save" ID="Button1" CssClass="btn btn-success" OnClientClick="selectValue();" OnClick="btnSave_Click" />
		        </div>	
		    </div>
            
		</div>
    </div>
    <script>
        function selectValue() {
            var ids = $("#dlTrackers").val();
            if (ids == 0) {
                alert("Please select a Tracker.");
                return false;
            }
        }
    </script>
</asp:Content>

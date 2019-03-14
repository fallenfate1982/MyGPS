<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="MyGPS.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-container">
        <div class="padding-md">
				<div class="row">
					<div class="col-sm-6 col-md-3">
						<div class="panel-stat3 bg-danger">
							<h2 class="m-top-none" id="txtEventCount" runat="server"></h2>
							<h5>Total Vehicle Events</h5>
							<i class="fa fa-clock-o fa-lg"></i><span class="m-left-xs">Today</span>
							<div class="stat-icon">
								<i class="fa fa-bell fa-3x"></i>
							</div>
						</div>
					</div><!-- /.col -->
					<div class="col-sm-6 col-md-3">
						<div class="panel-stat3 bg-primary">
							<h2 class="m-top-none"><span id="txtAttentionEventCount">0</span></h2>
							<h5>Vehicles require Attention</h5>
							<i class="fa fa-clock-o fa-lg"></i><span class="m-left-xs">Today</span>
							<div class="stat-icon">
								<i class="fa fa-exclamation-triangle fa-3x"></i>
							</div>
						</div>
					</div><!-- /.col -->
					<div class="col-sm-6 col-md-3">
						<div class="panel-stat3 bg-warning">
							<h2 class="m-top-none" id="txtIdleEventCount" runat="server"></h2>
							<h5>Vehicles with Engine Idle Events</h5>
							<i class="fa fa-clock-o fa-lg"></i><span class="m-left-xs">Today</span>
							<div class="stat-icon">
								<i class="fa fa-refresh fa-3x"></i>
							</div>
						</div>
					</div><!-- /.col -->
					<div class="col-sm-6 col-md-3">
						<div class="panel-stat3 bg-success">
							<h2 class="m-top-none" id="txtSpeedEventCount" runat="server"></h2>
							<h5>Vehicles with Speeding Events</h5>
							<i class="fa fa-clock-o fa-lg"></i><span class="m-left-xs">Today</span>
							<div class="stat-icon">
								<i class="fa fa-cab fa-3x"></i>
							</div>
						</div>
					</div><!-- /.col -->
				</div>
        
                <div class="panel panel-default">
							<div class="panel-heading">
								
                                Vehicles&nbsp;&nbsp;&nbsp;

								<span class="">	
									<a class="btn btn-info" href="MyTrack.aspx">Map View</a>
								</span>
							</div>
                            <asp:ListView ID="trackerList" runat="server">  
                                <LayoutTemplate>  
                                    <table class="table table-striped">  
                                        <thead>
									        <tr>
										        <th>Name</th>
										        <th>Status</th>
                                                <th>Mileage</th>
										        <th></th>
									        </tr>
								        </thead> 
                                        <tbody> 
                                            <tr id="ItemPlaceholder" runat="server">  
                                            </tr>  
                                        </tbody>
                                    </table>  
                                </LayoutTemplate>  
                                <ItemTemplate>  
                                    <tr>
                                        <td>
                                            <asp:Label   
                                                ID="lbName"  
                                                runat="server"  
                                                Text='<%# Eval("TrackerName")%>'  
                                                >  
                                            </asp:Label>  
                                        </td>
                                        <td>
                                            <asp:Label   
                                                ID="lbSpeed"  
                                                runat="server"  
                                                 CssClass='<%# Eval("TrackerSpeedClass")%>'
                                                Text='<%# Eval("TrackerSpeed")%>'  
                                                >  
                                            </asp:Label>
                                            &nbsp;&nbsp;&nbsp;<div class="btn btn-danger" id="A1" Visible='<%# Eval("NeedsAttention")%>' runat="server"><i class="fa fa-exclamation-triangle fa-lg"></i><span class="m-left-xs">Needs Attention</div>
                                        </td> 
                                        <td>
                                            <asp:Label   
                                                ID="lbMileage"  
                                                runat="server"  
                                                Text='<%# Eval("TrackerMileage")%>'  
                                                >  
                                            </asp:Label>
                                            &nbsp;&nbsp;&nbsp;<div class="btn btn-danger" id="Div1" Visible='<%# Eval("NeedsAttention")%>' runat="server"><i class="fa fa-exclamation-triangle fa-lg"></i><span class="m-left-xs">Needs Attention</div>
                                        </td> 
                                        <td>
                                            <a class="btn btn-success" id="btnViewDetails" href='<%# Eval("DetailsNavigationURL")%>'   runat="server">View Details</a>
                                            <a class="btn btn-success" id="btnViewHistory" href='<%# Eval("HistoryNavigationURL")%>'   runat="server">View History</a>
                                        </td>
                                    </tr>                  
                                </ItemTemplate>  
                            </asp:ListView>
						</div><!-- /panel -->
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="popUpCont" runat="server">
</asp:Content>

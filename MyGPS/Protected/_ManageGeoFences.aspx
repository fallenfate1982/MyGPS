<%@ Page Title="Manage Geo Fences" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="_ManageGeoFences.aspx.cs" Inherits="MyGPS.Protected._ManageGeoFences" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-container">
        <div id="map" class="row padding-sm">
            <div class="col-md-8">
                <div class="row">
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <input type="text" id="searchArea" style="width: 82%;" class="form-control inline-block" />
                    <button id="btnClearMap" class="btn btn-danger inline-block">Clear Map</button>
                 </div>
                    <div id="map_canvas" class="gmap3" style="height: 600px; width:100%"></div>        
             </div>
            <div class="col-md-4">
                <h4 class="headline">Fence Information</h4>
                <span class="line"></span>
                <div class="padding-sm">
                    <input type="hidden" id="hdId" value="0"/>
                        <label>Name</label>
                        <br/>
                        <input type="text" id="txtName" class="form-control"/>
                        <br/>
                        <div class="clear-fix" style="display:none">
                        <label>IsPublic</label>
                        <input type="checkbox" id="chkIsPublic" style="float:left"/>
                        </div>
                        <br/>
                        <label>Details</label>
                        <br/>
                        <textarea id="txtDetails" class="form-control"></textarea>

                    <%-- Removed panel box to select trackers TODO - we should remove /edit this functionality so its only there when needed --%>

                        <%--<div class="panel-body">
                            <div class="col-lg-5">
                                <div class="form-group">
			                        <label class="control-label block">Trackers linked </label>
                                    <br />
                                    <asp:ListBox runat="server" ID="lbTrackers" ClientIDMode="Static" CssClass="tracker-select-box  pull-left form-control" />
		                        </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="row">
                                    <div class="select-box-option tracker-select-box-nav">
                                        <br /><br /><br />
					                    <a class="btn btn-sm btn-default" id="btnSelect" >
						                    &nbsp;<i class="fa fa-angle-left"></i>&nbsp;
					                    </a>
					                    <a class="btn btn-sm btn-default" id="btnRemove" >
						                    &nbsp;<i class="fa fa-angle-right"></i>&nbsp;
					                    </a>
                                        <br /><br />
                                        <a class="btn btn-sm btn-default" id="btnSelectAll" >
						                    <i class="fa fa-angle-left"></i>
                                            <i class="fa fa-angle-left"></i>
					                    </a>
					                    <a class="btn btn-sm btn-default" id="btnRemoveAll" >
						                    <i class="fa fa-angle-right"></i>
                                            <i class="fa fa-angle-right"></i>
					                    </a>
				                    </div>
                                </div>
                            </div> 	
                            <div class="col-lg-5">
                                <div class="form-group">
			                        <label class="control-label block">Trackers NOT linked</label>
                                    <asp:ListBox ID="lbNotTrackers" ClientIDMode="Static" runat="server" CssClass="tracker-select-box  pull-right form-control" />
		                        </div>
                            </div>	
			            </div>  --%>
                        <div class="panel-footer">
                            <button id="btnSave" class="btn btn-success" >Save</button>
                            &nbsp;
                            <button id="btnCancel" class="btn btn-danger" >Cancel</button>              
                        </div>
                    </div>                        
                </div>
            </div>
        </div>
    <div id="fenceListing" class="row padding-sm">
            <div class="col-md-8 paddingLR-md">
						<div class="panel panel-default">
							<div class="panel-heading">
								<h3 class="no-margin">Geo Fences</h3>
                                <div class="value">
			    			        <span><a id="btnNew" class="btn btn-info">New GeoFence</a></span>
			    		        </div>
							</div>
							<table class="table table-hover table-striped">
								<thead>
									<tr>
										<th>Name</th>
                                        <th>Details</th>
										<th></th>
									</tr>
								</thead>
								<tbody data-bind="foreach: $root.fencesData" id="fencesList">
									<tr>
										<td><span id="#" data-bind="text: $data.FencesName"></span></td>
                                        <td><span id="#" data-bind="text: $data.Details"></span></td>
										<td><a data-bind="click: $root.selectedFences" class="btn btn-success">View/Edit</a>&nbsp;<a data-bind="    click: $root.deleteFences" class="btn btn-danger">Delete</a></td>
									</tr>
								</tbody>
							</table>
						</div><!-- /panel -->
					</div>
        <input type="hidden" runat="server" id ="hdnLat" />
        <input type="hidden" runat ="server" id="hdnLng" />
         <input type="hidden" runat ="server" id="hdnIcon" />

        </div>
     <script src="https://maps.googleapis.com/maps/api/js?v=3.exp&libraries=places"></script>
    <script src="/js/knockout-3.0.0rc.js"></script>
    <script src="/js/map.mygps.js"></script>
    <script src="/js/fencesVM.js"></script>
    <script src="/js/reversegeo.js"></script>
    <script type="text/javascript">
        
        //Load fences data
        var vm = new fences();
        $(document).ready(function () {
            getFences();
            
        });
      

        function setLocation() {
            if ($("#MainContent_hdnLat").val() != "" && $("#MainContent_hdnLng").val() != "") {
                
                $("#btnNew").click();

                var pos = new google.maps.LatLng($("#MainContent_hdnLat").val(), $("#MainContent_hdnLng").val());
                

                var marker = new google.maps.Marker({
                    position: pos,
                    map: map,
                    icon: $("#MainContent_hdnIcon").val(),
                    title: 'Location'
                });

                map.setZoom(19);
                map.setCenter(pos);

                $("#MainContent_hdnLat").val("");
                $("#MainContent_hdnLng").val("");
            }
        }

        function getFences() {
            $("#btnCancel").hide();
            $("#btnSave").hide();
            $("#map").hide();
            loadFences(vm);
            ko.applyBindings(vm, $("#fencesList")[0]);
          
        }

        //Initialize Map with initial settings
        $(function () {
            initializeMap();
            setLocation();
        });
        

        //Do clear map
        $("#btnClearMap").click(function () {
            setInitialLocation();
            initializeMap(map.getZoom());
            clearForm();
            $("#searchArea").val('');
            return false;
        });
        
        //Do Cancel
        $("#btnCancel").click(function () {
            $("#fenceListing").show();
            $("#map").hide();
            clearForm();
            setInitialLocation();
            initializeMap();
            $("#searchArea").val('');
            $("#btnCancel").hide();
            $("#btnSave").show();
            return false;
        });
        
        // Do New
        $("#btnNew").click(function () {            
            $("#fenceListing").hide();
            $("#map").show();
            clearForm();
            setInitialLocation();
            initializeMap();

            $("#searchArea").val('');
            $("#btnCancel").show();
            $("#btnSave").show();
            
            
            //getTrackers();

            return false;
        });

        //Do Save
        $("#btnSave").click(function () {
            var zoom = map.getZoom();
            try {
                var pPath = poly.getPath();
                var ar = pPath.getArray();
                var polyData = "";
                if (ar.length < 3) {
                    alert("Please specify a validate geo fence.");
                    return false;
                }

                for (var i = 0; i < ar.length; i++) {                    
                    var lat = ar[i].lat();// ar[i].ob;
                    var lg = ar[i].lng(); //ar[i].pb;
                    //polyData = polyData + "{lat:" + lat + ", lg:" + lg + "}";
                    polyData = polyData + "{lat:" + lat + ", lg:" + lg + "}";
                    if (i + 1 != ar.length)
                        polyData = polyData + ",";
                }
               
                var source = {
                    'FencesId': $("#hdId").val(),
                    'TempTypeId': $("#dlCategory").val(),
                    'FencesName': $('#txtName').val(),
                    'FencesCoordinate': polyData,
                    'IsPublic': $("#chkIsPublic").prop("checked"),
                    'Details': $('#txtDetails').val(),
                    'Zoom': zoom
                };

                //alert(polyData);

                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/api/fences/AddFence",
                    data: source,
                    success: function(data) {
                        loadFences(vm);
                        
                        setInitialLocation();
                        initializeMap();
                        $("#searchArea").val('');
                        $("#btnCancel").hide();
                        $("#btnSave").hide();
                        alert("Geo fence saved successfully.");
                        $("#fenceListing").show();
                        $("#map").hide();

                        //Map trackers
                        saveTrackers(data);
                    },
                    error: function(error) {
                        jsonValue = jQuery.parseJSON(error.responseText);
                        alert(jsonValue);
                    }
                });
                
            } catch(e) {
                alert(JSON.stringify(e));
            }

            return false;
        });

        function saveTrackers(id) {
            var trackersId = "";
            $("#lbTrackers > option").each(function () {
                var key = $(this).data('key');
                trackersId = trackersId + key + ",";
            });

            var source = {
                'fenceId': id,
                'trackers': trackersId
            };

            $.ajax({
                type: "GET",
                dataType: "json",
                url: "/api/fences/AddFenceTracker",
                data: source,
                success: function (data) {
                },
                error: function (error) {                    
                    alert(JSON.stringify(error));
                }
            });

            clearForm();
        }

        function clearForm() {
            $("#txtName").val("");
            $("#txtDetails").val("");
            $("#lbTrackers").html("");
            $("#lbNotTrackers").html("");
            $("#hdId").val("0");
        }

    </script>
</asp:Content>

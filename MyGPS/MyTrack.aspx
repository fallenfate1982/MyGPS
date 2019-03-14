<%@ Page  EnableEventValidation="false" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyTrack.aspx.cs" Inherits="MyGPS.MyTrack" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false&v=3.15"></script>

<%--	<script type="text/javascript" src="Scripts/jquery-ui-1.8.21.custom.min.js"></script>--%>
    <script type="text/javascript" src="Scripts/gmap3.min.js"></script>
    <script type="text/javascript" src="Scripts/markerwithlabel.js"></script>
    <script src="js/bootstrap-datepicker.min.js" type="text/javascript"></script>
    <script src="/js/reversegeo.js"></script>
    <link rel="stylesheet" href="css/datepicker.css" />
    
    <script type="text/javascript">
        
        var map;//google map 
        var googleMapWidth;//width of map frame ... reference needed for full screen mode
        var googleMapHeight; //height of map frame ... reference needed for full screen mode

        var arrMarkers; //array of all markers displayed on the map
        var pausedActiveTrack = false; //flag indicating whether user has paused automatic updates or not
        var activeTrackFocus;
        var defaultTrackerId;
        var defaultTrackerIndex;

        $(document).ready(function () {
            var today = new Date();
            $("#histDate").val(today.getMonth() +1 + "/" + today.getDate() + "/" + today.getFullYear());

            map = new google.maps.Map(document.getElementById('myTrackerMap'), {
                zoom: 9,
                center: new google.maps.LatLng(10.5526, -61.3152),
                mapTypeId: google.maps.MapTypeId.HYBRID,
                streetViewControl: false
            });
          
            googleMapWidth = $("#myTrackerMap").css('width');
            googleMapHeight = $("#myTrackerMap").css('height');

            $('#enter-full-screen').click(function () {
                $("#myTrackerMap").css("position", 'fixed').
			  css('top', 0).
			  css('left', 0).
			  css("width", '100%').
			  css("height", '100%');
                google.maps.event.trigger(map, 'resize');
                map.setCenter(new google.maps.LatLng(10.5526, -61.3152));
                this.style.display = "none";
                document.getElementById('exit-full-screen').style.display = "block";
                $('#exit-full-screen').css('margin-left', $('#header').width() - 261);
                return false;
            });
            $('#exit-full-screen').click(function () {
                $("#myTrackerMap").css("position", 'relative').
			  css('top', 0).
			  css("width", googleMapWidth).
			  css("height", googleMapHeight);
                google.maps.event.trigger(map, 'resize');
                map.setCenter(new google.maps.LatLng(10.5526, -61.3152));
                this.style.display = "none";
                document.getElementById('enter-full-screen').style.display = "block";
                return false;
            });
            
            //get default TrackerId if there is one
            defaultTrackerId = $('#txtDefaultTrackerId').val();
            defaultTrackerIndex = -1;

            //retrieve and display positions of all vehicles for current user
            arrMarkers = new Array();
            
            $("#breadcrumblist").append("<li><i class='fa fa-home'></i><a href='javascript:panToMarker(-1, 0);'> Map View</a></li>");
            //alert("Starting Retrival");
            $.ajax({
                type: "POST",
                url: "MyTrack.aspx/GetMarkers",
                data: "{}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (json) {
                    //alert("Success");
                    var data = json.d;
                    var json_parsed = $.parseJSON(data);
                    var labelMarker;
                    var index;
                    for (index = 0; index < json_parsed.Markers.length; index++) {
                        //create marker for current trackers and add to map
                        var markerInfo = json_parsed.Markers[index];
                        var pos = new google.maps.LatLng(markerInfo.LatitudeDecimal, markerInfo.LongitudeDecimal);
                        var markerIcon = determineMarkerIcon(markerInfo.Direction, markerInfo.Speed);
                        labelMarker = new MarkerWithLabel({
                            position: pos,
                            labelAnchor: new google.maps.Point(-18, 44),
                            labelClass: "labels",
                            labelStyle: { opacity: 0.75 },
                            labelContent: markerInfo.TrackerDetail.Name,
                            icon: markerIcon,
                            map: map
                        });
                        arrMarkers[index] = labelMarker;                       
                       // labelMarker.setAnimation(google.maps.Animation.DROP);
                        //setTimeout(function () { labelMarker.setAnimation(null); }, 750);

                        //add tracker to listing
                        var messageCenterAnchor = $('<a/>', {
                            href: '#',
                        });
                        var newListItem = $('<li/>', {
                            onclick: 'panToMarker(' + index + ',' + '"' + markerInfo.TrackerDetail.Id + '"' + ')'
                        });    // NOTE: you have to put quotes around "id" for IE..

                        var span = $('<span/>', {
                            html: '<i class="fa fa-map-marker fa-lg"></i> - ' + markerInfo.TrackerDetail.Name,
                            class: 'submenu-label'
                        });
                        
                        if (defaultTrackerId == markerInfo.TrackerDetail.Id) {
                            defaultTrackerIndex = index;
                        }

                        $("#breadcrumblist").append("<li class='active' id='map" + index + "' style='display:none;'>Map: " + markerInfo.TrackerDetail.Name + "</li>");

                        //span.append($('<i class="fa fa-map-marker fa-lg"></i>'));
                        messageCenterAnchor.append(span);
                        newListItem.append(messageCenterAnchor);

                        $("#vehicleListing").append(newListItem);

                        var vehicleDetails = $('<div/>', {
                            html: "",
                            "id": "vehicleDetails" + index,
                            "class":"list-group"
                        });


                        var vehicleNumber = $('<a />',{
                            html: '<span>Vehicle Number: ' + markerInfo.TrackerDetail.Description + '</span>',
                            "class": "list-group-item"
                        });

                        var vehicleTracker = $('<a />', {
                            html: '<span>Tracker Name: ' + markerInfo.TrackerDetail.Name + '</span>',
                            "class": "list-group-item"
                        });

                        var vehicleLatitude = $('<a/>', {
                            html: '<b>Lattitude:</b>&nbsp;<span id="latitude"' + index + '>' + markerInfo.LatitudeDecimal + '</span>',
                            "class": "list-group-item"
                        });
                        var vehicleLongitude = $('<a />', {
                            html: "<b>Longitude:</b>&nbsp;<span id='longitude" + index + "'>" + markerInfo.LongitudeDecimal + "</span>",
                            "class": "list-group-item"
                        });
                        var vehicleAddress = $('<a />', {
                            html: "<b>Address: N/A</b>&nbsp;<span id='address" + index +"'></span>",
                            "class": "list-group-item"
                        });

                        var vehicleMileage = $('<a />', {
                            html: "<i class='fa fa-rocket fa-lg grey'></i><b class='m-left-xs'>Mileage:</b>&nbsp;<span class='badge badge-info' id='mileage" + index + "'></span>",
                            "class": "list-group-item"
                        });


                        var vehicleSpeed = $('<a/>', {
                            html: "<i class='fa fa-dashboard fa-lg grey'></i><b class='m-left-xs'>Speed:</b>&nbsp;<span id='speed" + index + "'></span>",
                            "id": "",
                            "class": "list-group-item"
                        });
                       

                        var vehicleLastUpdate = $('<a />', {
                            html: "<i class='fa fa-clock-o fa-lg grey'></i><b class='m-left-xs'>Reached Location:</b>&nbsp;<span id='lastUpdate" + index + "'>" + markerInfo.FormattedClientRecordedDateTime + " </span>",
                            "class": "list-group-item"
                        });
                       

                        var vehicleReportTime = $('<a />', {
                            html: "<i class='fa fa-file-text fa-lg grey'></i><b class='m-left-xs'>Last Report:</b>&nbsp;<span id='rpt" + index + "'></span>",
                            "class": "list-group-item"
                        });
                       

                        var vehicleIdleTime = $('<a />', {
                            html: "<i class='fa fa-car fa-lg grey'></i><b class='m-left-xs'>Stop Time:</b>&nbsp;<span id='idle" + index + "'></span>",
                            "class": "list-group-item"
                        });
                       

                        var vehicleEngineState = $('<a />', {
                            html: "<i class='fa fa-flash fa-lg grey'></i><b class='m-left-xs'>Engine On:</b>&nbsp;<span id='engineStatus" + index + "'></span>",
                            "class": "list-group-item"
                        });

                        var input4 = $('<a />', {
                            html: "<i class='fa fa-flash fa-lg grey'></i><b class='m-left-xs'>Input 4:</b>&nbsp;<span id='input4" + index + "'></span>",
                            "class": "list-group-item"
                        });
                        var input3 = $('<a />', {
                            html: "<i class='fa fa-flash fa-lg grey'></i><b class='m-left-xs'>Input 3:</b>&nbsp;<span id='input3" + index + "'></span>",
                            "class": "list-group-item"
                        });

                        var vehicleLocation = $('<a />', {
                            html: "<i class='fa fa-map-marker fa-lg grey'></i><b class='m-left-xs'>Address:</b>&nbsp;<span class='badge badge-info' id='currentLocation" + index + "'></span><br /><br /><b class='m-left-xs'><a class='btn btn-success' onclick='getAddress(" + index + ");'>View Current Address</a></b>",
                            "class": "list-group-item"
                        });

                        var vehicleRecordMaintenance = $('<a />', {
                            html: "<b class='m-left-xs'><a href='#maintenanceModal' role='button' data-toggle='modal' class='btn btn-info btn-small'>Record Maintenance</a></b>",
                            "class": "list-group-item"
                        });

                        
                       

                        var vehicleDoorStatus = $('<a />', {
                            html: "<i class='fa fa-dashboard fa-lg grey'></i><b class='m-left-xs'>Doors:</b>&nbsp;<span id='doors" + index + "'>" + "Closed" + " </span>",
                            "class": "list-group-item"
                        });
                        var vehicleAlarm = $('<a />', {
                            html: "<i class='fa fa-dashboard fa-lg grey'></i><b class='m-left-xs'>Alarm:</b>&nbsp;<span id='alarm" + index + "'>" + "Off" + " </span>",
                            "class": "list-group-item"
                        });
                        var vehicleFuel = $('<a/>', {
                            html: "<i class='fa fa-dashboard fa-lg grey'></i><b class='m-left-xs'>Fuel:</b>&nbsp;<span id='fuel" + index + "'>" + "Good" + " </span>",
                            "class": "list-group-item"
                        });

                        // Header for Tracker Details
                        var header = $("<div/>",{
                            "class":"panel-body"
                        });

                        var headerAnchor = $("<a/>",{
                            "class":"pull-left avatar",
                            href:"#"
                        });

                        var headerImg = $("<img/>",{
                            "class":"img-circle",
                            alt:"Tracker Avatar",
                            src: markerInfo.TrackerDetail.TrackerPicture
                        });

                        var innerDiv = $("<div/>",{
                            "class":"pull-left",
                            "style":"margin-left:15px;"
                        });

                        var trackerNamer = $("<strong/>",{
                            "class":"font-16",
                            html: markerInfo.TrackerDetail.Name +"<br/>"
                        });

                        var vehicleNumberr = $("<span/>",{
                            "class":"grey",
                            html: markerInfo.TrackerDetail.Description
                        });

                        var histButtonIcon = $(" <a />", {
                            html:" History",
                            "class": "fa fa-history fa-lg grey btn btn-sm historyInfo_open",
                            href:"#historyInfo",
                            "data-popup-ordinal":"2",
                            id:"open_23102706"
                        });

                        var fullScrButtonIcon = $(" <a />", {
                            "class": "fa fa-arrows-alt fa-lg grey",
                            html: " Full Screen"
                        });

                        var geoButtonIcon = $(" <a />", {
                            html: " Add GeoFence",
                            "class": "fa fa-history fa-lg grey btn btn-sm",
                            href: "/protected/_managegeofences.aspx?lat=" + markerInfo.LatitudeDecimal + "&lng=" + markerInfo.LongitudeDecimal + "&icon=" + labelMarker.icon,
                            target: "_blank"
                        });

                        var lbBr = $("<br/>");
                        
                        headerAnchor.append(headerImg);
                        header.append(headerAnchor);
                        innerDiv.append(trackerNamer);
                        innerDiv.append(vehicleNumberr);
                        innerDiv.append(lbBr);
                        innerDiv.append(histButtonIcon);
                        innerDiv.append(geoButtonIcon);
                        //innerDiv.append(fullScrButtonIcon);
                        header.append(innerDiv);

                        vehicleDetails.append(header);

                        //vehicleDetails.append(vehicleNumber);
                        //vehicleDetails.append(vehicleTracker);
                        //vehicleDetails.append(vehicleLatitude);
                        //vehicleDetails.append(vehicleLongitude);
                        //vehicleDetails.append(vehicleAddress);
                        vehicleDetails.append(vehicleMileage);
                        vehicleDetails.append(vehicleSpeed);
                        vehicleDetails.append(vehicleLastUpdate);
                        vehicleDetails.append(vehicleIdleTime);
                        vehicleDetails.append(vehicleReportTime);

                        vehicleDetails.append(vehicleEngineState);
                        vehicleDetails.append(input4);
                        vehicleDetails.append(input3);
                        vehicleDetails.append(vehicleLocation);
                        vehicleDetails.append(vehicleRecordMaintenance);

                        //vehicleDetails.append(vehicleDoorStatus);
                       // vehicleDetails.append(vehicleAlarm);
                        //vehicleDetails.append(vehicleFuel);

                        $("#vehicleDetails").append(vehicleDetails);
                        vehicleDetails.hide();


                        setMileage($('#mileage' + index), Math.floor(markerInfo.TrackerDetail.Mileage));
                        ($('#speed' + index), Math.floor(markerInfo.Speed));
                        setReachedLocation($('#lastUpdate' + index), markerInfo.FormattedClientRecordedDateTime);
                        setLastReport($('#rpt' + index), markerInfo.TrackerDetail.RptHrs, markerInfo.TrackerDetail.RptMin, markerInfo.TrackerDetail.RptSec);
                        setIdleTime($('#idle' + index), markerInfo.IdleHrs, markerInfo.IdleMin, markerInfo.IdleSec);
                        setEngineStat($('#engineStatus' + index), markerInfo.DInput5);

                        setInput($('#input4' + index), markerInfo.DInput4);
                        setInput($('#input3' + index), markerInfo.DInput3);

                        google.maps.event.addListener(labelMarker, 'click', make_callback(vehicleDetails, markerInfo.TrackerDetail.Id));
                    }
                    if (defaultTrackerIndex != -1) {
                        panToMarker(defaultTrackerIndex, defaultTrackerId);
                    }
                    $('#txtDefaultTrackerId').hide();
                    setInterval("retrieveLatestTrackerInfo()", 5000);
                }
            });
            
        });

        // Callback function is needed to ensure the correct value is passed and not the last value
        function make_callback(id,did) {
            return function () {
                clickBounce(this);
                $("div#vehicleDetails .list-group").hide();
                id.show();
                $("#currTrackerId").val(did);
            };
        }

        function clickBounce(t) {

            t.setAnimation(google.maps.Animation.BOUNCE);
            setTimeout(function () { t.setAnimation(null); }, 1400);
        }

        function retrieveLatestTrackerInfo() {
            if (pausedActiveTrack == false) {
                $.ajax({
                    type: "POST",
                    url: "MyTrack.aspx/GetMarkers",
                    data: "{}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (json) {
                        var data = json.d;
                        var json_parsed = $.parseJSON(data);
                        for (var index = 0; index < json_parsed.Markers.length; index++) {
                            //create marker for current trackers and add to map
                            var markerInfo = json_parsed.Markers[index];
                            var pos = new google.maps.LatLng(markerInfo.LatitudeDecimal, markerInfo.LongitudeDecimal);
                            var markerIcon = determineMarkerIcon(markerInfo.Direction, markerInfo.Speed);
                            arrMarkers[index].setPosition(pos);
                            arrMarkers[index].setIcon(markerIcon);
                            //$('latitude' + index).html(markerInfo.LatitudeDecimal);
                            //$('longitude' + index).html(markerInfo.LongitudeDecimal);
                            
                            setMileage($('#mileage' + index), Math.floor(markerInfo.TrackerDetail.Mileage));
                            setSpeedBadge($('#speed' + index), Math.floor(markerInfo.Speed));
                            setReachedLocation($('#lastUpdate' + index),markerInfo.FormattedClientRecordedDateTime);
                            setIdleTime($('#idle' + index), markerInfo.IdleDays, markerInfo.IdleHrs, markerInfo.IdleMin, markerInfo.IdleSec);
                            setLastReport($('#rpt' + index), markerInfo.TrackerDetail.RptDays, markerInfo.TrackerDetail.RptHrs, markerInfo.TrackerDetail.RptMin, markerInfo.TrackerDetail.RptSec);
                            setEngineStat($('#engineStatus' + index), markerInfo.DInput5);

                            setInput($('#input4' + index), markerInfo.DInput4);
                            setInput($('#input3' + index), markerInfo.DInput3);
                        }
                    }
                });
            }
        }

        function getAddress(markerIndex) {
            $("#currentLocation" + markerIndex).html("");
            $("#currentTime" + markerIndex).html("");
            $.ajax({
                type: "GET",
                dataType: "json",
                url: getReverseURL(arrMarkers[markerIndex].getPosition().lat(), arrMarkers[markerIndex].getPosition().lng()),
                data: '',
                success: function (response) {
                    var address = processResponse(response);
                    var date = new Date();
                    var n = date.toDateString();
                    var time = date.toLocaleTimeString();
                    $("#currentLocation" + markerIndex).html(address);
                    //+ " at " + (n + ' ' + time)
                },
                error: function (error) {
                    $("#currentLocation" + markerIndex).html("Address cannot be retrieved.");
                }
            });
        }

        var currentLocationElement = '';
        function panToMarker(markerIndex,Id) {
            clearInterval(activeTrackFocus);
            $("#currTrackerId").val(Id);
            $("div#vehicleDetails .list-group").hide();
            $('#vehicleDetails' + markerIndex).css("display", "block");
            
            if (markerIndex == -1)
            {
                $("ul#breadcrumblist .active").hide();
                map.setZoom(9);
                map.setCenter(new google.maps.LatLng(10.5526, -61.3152));
            }
            else{
                map.setZoom(15);
                activeTrackFocus = setInterval("panToMarkerRec(" + markerIndex + ")", 1000);
            }
        }
        
        function panToMarkerRec(markerIndex) {
           
            $("div#vehicleDetails .list-group").hide();
            $("ul#breadcrumblist .active").hide();
            $('#vehicleDetails' + markerIndex).css("display", "block");
            currentLocationElement = $('#vehicleLocation' + markerIndex);
            $('#map' + markerIndex).show();
            map.panTo(arrMarkers[markerIndex].getPosition());
            
        }

        function resetZoom() {
            map.setZoom(9);
        }

        function determineMarkerIcon(direction, speed) {           
            if (direction >= 0 && direction <= 22 && speed != 0)
                return "/images/marker_north.png";
            if (direction > 22 && direction <= 67 && speed != 0)
                return "/images/marker_north_east.png";
            if (direction > 67 && direction <= 112 && speed != 0)
                return "/images/marker_east.png";
            if (direction > 112 && direction <= 157 && speed != 0)
                return "/images/marker_south_east.png";
            if (direction > 157 && direction <= 202 && speed != 0)
                return "/images/marker_south.png";
            if (direction > 202 && direction <= 247 && speed != 0)
                return "/images/marker_south_west.png";
            if (direction > 247 && direction <= 292 && speed != 0)
                return "/images/marker_west.png";
            if (direction > 292 && direction <= 337 && speed != 0)
                return "/images/marker_north_west.png";
            if (direction > 337 && direction <= 360 && speed != 0)
                return "/images/marker_north.png";
            if (direction > 360 || speed == 0)
                return "/images/marker_stop.png";
        }

        function toggleActiveTrack() {
            if (pausedActiveTrack == true) {
                pausedActiveTrack = false;
                $('#activeTrack').html("+ Pause Active-Track");
            }
            else {
                pausedActiveTrack = true;
                $('#activeTrack').html("+ UnPause Active-Track");
            }
        }

     
            
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:TextBox runat="server" ID="txtDefaultTrackerId" CssClass="default_mytrack_control" ClientIDMode="Static" Visible="true"></asp:TextBox>
        <div class="padding-md">
            <div class="row">
                <div class="col-lg-8">
                     <div id="myTrackerMap" class="gmap3" ></div>
		                     
                
		          <%--  <div  class="box-t">
                        <div class ="fullScreen" title="Enter Full Screen" id="enter-full-screen" >Enter Full-Screen</div>
                        <br />&nbsp;&nbsp;&nbsp;<a id="activeTrack" onclick="toggleActiveTrack()" href="#">+ Pause Active-Track</a>
                        <br />&nbsp;&nbsp;&nbsp;<a id="a2" onclick="panToMarker(-1)" href="#">+ Reset Active-Track</a>
		            </div>--%>
	                    
                </div>
                <div class="col-lg-4">
                    <div id="vehicleDetails" class="panel panel-default bg-success">
	                </div>
                </div>
            </div>
        </div>

</asp:Content>

<asp:Content ID="popups" ContentPlaceHolderID="popUpCont" runat="server">

    <div class="custom-popup width-100" id="historyInfo">
        
          <div class="row">
              <div class="col-md-10">
                  <h4 class="headline text-left">
							1 - Select your Date:
							<span class="line"></span>
						</h4>
                 <div class="input-append date" id="dp3" data-date-format="mm-dd-yyyy">
                     <%-- TODO - Need to default this to today! --%>
                      <input id="histDate"  class="span2" size="16" type="text">
                     <%-- <span class="add-on"><i class="fa fa-th"></i></span>--%>
                    </div>
                        <script type="text/javascript">
                            $(".span2").datepicker({format:"mm/dd/yyyy"});
                        </script>

                  </div>
          </div>
        <div class ="row">
            <div class="col-md-10">
                <h4 class="headline text-left">
							2 - Select your Desired Time Slice:
							<span class="line"></span>
						</h4>
                 
                 <a href="#" onclick="getHistory(1);" class="btn btn-info quick-btn"><i class="fa fa-sun-o"></i><span>12 AM - 6 AM</span></a>
                 <a href="#" onclick="getHistory(2);" class="btn btn-success quick-btn"><i class="fa fa-clock-o"></i><span>6 AM - 12 PM</span></a>
                 <a href="#" onclick="getHistory(3);" class="btn btn-success quick-btn"><i class="fa fa-clock-o"></i><span>12 PM - 6 PM</span></a>
                 <a href="#" onclick="getHistory(4);" class="btn btn-danger quick-btn"><i class="fa fa-moon-o"></i><span>6 PM - 12 AM</span></a>
                
                <input type="hidden" id="currTrackerId"/>
            </div>
            <div class="col-md-2">
                <a class="btn btn-danger quick-btn historyInfo_close"><i class="fa fa-minus-square"></i><span>Cancel</span></a>
            </div>
        </div>
            <script type="text/javascript">
                function getHistory(timeSlice) {
                    var historyDate = $("#histDate").val();
                    var thecurrTrackerId = $("#currTrackerId").val();

                    window.location.href = "/History.aspx?timeSlice=" + timeSlice + "&histDate=" + historyDate + "&currTrackerId=" + thecurrTrackerId;

                }
            </script>
    </div>

    <div class="modal fade" id="maintenanceModal">
  			<div class="modal-dialog">
    			<div class="modal-content">
      				<div class="modal-header">
        				<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
						<h4>Maintenance Details</h4>
      				</div>
				    <div class="modal-body">
						<form>
							<div class="row">
								<div class="col-md-6">
                                    <div class="form-group">
								        <label>Mileage</label>
								        <input id="maintenaceMileage" type="text" class="form-control input-sm">
							        </div><!-- /form-group -->
                                </div>
                            </div>
							<div class="row">
								<div class="col-md-6">
									<div class="form-group">
										<label>Notes</label>
                                        <textarea id="maintenanceNote" class="form-control" placeholder="Enter maintenance notes here..." rows="4"></textarea>
									</div>
								</div><!-- /.col -->
							</div><!-- /.row -->
						</form>
				    </div>
				    <div class="modal-footer">
				        <button class="btn btn-success btn-sm" data-dismiss="modal" aria-hidden="true">Close</button>
						<a href="#" onclick="saveMaintenance();" class="btn btn-danger btn-sm">Save</a>
                        <div id="maintenanceError">
						</div>
				    </div>
                    <script type="text/javascript">
                        function saveMaintenance() {
                            var tracker = $("#currTrackerId").val();
                            var maintMileage = $("#maintenaceMileage").val();
                            var mainNote = $("#maintenanceNote").val();

                            // make a call to save the Maintenance
                            $.ajax({
                                url: '/api/Maintenance/Add', //calling Web API controller
                                cache: false,
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                data: { trackerId: tracker, mileageCount: maintMileage, maintenanceNote: mainNote },
                                dataType: "json",
                                success: function (data) {
                                    if (data)
                                    {
                                        $("#maintenanceError").removeClass();
                                        $('#maintenanceModal').modal('hide');
                                        return true;
                                    }
                                    else
                                    {
                                        $("#maintenanceError").html("<strong>Oh snap!</strong> Change a few things up and try submitting again.");
                                        $("#maintenanceError").removeClass();
                                        $("#maintenanceError").addClass("alert");
                                        $("#maintenanceError").addClass("alert-danger")
                                        return false;
                                    }
                                }
                            }).fail(function (xhr, textStatus, err) {
                                console.log("Error ( request.js=> saveLineItem() : Data saving failed..." + err);
                                $("#maintenanceError").html("<strong>Oh snap!</strong> Change a few things up and try submitting again.");
                                return false;
                            });
                            
                        }

            </script>
			  	</div><!-- /.modal-content -->
			</div><!-- /.modal-dialog -->
		</div>

</asp:Content> 

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="BreadcrumbContentPlaceHolder">
        <div id="breadcrumb">
                <ul class="breadcrumb" id="breadcrumblist">
                    
                </ul>
            </div><!-- /breadcrumb-->
</asp:Content>
<asp:Content ID="trackers" runat="server" ContentPlaceHolderID="trackers">

          <li class="openable open">
                            <a id="mapView" href="#">
                                <span class="menu-icon">
                                    <i class="fa fa-globe fa-lg"></i>
                                </span>
                                <span class="text">
                                    Trackers <i class="fa fa-chevron-down fa-sm"></i>
                                   <span class="menu-icon">
                                    
                                </span>
                                </span>
                                <span class="menu-hover"></span>
                            </a>
              <ul id="vehicleListing" class="submenu">
            </ul>
        </li>
        
</asp:Content>
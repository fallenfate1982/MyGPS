<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="MyGPS.History" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false&v=3.15"></script>
	
    <script type="text/javascript" src="Scripts/gmap3.min.js"></script>
    <script type="text/javascript" src="Scripts/markerwithlabel.js"></script>
  <script type="text/javascript" src="Scripts/markerclusterer.js"></script>
    <script src="Scripts/glDatePicker.js" type="text/javascript"></script>

    <script src="js/bootstrap-datepicker.min.js" type="text/javascript"></script>

    <link rel="stylesheet" href="css/datepicker.css" />

    <script type="text/javascript">
        var map;//google map 
        var googleMapWidth;//width of map frame ... reference needed for full screen mode
        var googleMapHeight; //height of map frame ... reference needed for full screen mode

        var arrMarkers; //array of all markers displayed on the map
        var clusterOptions = { "maxZoom":18, "zoomOnClick": true };
        var markerCluster;
        var useCluster = true;
        var json_parsed;
        var startMarker;
        var endMarker;
        var currMarkerIndex;

        $(document).ready(function () {

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

            arrMarkers = [];
            trackerSelected($("#MainContent_trackerId").val());
            $("#breadcrumblist").append("<li><i class='fa fa-home'></i><a href='MyTrack.aspx'> Map View</a></li>");
        });

        function trackerSelected(trackerId) {
            clearMarkers();
            $.ajax({
                type: "POST",
                url: "History.aspx/GetTrackerHistory",
                data: "{'trackerId':'" + trackerId + "','selectedDate':'" + $("#MainContent_datepicker").val() + "','timeInterval':'" + $("#MainContent_time1").val() + "'}",
                contentType: "application/json",
                dataType: "json",
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log(xhr.status);
                    console.log(xhr.responseText);
                    console.log(thrownError);
                },
                success: function (json) {
                    var data = json.d;
                     json_parsed = $.parseJSON(data);
                    // note there is only one 
                    var infowindow = new google.maps.InfoWindow();
                    for (var index = 0; index < json_parsed.History.Markers.length; index++) {
                        //create marker for current trackers and add to map
                        var markerInfo = json_parsed.History.Markers[index];
                        var pos = new google.maps.LatLng(markerInfo.Lat, markerInfo.Lng);
                        var markerIcon = determineMarkerIcon(markerInfo.Dir, markerInfo.Speed);
                        var labelMarker = new MarkerWithLabel({
                            position: pos,
                            labelAnchor: new google.maps.Point(-18, 44),
                            labelClass: "labels",
                            labelStyle: { opacity: 0.75 },
                            //labelContent: "Seq#:" + (index+1) + "<br/>"+"Speed:"+ (Math.floor(markerInfo.Speed)),
                            icon: markerIcon,
                            map: map,
                            speed:markerInfo.Speed
                            //title: 
                        });
                        arrMarkers[index] = labelMarker;
                        labelMarker.setAnimation(google.maps.Animation.DROP);
                        //setTimeout(function () { labelMarker.setAnimation(null); }, 750);
                        var contentWindowData = "Seq#:" + (index + 1) + "<br/>" + "Speed:" + (Math.floor(markerInfo.Speed));

                            //"<b>" + json_parsed.Markers[index].MarkerDate + "</b>" + "<br/>" +
                            //"Idle Time: " + json_parsed.Markers[index].IdleHrs +":" +json_parsed.Markers[index].IdleMin + ":"+json_parsed.Markers[index].IdleSec + "<br/>" +
                            //"Engine On: " + json_parsed.Markers[index].DInput5 + "<br/>" + "Doors: Closed" + "<br/>" + "Alarm: Off " + "<br/>" +
                            //"Input 1: " + json_parsed.Markers[index].Input1 + "<br/>" +
                            //"Input 2: " + json_parsed.Markers[index].Input2;


                        //$("#vehicleDetails").empty(); // Removed this as it was clearing all the stuff and not working properly.
                        makeInfoWindowEvent(map, infowindow, contentWindowData, labelMarker,index);
                        addTrackerToInfoList(markerInfo, json_parsed.History.TrackerDetail, index, labelMarker);

                        if (index == 0) {
                            $("#breadcrumblist").append("<li class='active' id='map" + index + "'>History: " + json_parsed.History.TrackerDetail.Name + "</li>");
                            startMarker = labelMarker;
                        }

                        if (index == json_parsed.History.Markers.length - 1) {
                            endMarker = labelMarker;
                            panToMarker(arrMarkers[index]);
                        }
                    }
                    
                    if (useCluster)
                        markerCluster = new MarkerClusterer(map, arrMarkers, clusterOptions);
                }
            });            
        }

        function toggleClustering() {
            if (useCluster) {
                setClusterOff();
            }
            else
            {
                setClusterOn();
            }
        }
        function setClusterOff() {
            if(useCluster)
            {
                // remove clustering
                markerCluster = null;
                map.unbindAll();
                map.setZoom(map.getZoom() + 1);
                map.setZoom(map.getZoom() - 1);

                // add back markers
                var i;
                for (i = 0; i < arrMarkers.length; ++i) {
                    arrMarkers[i].setMap(map);
                }

                useCluster = false;
            }
        }

        // Not the best way to do this but alternatives seemed not to bind to map and clusters dissappeared after zoom changed so creating a new map here.
        function setClusterOn() {
            if (!useCluster) {
                var zoomlevelNow = map.getZoom();
                var center = map.getCenter();

                map = new google.maps.Map(document.getElementById('myTrackerMap'), {
                    zoom: zoomlevelNow,
                    center: center,
                    mapTypeId: google.maps.MapTypeId.HYBRID,
                    streetViewControl: false
                });

                markerCluster = new MarkerClusterer(map, arrMarkers, clusterOptions);
                useCluster = true;
            }
        }


        // Callback function is needed to ensure the correct value is passed and not the last value
        function make_callback(id, did,index) {
            return function () {
                clickBounce(this);
                currMarkerIndex = index;
                $("div#vehicleDetails .list-group").hide();
                id.show();
                $("#currTrackerId").val(did);
            };
        }

        function makeInfoWindowEvent(map, infowindow, contentString, index) {
            google.maps.event.addListener(index, 'click', function () {
                currMarkerIndex = index;
                infowindow.setContent(contentString);
                infowindow.open(map, index);

            });
        }

          

        function clearMarkers() {
            if (markerCluster == null) {
                
            }
            else {
                markerCluster.clearMarkers();
            }
            //clear all current markers on the screen
            for (var index = 0; index < this.arrMarkers.length; index++) {
                this.arrMarkers[index].setMap(null);
            }
            this.arrMarkers = [];
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


        function addTrackerToInfoList(markerInfo, TrackerDetail, index,labelMarker) {


            //add tracker to listing
            var messageCenterAnchor = $('<a/>', {
                href: '#',
            });
            var newListItem = $('<li/>', {
                onclick: 'panToMarker(' + index + ',' + TrackerDetail.Id + ')'
            });    // NOTE: you have to put quotes around "id" for IE..

            var span = $('<span/>', {
                html: '<i class="fa fa-map-marker fa-lg"></i> - ' + TrackerDetail.Name,
                class: 'submenu-label'
            });

            //span.append($('<i class="fa fa-map-marker fa-lg"></i>'));
            messageCenterAnchor.append(span);
            newListItem.append(messageCenterAnchor);

            $("#vehicleListing").append(newListItem);

            var vehicleDetails = $('<div/>', {
                html: "",
                "id": "vehicleDetails" + index,
                "class": "list-group"
            });


            var vehicleNumber = $('<a />', {
                html: '<span>Vehicle Number: ' + TrackerDetail.Description + '</span>',
                "class": "list-group-item"
            });

            var vehicleTracker = $('<a />', {
                html: '<span>Tracker Name: ' + TrackerDetail.Name + '</span>',
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
                html: "<b>Address: N/A</b>&nbsp;<span id='address" + index + "'></span>",
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
            var header = $("<div/>", {
                "class": "panel-body"
            });

            var headerAnchor = $("<a/>", {
                "class": "pull-left avatar",
                href: "#"
            });

            var headerImg = $("<img/>", {
                "class": "img-circle",
                alt: "User Avatar",
                src: TrackerDetail.TrackerPicture
            });

            $("#trackerImage").attr("src", TrackerDetail.TrackerPicture);

            var innerDiv = $("<div/>", {
                "class": "pull-left",
                "style": "margin-left:15px;"
            });

            var trackerNamer = $("<strong/>", {
                "class": "font-16",
                html: TrackerDetail.Name + "<br/>"
            });

            var vehicleNumberr = $("<span/>", {
                "class": "grey",
                html: TrackerDetail.Description
            });

            var histButtonIcon = $(" <a />", {
                html: " Live Map",
                "class": "fa fa-history fa-lg grey btn btn-sm",
                href: "/mytrack.aspx"
            });

            var geoButtonIcon = $(" <a />", {
                html: " Add GeoFence",
                "class": "fa fa-history fa-lg grey btn btn-sm",
                href: "/protected/_managegeofences.aspx?lat=" + markerInfo.Lat + "&lng=" + markerInfo.Lng + "&icon=" + labelMarker.icon,
                target: "_blank"
            });

            var fullScrButtonIcon = $(" <a />", {
                "class": "fa fa-arrows-alt fa-lg grey",
                html: " Full Screen"
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
            vehicleDetails.append(vehicleSpeed);
            vehicleDetails.append(vehicleLastUpdate);
            vehicleDetails.append(vehicleIdleTime);
            vehicleDetails.append(vehicleReportTime);

            vehicleDetails.append(vehicleEngineState);
            //vehicleDetails.append(vehicleDoorStatus);
            // vehicleDetails.append(vehicleAlarm);
            //vehicleDetails.append(vehicleFuel);

            $("#vehicleDetails").append(vehicleDetails);
            vehicleDetails.hide();

            setSpeedBadge($('#speed' + index), Math.floor(markerInfo.Speed));
            setReachedLocation($('#lastUpdate' + index), markerInfo.FormattedClientRecordedDateTime);
            setLastReport($('#rpt' + index), TrackerDetail.RptDays, TrackerDetail.RptHrs, TrackerDetail.RptMin, TrackerDetail.RptSec);
            setIdleTime($('#idle' + index), markerInfo.IdleDays, markerInfo.IdleHrs, markerInfo.IdleMin, markerInfo.IdleSec);
            setEngineStat($('#engineStatus' + index), markerInfo.DInput5);

            google.maps.event.addListener(labelMarker, 'click', make_callback(vehicleDetails, TrackerDetail.Id,index));
        }

        function panToMarker(marker) {
            //map.setZoom(15);
            map.setCenter(marker.getPosition());
            google.maps.event.trigger(marker, 'click');

        }

        function clickBounce(t) {

            t.setAnimation(google.maps.Animation.BOUNCE);
            setTimeout(function () { t.setAnimation(null); }, 1400);
        }

        function nextMarker() {
            panToMarker(arrMarkers[currMarkerIndex+1]);
        }

        function prevMarker() {
            panToMarker(arrMarkers[currMarkerIndex-1]);
        }

        function filterStop() {
            if (useCluster) {
                $(".onoffswitch-label").trigger("click");
            }

            var i;
            for (i = 0; i < arrMarkers.length; i++) {
                if (arrMarkers[i].speed == 0) {
                    arrMarkers[i].setMap(map);
                }
                else {
                    arrMarkers[i].setMap(null);
                }
            }
        }

        function filterSpeeding() {
            if (useCluster) {
                $(".onoffswitch-label").trigger("click");
            }
            var i;
            for (i = 0; i < arrMarkers.length; i++) {
                if (arrMarkers[i].speed > 80) {
                    arrMarkers[i].setMap(map);
                }
                else {
                    arrMarkers[i].setMap(null);
                }
            }
        }

        function filterOff() {
            var i;
            for (i = 0; i < arrMarkers.length; i++) {
                
                 arrMarkers[i].setMap(map);
               
            }
        }
      
    </script>
</asp:Content>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="BreadcrumbContentPlaceHolder">
        <div id="breadcrumb">
                <ul class="breadcrumb" id="breadcrumblist">
                    
                </ul>
            </div><!-- /breadcrumb-->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
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
                    

                    <div id="historyControls" class ="panel panel-default bg-success">
                        <div class="panel-body"><a class="pull-left avatar" href="#"><img id="trackerImage" class="img-circle" alt="User Avatar" src="img/capl.jpg"></a>
                            <div class="pull-left" style="margin-left:15px;">
                                <strong class="font-16">History Controls<br></strong>
                                <br><a class="fa fa-history fa-lg grey btn btn-sm historyInfo_open" href="#historyInfo">Set Time Slice</a></div></div>


                        <div id="" class="list-group-item"><i class="fa fa-group fa-lg grey"></i>
                            <b class="m-left-xs">Clustering:</b>

                                                    <div class="onoffswitch">
                                            <input type="checkbox" name="onoffswitch" class="onoffswitch-checkbox" id="myonoffswitch" checked>
                                            <label onclick="toggleClustering();" class="onoffswitch-label" for="myonoffswitch">
                                                <span class="onoffswitch-inner"></span>
                                                <span class="onoffswitch-switch"></span>
                                            </label>
                                        </div>
                                      
                        </div>

                        <div id="" class="list-group-item"><i class="fa fa-map-marker fa-lg grey"></i>
                           
                            <b class="m-left-xs">Move to Marker:
                            </b>
                            <span class="list-item-nl">
                            <a class="btn btn-danger quick-btn" href="#" onclick="panToMarker(startMarker);">
                                <i class="fa fa-fast-backward"></i><span>Start</span></a>
                            <a class="btn btn-info quick-btn" href="#" onclick="prevMarker();">
                                <i class="fa fa-backward"></i><span>Prev</span></a>
                            <a class="btn btn-info quick-btn" href="#" onclick="nextMarker();">
                                <i class="fa fa-forward"></i><span>Next</span></a>
                            <a class="btn btn-danger quick-btn" href="#" onclick="panToMarker(endMarker);">
                                <i class="fa fa-fast-forward"></i><span>End</span></a>
                             </span>   

                        </div>

                        <div id="" class="list-group-item"><i class="fa fa-filter fa-lg grey"></i>
                            <b class="m-left-xs">Only show particular types of Markers:
                            </b>

                            <span class="list-item-nl">
                            <a class="btn btn-info quick-btn" href="#" onclick="filterStop();"><i class="fa fa-stop"></i><span>Stopped</span></a>
                            <a class="btn btn-danger quick-btn" href="#" onclick="filterSpeeding();"><i class="fa fa-exclamation-circle"></i><span>Speeding</span></a>
                                 <a class="btn btn-warning quick-btn" href="#" onclick="filterOff();"><i class="fa fa-power-off"></i><span>Off</span></a>
                                </span>
                        </div>

                    </div>

                   <div id="vehicleDetails" class="panel panel-default bg-success">
	                </div>

                        </div>

                </div>
            </div>

<input runat="server" id="time1" style="display:none;" type="text" />


<input runat="server" id="datepicker" style="display:none;" type="text"/>
<input runat="server" id="trackerId" style="display:none;" type="text" />
  

</asp:Content>


<asp:Content ID="popups" ContentPlaceHolderID="popUpCont" runat="server">

    <div class="custom-popup width-100" id="historyInfo">
        
          <div class="row">
              <div class="col-md-10">
                  <h4 class="headline text-left">
							1 - Select your Date:
							<span class="line"></span>
						</h4>
                 <div class="input-append date" id="dp3"data-date-format="mm-dd-yyyy">
                     <%-- TODO - Need to default this to today! --%>
                      <input id="histDate"  class="span2" size="16" type="text">
                     <%-- <span class="add-on"><i class="fa fa-th"></i></span>--%>
                    </div>
                        <script type="text/javascript">
                            $(".span2").datepicker({ format: "mm/dd/yyyy" });
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

                    $("#MainContent_time1").val(timeSlice);
                    $("#MainContent_datepicker").val(historyDate);

                    $("#historyInfo").popup('hide')
                    trackerSelected($("#MainContent_trackerId").val());


                }
            </script>
    </div>

</asp:Content> 
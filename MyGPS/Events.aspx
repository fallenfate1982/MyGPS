<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Events.aspx.cs" Inherits="MyGPS.Events" EnableEventValidation="false" %>


<%-- TOD eventt validation turned off as it was not accepting datetime picker information. We need to get a compatible datetime picker --%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="js/bootstrap-datepicker.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false&v=3.15"></script>

    <script type="text/javascript" src="Scripts/gmap3.min.js"></script>
   <script type="text/javascript" src="Scripts/markerwithlabel.js"></script>
  <script type="text/javascript" src="Scripts/markerclusterer.js"></script>

    <script src="scripts/jquery.easing.1.3.min.js"></script>
    <script src="scripts/ej.web.all.min.js"></script>

        <link href="themes/bootstrap.min.css" rel="stylesheet" />
    <link href="themes/default-theme/ej.widgets.all.min.css" rel="stylesheet" />
    <link href="themes/default.css" rel="stylesheet" />
    <link href="themes/default-responsive.css" rel="stylesheet" />

     <style type="text/css">
         #ReportViewer1 {
             width: 100%;
             height: 100%;             
         }

         html, body {
             overflow: hidden; padding:0; margin: 0;height:100%;position:static;
         }
     </style>

    <link rel="stylesheet" href="css/datepicker.css" />

    <!-- Chosen -->
    <link href="css/chosen/chosen.min.css" rel="stylesheet" />

    <script type="text/javascript">
        // Mapping functions for Modal
        var _event;
        var tub;
        var map;
        var currMarkerIndex;

        $(document).ready(function () {

            map = new google.maps.Map(document.getElementById('myTrackerMap'), {
                zoom: 9,
                center: new google.maps.LatLng(10.5526, -61.3152),
                mapTypeId: google.maps.MapTypeId.HYBRID,
                streetViewControl: false
            });

            googleMapWidth = 100;//$("#myTrackerMap").css('width');
            googleMapHeight = 100; //$("#myTrackerMap").css('height');

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
            // trackerSelected($("#MainContent_trackerId").val());
            // $("#breadcrumblist").append("<li><i class='fa fa-home'></i><a href='MyTrack.aspx'> Dashboard</a></li>");
        });


        function loadEvent(eventId) {
            clearMarkers();
            $.ajax({
                type: "POST",
                url: "Events.aspx/GetEvent",
                data: "{'eventId':'" + eventId + "'}",
                contentType: "application/json",
                dataType: "json",
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log(xhr.status);
                    console.log(xhr.responseText);
                    console.log(thrownError);
                },
                success: function (json) {
                    tub = json;
                    var data = json.d;
                    json_parsed = $.parseJSON(data);
                    // note there is only one 
                    var infowindow = new google.maps.InfoWindow();
                    _event = json_parsed;

                    $("#vehicleDetails").empty();

                    //If its a speeding event we have multiple markers to add

                    if (_event.Event.$type == "GTSBizObjects.Events.TrackerEvents.Speeding, GTSBizObjects") {
                        // Set up stuff for speeding
                        var i;
                        for (i = 0; i < _event.Event.Messages.$values.length; i++) {
                            var markerInfo = json_parsed.Event.Messages.$values[i];
                            var pos = new google.maps.LatLng(markerInfo.LatitudeDecimal, markerInfo.LongitudeDecimal);
                            var markerIcon = determineMarkerIcon(markerInfo.Direction, markerInfo.Speed);
                            var labelMarker = new MarkerWithLabel({
                                position: pos,
                                labelAnchor: new google.maps.Point(-18, 44),
                                labelClass: "labels",
                                labelStyle: { opacity: 0.75 },
                                icon: markerIcon,
                                map: map,
                                speed: markerInfo.Speed
                            });
                            arrMarkers[i] = labelMarker;

                            labelMarker.setAnimation(google.maps.Animation.DROP);
                            var contentWindowData = "Seq#:" + (i + 1) + "<br/>" + "Speed:" + (Math.floor(markerInfo.Speed));

                            makeInfoWindowEvent(map, infowindow, contentWindowData, labelMarker, i);
                            addTrackerToInfoList(markerInfo, json_parsed.Event.TrackerDetail, i, labelMarker);


                        }
                        panToMarker(arrMarkers[0]);
                    }
                        // else single pointer
                    else {
                        var markerInfo = json_parsed.Event.LocationMessage;
                        var pos = new google.maps.LatLng(markerInfo.LatitudeDecimal, markerInfo.LongitudeDecimal);
                        var markerIcon = determineMarkerIcon(markerInfo.Direction, markerInfo.Speed);
                        var labelMarker = new MarkerWithLabel({
                            position: pos,
                            labelAnchor: new google.maps.Point(-18, 44),
                            labelClass: "labels",
                            labelStyle: { opacity: 0.75 },
                            icon: markerIcon,
                            map: map,
                            speed: markerInfo.Speed
                        });
                        arrMarkers[0] = labelMarker;
                        labelMarker.setAnimation(google.maps.Animation.DROP);
                        var contentWindowData = "Seq#:" + 1 + "<br/>" + "Speed:" + (Math.floor(markerInfo.Speed));

                        makeInfoWindowEvent(map, infowindow, contentWindowData, labelMarker, 1);
                        addTrackerToInfoList(markerInfo, json_parsed.Event.TrackerDetail, 1, labelMarker);
                        panToMarker(arrMarkers[0]);
                    }

                }
            });
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


        function addTrackerToInfoList(markerInfo, TrackerDetail, index, labelMarker) {


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
                html: "<i class='fa fa-clock-o fa-lg grey'></i><b class='m-left-xs'>Reached Location:</b>&nbsp;<span id='lastUpdate" + index + "'>" + markerInfo.FormattedServerRecordedDateTime + " </span>",
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

            var fullScrButtonIcon = $(" <a />", {
                "class": "fa fa-arrows-alt fa-lg grey",
                html: " Full Screen"
            });

            var lbBr = $("<br/>");

            var geoButtonIcon = $(" <a />", {
                html: " Add GeoFence",
                "class": "fa fa-history fa-lg grey btn btn-sm",
                href: "/protected/_managegeofences.aspx?lat=" + markerInfo.LatitudeDecimal + "&lng=" + markerInfo.LongitudeDecimal + "&icon=" + labelMarker.icon,
                target:"_blank"
            });

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
            setReachedLocation($('#lastUpdate' + index), markerInfo.FormattedServerRecordedDateTime);
            setLastReport($('#rpt' + index), TrackerDetail.RptDays, TrackerDetail.RptHrs, TrackerDetail.RptMin, TrackerDetail.RptSec);
            setIdleTime($('#idle' + index), markerInfo.IdleDays, markerInfo.IdleHrs, markerInfo.IdleMin, markerInfo.IdleSec);
            setEngineStat($('#engineStatus' + index), markerInfo.DInput5);

            google.maps.event.addListener(labelMarker, 'click', make_callback(vehicleDetails, TrackerDetail.Id, index));
        }

        function makeInfoWindowEvent(map, infowindow, contentString, index) {
            google.maps.event.addListener(index, 'click', function () {
                currMarkerIndex = index;
                infowindow.setContent(contentString);
                infowindow.open(map, index);

            });
        }

        function clickBounce(t) {

            t.setAnimation(google.maps.Animation.BOUNCE);
            setTimeout(function () { t.setAnimation(null); }, 1400);
        }

        function make_callback(id, did, index) {
            return function () {
                clickBounce(this);
                currMarkerIndex = index;
                $("div#vehicleDetails .list-group").hide();
                id.show();
                $("#currTrackerId").val(did);
            };
        }

        function panToMarker(marker) {
            //map.setZoom(15);
            map.setCenter(marker.getPosition());
            google.maps.event.trigger(marker, 'click');

        }

        function clearMarkers() {

            //clear all current markers on the screen
            for (var index = 0; index < this.arrMarkers.length; index++) {
                this.arrMarkers[index].setMap(null);
            }
            this.arrMarkers = [];
        }

        //// End Mapping functions for Modal


        // Remember pop up is tied to report "View" Link TDO - Change to something graphical
        function ShowMapLoc(eventId) {
            loadEvent(eventId);
            $('#formModal').modal('show');

            setTimeout(function () {
                google.maps.event.trigger(map, 'resize');
                map.setZoom(map.getZoom());
                map.setCenter(new google.maps.LatLng(10.5526, -61.3152));
            }, 300)


        }

        $(document).ready(function () {
            $("#MainContent_txtdate").datepicker({ format: "mm/dd/yyyy" });
        });

        // Placed here for ensureing code ran after partial post back
        function foo() {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
        }
        function endRequestHandler(sender, args) {
            // Do your stuff
            $("#MainContent_txtdate").datepicker({ format: "mm/dd/yyyy" });
            $.getScript("js/chosen.jquery.min.js", function () { });
            $.getScript("js/endless/endless_form.js", function () { });
        }
    </script>


    <!-- Chosen -->
    <script src='js/chosen.jquery.min.js'></script>

    <!-- Endless -->
    <script src="js/endless/endless_form.js"></script>

</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="BreadcrumbContentPlaceHolder">
    <div id="breadcrumb">
        <ul class="breadcrumb" id="breadcrumblist">
        </ul>
    </div>
    <!-- /breadcrumb-->
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:ScriptManager EnablePartialRendering="true" ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updtReport" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="padding-md">
                <div class="row">

                    <div class="col-md-10">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                Set Report Parameters
                            </div>
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">

                                            <div class="col-3">
                                                Select Vehicle: 
                                            </div>
                                            <div class="col-3">
                                                <%--TODO - need to fix this, after update panel this does not revert to normal state. Also text align and size is a bit off--%>
                                                <asp:DropDownList ID="dropDown" runat="server" class="form-control chzn-select larger">
                                                </asp:DropDownList>

                                            </div>

                                        </div>
                                    </div>
                                    <div class="col-md-4">

                                        <div class="form-group">

                                            <div class="col-3">
                                                Enter Date:
                                            </div>

                                            <div class="col-3">
                                                <%-- TODO - Need to default this to today!  Need to also put loading when refreshing report--%>
                                                <asp:TextBox ID="txtdate" CssClass="form-control input-sm" runat="server"></asp:TextBox>
                                                <%-- <span class="add-on"><i class="fa fa-th"></i></span>--%>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-md-3">
                                        <asp:LinkButton ID="btnUpdateReport" runat="server" class="btn btn-success quick-btn" OnClick="btnUpdateReport_Click">
                        <i class="fa fa-music"></i><span>Run Report</span>
                                        </asp:LinkButton>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                Event Report
                            </div>
                            <div class="panel-body" style="height:400px;">
                                <%--<rsweb:ReportViewer ID="reportViewer" runat="server" Height="100%" AsyncRendering="False" SizeToReportContent="True"></rsweb:ReportViewer>--%>
                                <ej:ReportViewer ID="ReportViewer1" runat="server" ReportPath="~/Reports/Events.rdl" ProcessingMode="Remote"/>         
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>

    </asp:UpdatePanel>


    <div class="modal fade" id="formModal">
        <%-- TODO - Friggin Custom .css not seeming to pickup if Bootstrat css applies - Need to fix!!! --%>
        <div class="modal-dialog" style="width: 1300px;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4>Events - Map View</h4>
                </div>
                <div class="modal-body">
                    <div class="padding-md">
                        <div class="row">
                            <div class="col-lg-8">
                                <div id="myTrackerMap" class="gmap3"></div>

                            </div>
                            <div class="col-lg-4">
                                <div id="vehicleDetails" class="panel panel-default bg-success">
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
        <input type="hidden" id="currTrackerId" />
    </div>

</asp:Content>

<asp:Content ID="popups" ContentPlaceHolderID="popUpCont" runat="server"></asp:Content>

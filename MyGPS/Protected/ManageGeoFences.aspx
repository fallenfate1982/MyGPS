<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageGeoFences.aspx.cs" Inherits="MyGPS.Protected.ManageGeoFences" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-boxes" style="min-height: 545px;">
        <div style="width: 320px; float: left;">
            <div class="box">
                <div class="box-b">
                    <div id="vehicleDetailsListing" class="box-t">
                        <h3 style="display: inline-block">Geo Fences</h3>
                        <br />
                        &nbsp;&nbsp;&nbsp;<a id="activeTrack" href="#"></a>
                        <br />
                        &nbsp;&nbsp;&nbsp;
                        <ul data-bind="foreach: $root.fencesData" id="fancesList">
                            <li>
                                <a id="#" data-bind="text: FencesName, click: $root.selectedFenes"></a>
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <a href="#" data-bind="click: $root.deleteFences">
                                    <img src="/images/delete.png" />
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <div style="float: left;margin-top:5px;" class="fancesArea">
            <table>
                <tr>
                    <td>
                        <div>
                            <strong>Search area</strong>
                            <input type="text" id="area_of_interest" />
                        </div>
                    </td>
                    <td>&nbsp;&nbsp;
                        <button class="button" id="polygon_main_add">New Fences</button>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td colspan="2"><input type="hidden" id="hdId"/>
                        <table id="table_paths" style="display:none" >
                            <thead>
                                <tr>
                                    <th>Fences name</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot>
                                <tr>
                                    <td colspan="2">
                                        <button class="button" id="btnSave"> Save </button>
                                        &nbsp;&nbsp;
                                       <!-- <button class="button" id="polygon_reset"> Reset </button> -->
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="map_canvas" class="gmap3" style="height: 500px;"></div>
            <div style="display: none">
                <div><strong>Coordinates:</strong> <span id="coordinates"></span><strong>Zoom:</strong> <span id="zoom"></span></div>
                <h4>Grab the coordinates</h4>
                <div class="export">
                    <span class="label label-success">GeoJSON</span>
                    <span class="label label-important hide" id="geojson_alert">Invalid GeoJSON</span>
                    <textarea rows="5" class="btn-block" id="coordinates_geojson"></textarea>
                    <div class="pull-right">
                        <button class="btn btn-small btn-primary" id="geojson_load">LOAD</button>
                    </div>
                    <div class="clearfix"></div>
                </div>

                <div class="export">
                    <span class="label label-success">KML&lt;Polygon&gt;</span>
                    <textarea rows="5" class="btn-block" id="coordinates_kml"></textarea>
                </div>

                <div class="export">
                    <span class="label label-success">google.maps.Polygon</span>
                    <textarea rows="5" class="btn-block" id="coordinates_gmaps_polygon"></textarea>
                </div>
            </div>
        </div>
    </div>
    <div class="clear-fix"></div>
    <script type="text/javascript" src="http://code.jquery.com/jquery-1.9.1.js"></script>
    <script src="http://maps.googleapis.com/maps/api/js?v=3.12&amp;sensor=false&amp;libraries=places"></script>
    <script type="text/javascript" src="/Scripts/page.js"></script>
    <script src="/Scripts/knockout-2.3.0.debug.js"></script>
    <script type="text/javascript">
        
        var fences = function () {
            debugger;
            self = this;
            self.fencesData = ko.observableArray([]);
            self.deleteFences = function (data) {
                var yn = confirm("Current drawing will be erased\nDo you want to continue ?");
                if (!yn) return;
                $.ajax({
                    url: '/api/fences?id=' + data.FencesId,
                    type: 'DELETE',
                    dataType: "json",
                    success: function (response) {
                        clearForms();
                        self.fencesData.remove(data);
                    },
                    error: function (error) {
                        jsonValue = jQuery.parseJSON(error.responseText);
                        alert(jsonValue);
                        //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
                    }
                });

            };
            self.selectedFenes = function (data) {
                $('#coordinates_geojson').val(data.FencesCoordinate);
                $("#hdId").val(data.FencesId);
                $('#geojson_load').trigger('click');
                $('#polygon_main_add').prop('disabled', true);
                alert('ok');
                $("#table_paths").show();
                $('#table_paths tbody').find(".input").val(data.FencesName);
                return false;
            };
        };
        
        var vm = new fences();
        
        $(document).ready(function () {
            getFences();
        });
        
        function getFences() {
            loadFences();
            ko.applyBindings(vm, $("#fancesList")[0]);
        }

        $("#btnSave").click(function () {

            var source = {
                'FencesId' : $("#hdId").val(),
                'FencesName': $('#table_paths tbody').find(".input").val(),
                'FencesCoordinate': $('#coordinates_geojson').val(),
                //'VendorID': $('#Vendors').val()
            };
            $.ajax({
                type: "POST",
                dataType: "json",
                url: "/api/fences",
                data: source,
                success: function (data) {
                    loadFences();
                },
                error: function (error) {
                    jsonValue = jQuery.parseJSON(error.responseText);
                    alert(jsonValue);
                    //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
                }
            });

            clearForms();
            
            return false;
        });

        function loadFences() {
            $.ajax({
                type: "GET",
                dataType: "json",
                url: "/api/fences",
                success: function (data) {
                    vm.fencesData(data);
                },
                error: function (error) {
                    jsonValue = jQuery.parseJSON(error.responseText);
                    alert(jsonValue);
                    //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
                }
            });
        }
        
    </script>
</asp:Content>

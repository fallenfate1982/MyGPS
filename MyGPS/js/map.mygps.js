var poly, map;
var markers = [];
var path = new google.maps.MVCArray;
var initX = 10.377064;
var initY = -61.23024;
var tnt = new google.maps.LatLng(initX, initY );

function setInitialLocation() {
    path = new google.maps.MVCArray;
    tnt = new google.maps.LatLng(initX, initY);
}
function initializeMap(zm) {
    
    var z;
    if (zm != null)
        z = zm;
    else
        z = 10;
        
    map = new google.maps.Map(document.getElementById("map_canvas"), {
      
        zoom: z,
        center: tnt,
        mapTypeId: google.maps.MapTypeId.HYBRID
    });

    poly = new google.maps.Polygon({
        strokeWeight: 2,
        fillColor: '#5555FF',
        editable: true
    });
    poly.setMap(map);
    poly.setPaths(new google.maps.MVCArray([path]));

    google.maps.event.addListener(map, 'click', addPoint);

    var autocomplete = new google.maps.places.Autocomplete($('#searchArea')[0], {
        types: ['geocode']
    });
    autocomplete.bindTo('bounds', map);
    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace();
        if ((typeof place.geometry) === 'undefined') {
            geocoder.geocode({ 'address': place.name }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    var place = results[0];
                    $('#area_of_interest').val(place.formatted_address);
                    map.fitBounds(place.geometry.viewport);
                }
            });
        } else {
            map.fitBounds(place.geometry.viewport);
        }
    });

}

function addPoint(event) {
    path.insertAt(path.length, event.latLng);

    var marker = new google.maps.Marker({
        position: event.latLng,
        map: map,
        draggable: true
    });
    markers.push(marker);
    marker.setTitle("#" + path.length);

    google.maps.event.addListener(marker, 'click', function () {
        marker.setMap(null);
        for (var i = 0, I = markers.length; i < I && markers[i] != marker; ++i);
        markers.splice(i, 1);
        path.removeAt(i);
    }
    );

    google.maps.event.addListener(marker, 'dragend', function () {
        for (var i = 0, I = markers.length; i < I && markers[i] != marker; ++i);
        path.setAt(i, marker.getPosition());
    }
    );
}

function loadPolygon(coordinate,zm) {
    var x = 0, y = 0;
    var myObject = eval('(' + coordinate + ')');
    path = new google.maps.MVCArray;
    for (i in myObject) {
        if (x == 0) {
            x = myObject[i]["lat"];
            y = myObject[i]["lg"];
        }

        var latlang = new google.maps.LatLng(myObject[i]["lat"], myObject[i]["lg"]);
        path.push(latlang);
    }
    
    poly.setPaths(new google.maps.MVCArray([path]));
    tnt = new google.maps.LatLng(x, y);
    initializeMap(zm);
}

/*$(function () {
    $(document).ready(function () {
        $("#test").click(function () {
            var pPath = poly.getPath();
            var ar = pPath.getArray();
            for (var i = 0 ; i < ar.length  ; i++) {
                document.write(ar[i].lb);
            }
        });
    });            
});*/

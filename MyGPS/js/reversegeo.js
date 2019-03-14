var POST_REVERSE_URL = 'http://open.mapquestapi.com/geocoding/v1/reverse?key=Fmjtd%7Cluur2ha7nu%2Cb0%3Do5-9wb01f&location=';

function getReverseURL(latitude, longitude) {
    var newURL = POST_REVERSE_URL + latitude + "," + longitude;
    return newURL;
};

function processResponse(response) {
    var location = response.results[0].locations[0];
    var address = "";
    if (location.street != null || location.street != "") {
        address += location.street;
    }
    if (location.adminArea5) {
        address += ", " + location.adminArea5;
    }
    if (location.adminArea4) {
        address += ", " + location.adminArea4;
    }
    if (location.adminArea3) {
        address += ", " + location.adminArea3;
    }
    if (location.adminArea1) {
        address += ", " + location.adminArea1;
    }
    return address.toUpperCase();
}

//$.ajax({
//    type: "GET",
//    dataType: "json",
//    url: getReverseURL(10.251219, -61.482478),
//    data: '',
//    success: function (response) {
//        alert(processResponse(response));
//    },
//    error: function (error) {
//        return "Address cannot be retrieved."
//    }
//});

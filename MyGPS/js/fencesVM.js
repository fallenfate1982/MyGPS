

var fences = function () {
    self = this;
    self.fencesData = ko.observableArray([]);
    self.deleteFences = function (data) {
        var yn = confirm("The selected GeoFence will be deleted.\nDo you want to continue?");
        if (!yn) return;
        $.ajax({
            url: '/api/fences/DeleteFences?id=' + data.FencesId,
            type: 'DELETE',
            dataType: "json",
            success: function (response) {
                self.fencesData.remove(data);
                setInitialLocation();
                initializeMap();
                clearForm();
            },
            error: function (error) {
                jsonValue = jQuery.parseJSON(error.responseText);
                alert(jsonValue.Message);
                //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
            }
        });

    };
    self.selectedFences = function (data) {
        var coordinate = " [" + data.FencesCoordinate + "]";
        $("#hdId").val(data.FencesId);
        $("#dlCategory").val(data.FencesTypeId);
        $("#txtName").val(data.FencesName);
        $("#txtDetails").val(data.Details);
        var zm = data.Zoom
        $("#chkIsPublic").prop("checked", data.IsPublic);
        if (data.IsPublic == true) {

            $("#fenceListing").hide();
            $("#map").show();
            $("#btnCancel").show();
        }
        else {
            $("#fenceListing").hide();
            $("#map").show();
            $("#btnCancel").show();
            $("#btnSave").show();
        }
        loadPolygon(coordinate, zm);

        getTrackers();

        return false;
    };
};

function loadFences(vm) {
    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/api/fences/GetAllFances",
        success: function (data) {
            vm.fencesData(data);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            alert(JSON.stringify(jsonValue));
            //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
        }
    });
}

function getTrackers() {

    var fenceId = 0;
    var id = $("#hdId").val();
    if (id != "")
        fenceId = id;

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/api/Fences/GetTrackersByFenceId?id=" + fenceId,
        success: function (data) {
            
            var inHTML = "";
            $.each(data.TrackersOut, function (i, ob) {
                inHTML += '<option value="' + ob.Id + '" data-key="' + ob.Id + '">' + ob.Name + '</option>';
            });

            $("#lbNotTrackers").html(inHTML);
            inHTML = "";
            $.each(data.TrackersIn, function (i, ob) {
                inHTML += '<option value="' + ob.Id + '" data-key="' + ob.Id + '">' + ob.Name + '</option>';
            }); 
            $("#lbTrackers").html(inHTML);
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
            alert(JSON.stringify(jsonValue));
            //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
        }
    });
}

$('#btnSelect').click(function () {    
    addTracker();
});

$('#btnRemove').click(function () {    
    removeTracker();
});

$('#btnSelectAll').click(function () {
    $("#lbNotTrackers > option").prop("selected", true);
    addTracker();
});

$('#btnRemoveAll').click(function () {
    $("#lbTrackers > option").prop("selected", true);
    removeTracker();
});

function addTracker() {
    var itemsToAdd = [];
    $("#lbNotTrackers option:selected").each(function () {
        var optionVal = $(this).val();
        var key = $(this).data('key');
        itemsToAdd.push($(this).clone(true));
    });

    if (itemsToAdd.length == 0) return;

    $("#lbNotTrackers option:selected").remove();
    $("#lbTrackers").append(itemsToAdd);
}

function removeTracker() {

    var itemsToAdd = [];
    $("#lbTrackers option:selected").each(function () {
        var optionVal = $(this).val();
        var key = $(this).data('key');
        itemsToAdd.push($(this).clone(true));
    });

    if (itemsToAdd.length == 0) return;

    $("#lbTrackers option:selected").remove();
    $("#lbNotTrackers").append(itemsToAdd);
}

function clearForm() {
    //$("#hdId").val("0");
    //$("#dlCategory").val("0");
    //$("#txtName").val("");
    //$("#txtDetails").val("");
    //$("#chkIsPublic").prop("checked", false);
}
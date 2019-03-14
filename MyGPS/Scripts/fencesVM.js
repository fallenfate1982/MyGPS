

var fences = function () {
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
                self.fencesData.remove(data);
                setInitialLocation();
                initializeMap();
                clearForm();
            },
            error: function (error) {
                jsonValue = jQuery.parseJSON(error.responseText);
                alert(jsonValue);
                //jError('An error has occurred while saving the new part source: ' + jsonValue, { TimeShown: 3000 });
            }
        });

    };
    self.selectedFenes = function (data) {
        var coordinate = " [" + data.FencesCoordinate + "]";
        $("#hdId").val(data.FencesId);
        $("#dlCategory").val(data.FencesTypeId);
        $("#txtName").val(data.FencesName);
        $("#txtDetails").val(data.Details);
        var zm = data.Zoom
        $("#chkIsPublic").prop("checked", data.IsPublic);
        if (data.IsPublic == true) {

            $("#btnSave").hide();
            $("#btnCancel").show();
              $("#btnNew").hide();
        }
        else {
            $("#btnNew").hide();
            $("#btnCancel").show();
            $("#btnSave").hide();
            $("#BtnEdit").show();
        }
        loadPolygon(coordinate,zm);
        return false;
    };
};

function loadFences(vm) {
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

function clearForm() {
    $("#hdId").val("0");
    $("#dlCategory").val("0");
    $("#txtName").val("");
    $("#txtDetails").val("");
    $("#chkIsPublic").prop("checked", false);
}
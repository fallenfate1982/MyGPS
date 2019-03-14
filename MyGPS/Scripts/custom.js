function setMileage(element, mileage) {
    element.html(mileage + " km");
    element.removeClass();
    element.addClass("badge");
    element.addClass("badge-info")
}

function setSpeedBadge(element, speed) {
    element.html(speed + " kmph");
    element.removeClass();
    element.addClass("badge");

    if (speed > 80) { element.addClass("badge-danger"); }
    else if (speed > 60) { element.addClass("badge-warning") }
    else if (speed > 20) { element.addClass("badge-success") }
    else { element.addClass("badge-info") }
}

function setReachedLocation(element, date) {
    element.html(date);
    element.removeClass();
    element.addClass("badge");

    element.addClass("badge-info");
}

function setIdleTime(element, IdleDays, IdleHrs, IdleMin, IdleSec) {
    element.html(IdleDays + "Days " + IdleHrs + "Hrs " + IdleMin + "Mins " + IdleSec + "Sec");
    element.removeClass();
    element.addClass("badge");

    if (IdleHrs > 21 || IdleDays >0) { element.addClass("badge-danger"); }
    else if (IdleHrs > 12) { element.addClass("badge-warning") }
    else if (IdleMin > 20) { element.addClass("badge-success") }
    else { element.addClass("badge-info") }
}

function setLastReport(element, rptDays, rptHrs, rptMin, rptSec) {
    element.html(rptDays +"Days " + rptHrs + "Hrs " + rptMin + "Mins " + rptSec + "Sec");
    element.removeClass();
    element.addClass("badge");

    if (rptHrs > 21 || rptDays >0) { element.addClass("badge-danger"); }
    else if (rptMin > 20) { element.addClass("badge-warning") }
    else { element.addClass("badge-success") }
}

function setEngineStat(element, status) {
    element.removeClass();
    element.addClass("badge");

    if (status) {
        element.addClass("badge-success");
        element.html("Running");
    }
    else {
        element.addClass("badge-warning")
        element.html("Off");
    }
}


function setInput(element, status) {
    element.removeClass();
    element.addClass("badge");

    if (status) {
        element.addClass("badge-success");
        element.html("On");
    }
    else {
        element.addClass("badge-warning")
        element.html("Off");
    }
}


$(document).ready(function () {
    App.init();

    loadEvents();

});

var applicants = [];

function patientId() {
    if (!localStorage["patientId"]) {
        localStorage["patientId"] = DayPilot.guid();
    }
    return localStorage["patientId"];
}



var loadEvents = function (day) {
    const start = nav.visibleStart() > DayPilot.Date.now() ? nav.visibleStart() : DayPilot.Date.now();
    const end = nav.visibleEnd();
    const patient = patientId();
    
    var a = $(this).closest(".panel");

    $.ajax({
        url: '/api/appointments/free?start=' + start + '&end= ' + end + '&patient=' + patient,
        type: "GET",
        success: function (data) {
            if (day) {
                calendar.startDate = day;
            }
            calendar.events.list = data;
            calendar.update();

            nav.events.list = data;
            nav.update();
        },
        error: function (xhr, textStatus, errorThrown) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            Swal.fire({
                title: "Failed",
                text: "Record could not be saved " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });       
}

const nav = new DayPilot.Navigator("nav", {
    selectMode: "week",
    showMonths: 3,
    skipMonths: 3,
    onTimeRangeSelected: (args) => {
        const weekStarts = DayPilot.Locale.find(nav.locale).weekStarts;
        const start = args.start.firstDayOfWeek(weekStarts);
        const end = args.start.addDays(7);
        loadEvents(start, end);
    }

});

nav.init();

const calendar = new DayPilot.Calendar("calendar", {
    viewType: "Week",
    timeRangeSelectedHandling: "Disabled",
    eventMoveHandling: "Disabled",
    eventResizeHandling: "Disabled",
    eventArrangement: "SideBySide",
    onBeforeEventRender: (args) => {
        switch (args.data.status) {
            case "free":
                args.data.backColor = "#3d85c6";  // blue
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                args.data.html = `Available<br/>${args.data.doctorName}`;
                args.data.toolTip = "Click to request this time slot";
                break;
            case "waiting":
                args.data.backColor = "#e69138";  // orange
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                args.data.html = "Your appointment, waiting for confirmation";
                break;
            case "confirmed":
                args.data.backColor = "#6aa84f";  // green
                args.data.barHidden = true;
                args.data.borderColor = "darker";
                args.data.fontColor = "white";
                args.data.html = "Your appointment, confirmed";
                break;
        }
    },
    onEventClick: async (args) => {
        if (args.e.data.status !== "free") {
            calendar.message("Please use a free slot to request an appointment.");
            return;
        }
        const data = {
            id: args.e.id(),
            start: args.e.start(),
            end: args.e.end(),
            patient: patientId()
        };

        
        $('.modal-body #start_date').val(data.start);
        $('.modal-body #end_date').val(data.end);
        $('.modal-body #recordid').val(data.id);

        $("#capture-record").appendTo("body").modal("show");

        

    }
});

calendar.init();

App = function () {
     "use strict";
     return {
         init: function () {
             
         }
     }
}();

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var patient_id = document.getElementById('patient_id').value;
    var name = document.getElementById('name').value;
    var remarks = document.getElementById('remarks').value;

    var parameter = {
        id: id,
        patient_id: patient_id,
        name: name,
        remarks: remarks
    };


    $.ajax({
        url: "/api/appointments/request",
        type: "POST",
        data: JSON.stringify(parameter),
        contentType: "application/json;charset=utf-8",
        success: function (data) {
            var kk = data;
            console.log(kk);
            if (data == 'Success') {

                $("#capture-record").appendTo("body").modal("hide");
                loadEvents();
                
            }

        },

    });
});

$("#capture-record").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#task_name').val("");
    $('#assignee').val("-1").trigger("change");
    $('#start_date').val("");
    $('#end_date').val("");
    $('#remarks').val("");
});
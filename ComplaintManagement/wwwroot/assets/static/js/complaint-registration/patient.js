
$(document).ready(function () {
    App.init();
    //TableManageResponsive.init();

    var InitiateEditableDataTable = function () {
        return {
            init: function () {
                //Datatable Initiating
                var oTable = $('#editabledatatable').dataTable({
                    "responsive": true,
                    "createdRow": function (row, data, dataIndex) {
                        $(row).attr("recid", data.id);
                    },
                    "aoColumns": [
                        { "data": "hospital", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "department", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "doctor", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "age", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "residency", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "phone_no", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "insurance", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "insurance_no", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "health_issue", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "book_appointment_date", "autoWidth": true, "sDefaultContent": "n/a" },

                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i class='fa fa-edit'></i> Edit</a>"
                        },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-danger btn-xs delete'><i class='fa fa-trash'></i> Delete</a>"
                        }
                    ]
                });

                var isEditing = null;

                //Edit
                $('#editabledatatable').on("click", 'a.edit', function (e) {
                    e.preventDefault();

                    nRow = $(this).parents('tr')[0];

                    //console.log($(this).parents('tr').attr("recid"));

                    //console.log(nRow);

                    if (isEditing !== null && isEditing != nRow) {
                        //restoreRow(oTable, isEditing);
                        editRow(oTable, nRow);
                        isEditing = nRow;
                    } else {
                        editRow(oTable, nRow);
                        isEditing = nRow;
                    }
                });

                function editRow(oTable, nRow) {
                    var aData = oTable.fnGetData(nRow);
                    var jqTds = $('>td', nRow);

                    var json = JSON.parse(JSON.stringify(aData));
                    console.log(json);
                    console.log($('.modal-body #recordid').val($(nRow).attr("recid")));

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #name').val(json["name"]);
                    $('.modal-body #age').val(json["age"]).trigger("change");
                    $('.modal-body #residency').val(json["residency"]);
                    $('.modal-body #phone_no').val(json["phone_no"]);
                    $('.modal-body #insurance').val(json["insurance"]).trigger("change");
                    $('.modal-body #insurance_no').val(json["insurance_no"]);
                    $('.modal-body #health_issue').val(json["health_issue"]).trigger("change");
                    $('.modal-body #book_appointment_date').val(json["book_appointment_date"]).trigger("change");
                    $('.modal-body #hospital_id').val(json["hospital_id"]).trigger("change");
                    $('.modal-body #department_id').val(json["department_id"]).trigger("change");
                    $('.modal-body #doctor_id').val(json["doctor_id"]).trigger("change");

                    $("#capture-record").appendTo("body").modal("show");
                }

                //Delete an Existing Row
                $('#editabledatatable').on("click", 'a.delete', function (e) {
                    e.preventDefault();

                    var a = $(this).closest(".panel");

                    var nRow = $(this).parents('tr')[0];

                    var rec = $(this).parents('tr').attr("recid");

                    Swal.fire({
                        title: "Are you sure?",
                        text: "You want to delete this record",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Proceed!",
                        reverseButtons: true
                    }).then((result) => {
                        if (result.isConfirmed) {
                            //$.blockUI();

                            oTable.fnDeleteRow(nRow);
                            //Ajax to flag as deleted
                            var parameters = { module: 'patient_record', id: rec };
                            $.ajax({
                                url: "/HealthManagement/Delete",
                                type: "GET",
                                data: parameters,
                                beforeSend: function () {
                                    if (!$(a).hasClass("panel-loading")) {
                                        var t = $(a).find(".panel-body"),
                                            i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                                        $(a).addClass("panel-loading"), $(t).prepend(i);
                                    }
                                },
                                success: function (data) {
                                    //$.unblockUI();
                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                    Swal.fire({
                                        title: "Deleted",
                                        text: "Record has been deleted",
                                        icon: "success",
                                        confirmButtonText: "Ok"
                                    });
                                    GetPatient();
                                },
                                error: function (xhr, textStatus, errorThrown) {
                                    //$.unblockUI();
                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                    Swal.fire({
                                        title: "Failed",
                                        text: "Operation could not be completed " + errorThrown,
                                        icon: "error",
                                        confirmButtonText: "Ok"
                                    });
                                }
                            });
                        } else {
                            e.preventDefault();
                        }
                    });
                });
            }
        };
    }();

    GetPatient();
    GetHospital();
    GetDepartment();
    GetDoctors();
    

    InitiateEditableDataTable.init();

    var FormPlugins = function () {
        "use strict";
        return {
            init: function () {
                handleSelectpicker();
            }
        }
    }();

    FormPlugins.init();

    
});

handleSelectpicker = function () {
    $(".selectpicker").select2()
}


function GetPatient() {
    $.get('GetRecords', { module: 'patient_record' }, function (data) {
        table = $('#editabledatatable').dataTable();
        getData(table, data);
    });
}
function getData(table, jsonstring) {
    oSettings = table.fnSettings();
    table.fnClearTable(this);

    var json = $.parseJSON(JSON.stringify(jsonstring));
    for (var i = 0; i < json.length; i++) {
        var item = json[i];
        table.oApi._fnAddData(oSettings, item);
    }
    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
    table.fnDraw();
}

function GetHospital() {
    $.get('GetRecords', { module: 'hospital_record' }, function (data) {
        $("#hospital_id").get(0).options.length = 0;
        $("#hospital_id").get(0).options[0] = new Option("Please Select Hospital", "-1");

        $.each(data, function (index, item) {
            $("#hospital_id").get(0).options[$("#hospital_id").get(0).options.length] = new Option(item.hospital_name, item.id);
        });

        $("#hospital_id").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

function GetDepartment() {
    $.get('GetRecords', { module: 'department_record' }, function (data) {
        $("#department_id").get(0).options.length = 0;
        $("#department_id").get(0).options[0] = new Option("Please Select Department", "-1");

        $.each(data, function (index, item) {
            $("#department_id").get(0).options[$("#department_id").get(0).options.length] = new Option(item.department_name, item.id);
        });

        $("#department_id").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

function GetDoctors() {
    $.get('GetRecords', { module: 'doctors_record' }, function (data) {
        $("#doctor_id").get(0).options.length = 0;
        $("#doctor_id").get(0).options[0] = new Option("Please Select Doctor", "-1");

        $.each(data, function (index, item) {
            $("#doctor_id").get(0).options[$("#doctor_id").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#doctor_id").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var name = document.getElementById('name').value;
    var age = document.getElementById('age').value;
    var residency = document.getElementById('residency').value;
    var phone_no = document.getElementById('phone_no').value;
    var insurance = document.getElementById('insurance').value;
    var insurance_no = document.getElementById('insurance_no').value;
    var health_issue = document.getElementById('health_issue').value;
    var book_appointment_date = document.getElementById('book_appointment_date').value;
    var hospital_id = document.getElementById('hospital_id').value;
    var department_id = document.getElementById('department_id').value;
    var doctor_id = document.getElementById('doctor_id').value;
   
    var parameters = {
        id: id,
        name: name,
        age: age,
        residency: residency,
        phone_no: phone_no,
        insurance: insurance,
        insurance_no: insurance_no,
        health_issue: health_issue,
        book_appointment_date: book_appointment_date,
        hospital_id: hospital_id,
        department_id: department_id,
        doctor_id: doctor_id


        
    };
    console.log(parameters);

    $.ajax({
        url: "/HealthManagement/CreatePatient",
        type: "POST",
        data: parameters,
        beforeSend: function () {
            if (!$(a).hasClass("panel-loading")) {
                var t = $(a).find(".panel-body"),
                    i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                $(a).addClass("panel-loading"), $(t).prepend(i);
            }
        },
        success: function (data) {
            //$.unblockUI();
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            if (data == 'Success') {
                $("#capture-record").modal("hide").data("bs.modal", null);
                GetNurse();
                
            } else {
                Swal.fire({
                    title: "Failed",
                    text: data,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            //$.unblockUI();
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            Swal.fire({
                title: "Failed",
                text: "Operation could not be completed " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

$("#capture-record").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#name').val("");
    $('#age').val("").trigger('change');
    $('#residency').val("");
    $('#phone_no').val("");
    $('#insurance').val("");
    $('#insurance_no').val("").trigger('change');
    $('#health_issue').val("").trigger('change');
    $('#book_appointment_date').val("").trigger('change');
    $('#hospital_id').val("").trigger('change');
    $('#department_id').val("").trigger('change');
    $('#doctor_id').val("").trigger('change');

});

$('#book_appointment_date').datepicker({
    todayHighlight: true,
    startDate: '',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

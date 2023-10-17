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
                        { "data": "first_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "employer_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "phone_number", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "postal_address", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "position", "autoWidth": true, "sDefaultContent": "n/a" },

                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i class='fa fa-edit'></i> Edit</a>"
                        },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-danger btn-xs delete'><i class='fas fa-trash-alt'></i> Delete</a>"
                        }
                    ]
                });

                var isEditing = null;

                //Edit
                $('#editabledatatable').on("click", 'a.edit', function (e) {
                    e.preventDefault();

                    nRow = $(this).parents('tr')[0];

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

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #mobile_number').val(json["mobile_number"]);
                    $('.modal-body #first_name').val(json["first_name"]);
                    $('.modal-body #middle_name').val(json["middle_name"]);
                    $('.modal-body #last_name').val(json["last_name"]);
                    $('.modal-body #patient_id').val(json["patient_id"]);
                    $('.modal-body #employer_name').val(json["employer_name"]);
                    $('.modal-body #phone_number').val(json["phone_number"]);
                    $('.modal-body #postal_address').val(json["postal_address"]);
                    $('.modal-body #physical_address').val(json["physical_address"]);
                    $('.modal-body #email').val(json["email"]);
                    $('.modal-body #position').val(json["position"]);
                    $('.modal-body #position').val(json["position"]);
                    $('#information').show();
                    $('#info').show();

                    $("#capture-employer-details").appendTo("body").modal("show");

                }

                //Delete an Existing Row
                $('#editabledatatable').on("click", 'a.delete', function (e) {
                    e.preventDefault();
                    var a = $(this).closest(".panel");

                    var nRow = $(this).parents('tr')[0];

                    var parameters = {
                        id: $(this).parents('tr').attr("recid"),
                        module: 'employer'
                    };

                    Swal.fire({
                        title: "Are you sure?",
                        text: "You want to delete this record",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Proceed!",
                        reverseButtons: true
                    }).then((result) => {
                        if (result.isConfirmed) {

                            $.ajax({
                                url: "/Patient/Delete",
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
                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                    oTable.fnDeleteRow(nRow);
                                    Swal.fire({
                                        title: "Deleted",
                                        text: "Record has been deleted",
                                        icon: "success",
                                        confirmButtonText: "Ok"
                                    });

                                    GetPatientEmployer();
                                },
                                error: function (xhr, textStatus, errorThrown) {
                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                    Swal.fire({
                                        title: "Failed",
                                        text: "Record could not be deleted " + errorThrown,
                                        icon: "error",
                                        confirmButtonText: "Ok"
                                    });
                                }
                            });
                        } else {
                            return;
                            e.preventDefault();
                        }
                    });
                });
            }
        };
    }();

    InitiateEditableDataTable.init();
    GetPatientEmployer();
});

$('#expiry_date').datepicker({
    todayHighlight: true,
    startDate: '-6m',
    //endDate: '0',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

$('#mobile_number').change(function () {

    var mobile = document.getElementById('mobile_number').value;
    //console.log(account_number);
    var parameters = { param: mobile };

    $.ajax({
        url: "/Patient/account",
        type: "GET",
        data: parameters,
        success: function (data) {


            if (mobile == data.mobile) {
                $('#first_name').val(data.first);
                $('#middle_name').val(data.middle);
                $('#last_name').val(data.last);
                $('#patient_id').val(data.id);
                $('#information').show();
                $('#info').show();

            }

            else {
                Swal.fire({
                    title: "Failed",
                    text: + mobile + " Record could not be Found ",
                    icon: "error",
                    confirmButtonText: "Ok"
                });

                $('#information').hide();
                $('#info').hide();

            }
        }
    });

});


function GetPatientEmployer() {
    $.get('GetRecords', { module: 'patient_employer' }, function (data) {
        getData(data);
    });
}

function getData(jsonstring) {
    table = $('#editabledatatable').dataTable();
    oSettings = table.fnSettings();
    table.fnClearTable(this);

    var json = $.parseJSON(JSON.stringify(jsonstring));
    //var json = JSON.parse(jsonstring);
    for (var i = 0; i < json.length; i++) {
        var item = json[i];
        table.oApi._fnAddData(oSettings, item);
    }
    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
    table.fnDraw();
}

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var patient_id = document.getElementById('patient_id').value
    var employer_name = document.getElementById('employer_name').value;
    var phone_number = document.getElementById('phone_number').value;
    var postal_address = document.getElementById('postal_address').value;
    var physical_address = document.getElementById('physical_address').value;
    var email = document.getElementById('email').value;
    var position = document.getElementById('position').value;

    var parameters = {
        id: id,
        patient_id: patient_id,
        employer_name: employer_name,
        mobile_number: phone_number,
        postal_address: postal_address,
        physical_address: physical_address,
        email: email,
        position: position
    };

    $.ajax({
        url: "/Patient/CreatePatientEmployer",
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
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
            

            if (data.error_code === '00') {
                $("#capture-employer-details").appendTo("body").modal("hide");
                GetPatientEmployer();

                Swal.fire({
                    title: "Success",
                    text: data.error_desc,
                    icon: "success",
                    confirmButtonText: "Ok"
                });
            }
            else {
                Swal.fire({
                    title: "Failed",
                    text: data.error_desc,
                    icon: "error",
                    confirmButtonText: "Ok"
                });
            }

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
});

$("#capture-employer-details").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#mobile_number').val("");
    $('#first_name').val("");
    $('#middle_name').val("");
    $('#last_name').val("");
    $('#patient_id').val("");
    $('#employer_name').val("");
    $('#phone_number').val("");
    $('#postal_address').val("");
    $('#physical_address').val("");
    $('#email').val("");
    $('#position').val("");
    $('#information').hide();
    $('#info').hide();
});
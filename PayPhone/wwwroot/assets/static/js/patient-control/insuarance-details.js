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
                        { "data": "insured_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "insurance_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "scheme_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "member_no", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "expiry_date", "autoWidth": true, "sDefaultContent": "n/a" },

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
                    //console.log(json);

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #mobile_number').val(json["mobile_number"]);
                    $('.modal-body #first_name').val(json["first_name"]);
                    $('.modal-body #middle_name').val(json["middle_name"]);
                    $('.modal-body #last_name').val(json["last_name"]);
                    $('.modal-body #patient_id').val(json["patient_id"]);
                    $('.modal-body #insurance_name').val(json["insurance_id"]).trigger("change");
                    $('.modal-body #scheme_name').val(json["scheme_name"]);
                    $('.modal-body #insured_name').val(json["insured_name"]);
                    $('.modal-body #member_no').val(json["member_no"]);
                    $('.modal-body #description').val(json["description"]);
                    $('.modal-body #expiry_date').val(json["expiry_date"]);

                    $('#information').show();
                    $('#info').show();

                    $("#capture-insuarance").appendTo("body").modal("show");

                }

                //Delete an Existing Row
                $('#editabledatatable').on("click", 'a.delete', function (e) {
                    e.preventDefault();
                    var a = $(this).closest(".panel");

                    var nRow = $(this).parents('tr')[0];

                    var parameters = {
                        id: $(this).parents('tr').attr("recid"),
                        module: 'insuarance'
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
                                    getData(data);
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
    GetPatientRecords();
    GetPatientInsuarance();

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

function GetPatientInsuarance() {
    $.get('GetRecords', { module: 'patient_insuarance' }, function (data) {
        $("#insurance_name").get(0).options.length = 0;
        $("#insurance_name").get(0).options[0] = new Option("Please Select insurance_name", "-1");

        $.each(data, function (index, item) {
            $("#insurance_name").get(0).options[$("#insurance_name").get(0).options.length] = new Option(item.insurance_name, item.id);
        });

        $("#insurance_name").bind("change", function () {
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

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

function GetPatientRecords() {
    $.get('GetRecords', { module: 'patient_insuarance_record' }, function (data) {
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
    var patient_id = document.getElementById('patient_id').value;
    var insurance_name = document.getElementById('insurance_name').value;
    var scheme_name = document.getElementById('scheme_name').value;
    var insured_name = document.getElementById('insured_name').value;
    var member_no = document.getElementById('member_no').value;
    var expiry_date = document.getElementById('expiry_date').value;

    var parameters = {
        id: id,
        patient_id: patient_id,
        insurance_name: insurance_name,
        scheme_name: scheme_name,
        insured_name: insured_name,
        member_no: member_no,
        expiry_date: expiry_date
    };
    //console.log(parameters);

    $.ajax({
        url: "/Patient/CreatePatientInsuarance",
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

            if (data.error_code === '00') {

                $("#capture-insuarance").appendTo("body").modal("hide");
                GetPatientRecords();

                Swal.fire({
                    title: "Success",
                    text: data.error_desc,
                    icon: "success",
                    confirmButtonText: "Ok"
                });

                ;

            } else {
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

$("#capture-insuarance").on("hidden.bs.modal", function (e) {
    $('#recordid').val("");
    $('#mobile_number').val("");
    $('#first_name').val("");
    $('#middle_name').val("");
    $('#last_name').val("");
    $('#patient_id').val("");
    $('#scheme_name').val("");
    $('#insurance_name').val("").trigger('change');
    $('#insured_name').val("");
    $('#member_no').val("");
    $('#description').val("");
    $('#expiry_date').val("");
    $('#information').hide();
    $('#info').hide();
});

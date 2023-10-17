$(document).ready(function () {
    App.init();

    $("#invoice_type").bind("change", function () {

       
        var y = document.getElementById("comp_div");

        var str = $("#invoice_type option:selected").val();

        if (str == 'Hourly Rate') {
            y.style.display = "block";
        }
        else {
            y.style.display = "none";

        }
    });

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
                        { "data": "invoice_number", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "invoice_type", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "client", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "amount", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "date_issued", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "due_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        {
                            "data": "client_type",
                            "autoWidth": true,
                            "bSearchable": false,
                            "bSortable": false,
                            "sDefaultContent": ""
                        },
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

                //$('#editabledatatable').on("click", 'a.jobgroup', function (e) {
                //    e.preventDefault();
                //    nRow = $(this).parents('tr')[0];

                //    $('.modal-body #jobgroupclientid').val($(nRow).attr("recid"));

                //    GetClient_Job_Group($(nRow).attr("recid"));

                //    $("#capture-jobgroup-record").appendTo("body").modal("show");
                //});

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

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #invoice_type').val(json["invoice_type"]);
                    $('.modal-body #client_id').val(json["client_id"]).trigger("change");
                    $('.modal-body #invoice_number').val(json["invoice_number"]);
                    $('.modal-body #date_issued').val(json["date_issued"]);
                    $('.modal-body #due_date').val(json["due_date"]);
                    $('.modal-body #amount').val(json["amount"]);
                    $('.modal-body #tax').val(json["tax"]);
                    $('.modal-body #duration').val(json["duration"]);
                    $('.modal-body #hourly_rate').val(json["hourly_rate"]);
                    $('.modal-body #description').val(json["description"]);
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
                            var parameters = { module: 'matter', id: rec };
                            $.ajax({
                                url: "/Data/Delete",
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
                                    GetPartners();
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
    GetInvoice();

    GetClient();



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


function GetInvoice() {
    $.get('GetRecords', { module: 'invoice' }, function (data) {
        table = $('#editabledatatable').dataTable();
        getData(table, data);
    });
}

function GetClient() {
    $.get('GetRecords', { module: 'client_record' }, function (data) {
        $("#client_id").get(0).options.length = 0;
        $("#client_id").get(0).options[0] = new Option("Please Select Client", "-1");

        $.each(data, function (index, item) {
            $("#client_id").get(0).options[$("#client_id").get(0).options.length] = new Option(item.client_name, item.id);
        });

        $("#client_id").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
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


$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var invoice_type = document.getElementById('invoice_type').value;
    var client_id = document.getElementById('client_id').value;
    var invoice_number = document.getElementById('invoice_number').value;
    var date_issued = document.getElementById('date_issued').value;
    var due_date = document.getElementById('due_date').value;
    var amount = document.getElementById('amount').value;
    var tax = document.getElementById('tax').value;
    var duration = document.getElementById('duration').value;
    var hourly_rate = document.getElementById('hourly_rate').value;
    var description = document.getElementById('description').value;


    var parameters = {
        id: id,
        invoice_type: invoice_type,
        client_id: client_id,
        invoice_number: invoice_number,
        date_issued: date_issued,
        due_date: due_date,
        amount: amount,
        tax: tax,
        duration: duration,
        hourly_rate: hourly_rate,
        description: description
    };

    $.ajax({
        url: "ClientManagement/CreateInvoices",
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
                GetClient();
                $("#capture-record").modal("hide").data("bs.modal", null);
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
    $('#invoice_type').val("");
    $('#client_id').val("").trigger('change');
    $('#invoice_number').val("");
    $('#date_issued').val("");
    $('#due_date').val("");
    $('#amount').val("");
    $('#tax').val("");
    $('#duration').val("");
    $('#hourly_rate').val("");
    $('#description').val("");
});

$('#date_issued').datepicker({
    todayHighlight: true,
    startDate: '',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

$('#due_date').datepicker({
    todayHighlight: true,
    startDate: '',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

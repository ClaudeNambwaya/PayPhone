$(document).ready(function () {
    App.init();

    GetUser();
    
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
                        { "data": "matter_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "matter_number", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "client", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "start_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "close_date", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "matter_billing", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "user_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "practice_area", "autoWidth": true, "sDefaultContent": "n/a" },
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
                    $('.modal-body #matter_name').val(json["matter_name"]);
                    $('.modal-body #matter_number').val(json["matter_number"]);
                    $('.modal-body #user_id').val(json["user_id"]).trigger("change");
                    $('.modal-body #client_id').val(json["client_id"]).trigger("change");
                    $('.modal-body #start_date').val(json["start_date"]);
                    $('.modal-body #close_date').val(json["close_date"]);
                    $('.modal-body #matter_status').val(json["matter_status"]);
                    $('.modal-body #matter_billing').val(json["matter_billing"]);
                    $('.modal-body #description').val(json["description"]);
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

    GetMatter();

    GetClient();

    GetGetUser();

    

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


function GetMatter() {
    $.get('GetRecords', { module: 'matters' }, function (data) {
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



function GetUser() {
    $.get('GetRecords', { module: 'portal_users' }, function (data) {
        $("#user_id").get(0).options.length = 0;
        $("#user_id").get(0).options[0] = new Option("Please Select Assigned", "-1");

        $.each(data, function (index, item) {
            $("#user_id").get(0).options[$("#user_id").get(0).options.length] = new Option(item.name, item.id);
        });

        $("#user_id").bind("change", function () {
            //console.log($(this).val() + ' ' + $("#user option:selected").text());
            //GetUnallocatedRoles($(this).val());
            //GetAllocatedRoles($(this).val());
        });
    });
}





//function getData(jsonstring) {
//    table = $('#editabledatatable').dataTable();
//    oSettings = table.fnSettings();
//    table.fnClearTable(this);

//    var json = $.parseJSON(JSON.stringify(jsonstring));
//    //var json = JSON.parse(jsonstring);
//    for (var i = 0; i < json.length; i++) {
//        var item = json[i];
//        table.oApi._fnAddData(oSettings, item);
//    }
//    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
//    table.fnDraw();
//}


function getData(table, jsonstring) {
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
    var matter_name = document.getElementById('matter_name').value;
    var matter_number = document.getElementById('matter_number').value;
    var user_id = document.getElementById('user_id').value;
    var client_id = document.getElementById('client_id').value;
    var start_date = document.getElementById('start_date').value;
    var close_date = document.getElementById('close_date').value;
    var matter_status = document.getElementById('matter_status').value;
    var matter_billing = document.getElementById('matter_billing').value;
    var description = document.getElementById('description').value;


    var parameters = {
        id: id,
        matter_name: matter_name,
        matter_number: matter_number,
        user_id: user_id,
        client_id: client_id,
        start_date: start_date,
        close_date: close_date,
        matter_status: matter_status,
        matter_billing: matter_billing,
        description: description
    };

    $.ajax({
        url: "ClientManagement/CreateMatters",
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
    $('#matter_name').val("");
    $('#matter_number').val("");
    $('#user_id').val("").trigger('change');
    $('#client_id').val("").trigger('change');
    $('#start_date').val("");
    $('#close_date').val("");
    $('#matter_status').val("");
    $('#matter_billing').val("");
    $('#description').val("");
});


$('#start_date').datepicker({
    todayHighlight: true,
    startDate: '+12',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

$('#close_date').datepicker({
    todayHighlight: true,
    startDate: '+12',
    format: 'yyyy-mm-dd',
    changeMonth: true,
    changeYear: true,
    autoclose: true,
    todayBtn: 'linked'
});

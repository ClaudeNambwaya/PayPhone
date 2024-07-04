$(document).ready(function () {
    App.init();
    //TableManageResponsive.init();

    GetUnapprovedClients();

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
                        { "data": "client_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "phone_number", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "email", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "physical_address", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "client_type", "autoWidth": true, "sDefaultContent": "n/a" },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs approve'><i class='fa fa-check'></i> Approve</a>"
                        }
                    ]
                });

                //Delete an Existing Row
                $('#editabledatatable').on("click", 'a.approve', function (e) {
                    e.preventDefault();
                    var a = $(this).closest(".panel");

                    var nRow = $(this).parents('tr')[0];

                    var parameters = {
                        id: $(this).parents('tr').attr("recid"),
                        module: 'client_record'
                    };

                    Swal.fire({
                        title: "Are you sure?",
                        text: "You want to approve this record",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Proceed!",
                        reverseButtons: true
                    }).then((result) => {
                        if (result.isConfirmed) {
                            //$.blockUI();

                            $.ajax({
                                url: "Approve/ClientManagement",
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

                                    if (data.substring(0, 7) == "Success") {
                                        //oTable.fnDeleteRow(nRow);
                                        Swal.fire({
                                            title: "Approved",
                                            text: "Record has been approved",
                                            icon: "success",
                                            confirmButtonText: "Ok"
                                        });
                                    } else {
                                        Swal.fire({
                                            title: "Error",
                                            text: data,
                                            icon: "error",
                                            confirmButtonText: "Ok"
                                        });
                                    }
                                    GetUnapprovedClients();
                                },
                                error: function (xhr, textStatus, errorThrown) {
                                    //$.unblockUI();
                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                                    Swal.fire({
                                        title: "Failed",
                                        text: "Record could not be approved " + errorThrown,
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

    GetUnapprovedClients();

    InitiateEditableDataTable.init();

    
});

function GetUnapprovedClients() {
    $.get('GetRecords', { module: 'client_record', param: 'unapproved' }, function (data) {
        getData(data);
    });
}

function getData(jsonstring) {
    table = $('#editabledatatable').dataTable();
    oSettings = table.fnSettings();
    table.fnClearTable(this);

    var json = $.parseJSON(JSON.stringify(jsonstring));
    var arr = [];
    for (var i = 0; i < json.length; i++) {
        var item = json[i];
        table.oApi._fnAddData(oSettings, item);
    }
    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
    table.fnDraw();
}
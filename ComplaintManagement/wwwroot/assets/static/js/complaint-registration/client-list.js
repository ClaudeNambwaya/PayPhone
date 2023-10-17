$(document).ready(function () {
    App.init();
    //TableManageResponsive.init();

    GetClient();

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

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #client_type').val(json["client_type"]);
                    $('.modal-body #client_name').val(json["client_name"]);
                    $('.modal-body #phone_number').val(json["phone_number"]);
                    $('.modal-body #email').val(json["email"]);
                    $('.modal-body #id_number').val(json["id_number"]);
                    $('.modal-body #kra_pin').val(json["kra_pin"]);
                    $('.modal-body #physical_address').val(json["physical_address"]);
                    $('.modal-body #postal_address').val(json["postal_address"]);
                    $('.modal-body #industry').val(json["industry"]);
                    $('.modal-body #remarks').val(json["remarks"]);
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
                            var parameters = { module: 'client_record', id: rec };
                            $.ajax({
                                url: "/ClientManagement/Delete",
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


function GetClient() {
    $.get('GetRecords', { module: 'client_list' }, function (data) {
        table = $('#editabledatatable').dataTable();
        getData(table, data);
    });
}




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
    var client_type = document.getElementById('client_type').value;
    var client_name = document.getElementById('client_name').value;
    var phone_number = document.getElementById('phone_number').value;
    var email = document.getElementById('email').value;
    var id_number = document.getElementById('id_number').value;
    var kra_pin = document.getElementById('kra_pin').value;
    var physical_address = document.getElementById('physical_address').value;
    var postal_address = document.getElementById('postal_address').value;
    var industry = document.getElementById('industry').value;
    var remarks = document.getElementById('remarks').value;
    var kra_pin = document.getElementById('kra_pin').value;


    var parameters = {
        id: id,
        client_type: client_type,
        client_name: client_name,
        phone_number: phone_number,
        email: email,
        id_number: id_number,
        kra_pin: kra_pin,
        physical_address: physical_address,
        postal_address: postal_address,
        industry: industry,
        remarks: remarks,
        kra_pin: kra_pin,
    };

    $.ajax({
        url: "Data/CreateClient",
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
    $('#client_type').val("").trigger('change');
    $('#client_name').val("");
    $('#phone_number').val("");
    $('#email').val("");
    $('#id_number').val("");
    $('#kra_pin').val("");
    $('#physical_address').val("");
    $('#postal_address').val("");
    $('#industry').val("");
    $('#remarks').val("");
});







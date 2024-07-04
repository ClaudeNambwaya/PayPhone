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
                        { "data": "sub_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "category", "autoWidth": true, "sDefaultContent": "n/a" },

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
                    $('.modal-body #sub_name').val(json["sub_name"]);
                    $('.modal-body #category_id').val(json["category_id"]);
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
                            var parameters = { module: 'subcategory_record', id: rec };
                            $.ajax({
                                url: "/ComplaintManagement/Delete",
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
                                    GetSubcategory();
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

    GetSubcategory();
    GetCategory();


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


function GetSubcategory() {
    $.get('GetRecords', { module: 'subcategory_record' }, function (data) {
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

function GetCategory() {
    $.get('GetRecords', { module: 'category_record' }, function (data) {
        $("#category_id").get(0).options.length = 0;
        $("#category_id").get(0).options[0] = new Option("Please Select Category", "-1");

        $.each(data, function (index, item) {
            $("#category_id").get(0).options[$("#category_id").get(0).options.length] = new Option(item.category_name, item.id);
        });

        $("#category_id").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var sub_name = document.getElementById('sub_name').value;
    var category_id = document.getElementById('category_id').value;


    var parameters = {
        id: id,
        sub_name: sub_name,
        category_id: category_id

    };
    console.log(parameters);

    $.ajax({
        url: "/ComplaintManagement/CreateSubcategory",
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
                GetSubcategory();

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
    $('#sub_name').val("");
    $('#category_id').val("").trigger('change');



});


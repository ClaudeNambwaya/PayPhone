
//var selected_sub_category = "-1";
//var selected_sub_county = "-1";
//var selected_ward = "-1";

$(document).ready(function () {
    App.init();
   
    //TableManageResponsive.init();

    var InitiateEditableDataTable = function () {
        return {
            init: function () {
                //Datatable Initiating
                var oTable = $('#complaintsdatatable').dataTable({
                    "responsive": true,
                    "createdRow": function (row, data, dataIndex) {
                        $(row).attr("recid", data.id);
                    },

                   
                    "columnDefs": [
                        {
                            "targets": 6,
                            "render": function (data, type, row, meta) {
                               
                                if (row.state_id === 0) {
                                    return '<a href="#" class="btn btn-warning btn-xs flagclosed">Open</a>';
                                } else {
                                    return '<a href="#" class="btn btn-success btn-xs flagunOpen"> Closed</a>';
                                }
                            }
                        }
                    ],

                    "aoColumns": [
                        { "data": "category", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "subcategory", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "complaint", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "nature_of_complaint", "autoWidth": true, "sDefaultContent": "n/a" },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs view'><i class='fas fa-eye'></i> View</a>"
                        },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs edit'><i ></i> Take Action</a>"
                        },
                        {
                            "data": "state_id",
                            "autoWidth": true,
                            "bSearchable": false,
                            "bSortable": false,
                            "sDefaultContent": "n/a"
                        }
                        
                        
                    ]
                });

                var isView = null;

                //View
                $('#complaintsdatatable').on("click", 'a.view', function (e) {
                    e.preventDefault();

                    nRow = $(this).parents('tr')[0];

                    //console.log($(this).parents('tr').attr("recid"));

                    //console.log(nRow);

                    if (isView !== null && isView != nRow) {
                        //restoreRow(oTable, isEditing);
                        viewRow(oTable, nRow);
                        isView = nRow;
                    } else {
                        viewRow(oTable, nRow);
                        isView = nRow;
                    }
                });

                function viewRow(oTable, nRow) {
                    var aData = oTable.fnGetData(nRow);
                    var jqTds = $('>td', nRow);

                    var json = JSON.parse(JSON.stringify(aData));
                    console.log(json);

                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #category_id').val(json["category"]);
                    $('.modal-body #subcategory_id').val(json["subcategory"]);
                    $('.modal-body #complaint_type').val(json["complaint"]);
                    $('.modal-body #nature_of_complaint').val(json["nature_of_complaint"]);
                    $('.modal-body #county_id').val(json["county"]);
                    $('.modal-body #sub_county_id').val(json["subcounty"]);
                    $('.modal-body #ward_id').val(json["ward"]);
                    $('.modal-body #isanonymous').val(json["complainant_name"]);
                    $('.modal-body #complaint_description').val(json["complaint_description"]);
                    $('.modal-body #address').val(json["address"]);
                    $('.modal-body #state_id').val(json["state_id"]);
                    $('.modal-body #remarks').val(json["remarks"]);


                    var rec = json["id"];
                    console.log(rec);

                    GetComplaintDocuments(rec);
                    //GetProfilePic(rec);

                    $("#view-record").appendTo("body").modal("show");
                }


              
                $('#complaintsdatatable').on("click", 'a.flagclosed', function (e) {
                    e.preventDefault();

                    nRow = $(this).parents('tr')[0];

                    var aData = oTable.fnGetData(nRow);

                    var json = JSON.parse(JSON.stringify(aData));
                    console.log(json);

                    var state_id = json["id"];

                    console.log(state_id);

                    //ajax call to update debit_credit_note table - paid = 1

                Swal.fire({
                    title: "Are you sure?",
                    text: "You want to Change Complaint Status ?",
                    icon: "question",
                    showCancelButton: true,
                    confirmButtonText: "YES!",
                    reverseButtons: true
                }).then((result) => {
                    if (result.isConfirmed) {
                        //$.blockUI();

                        oTable.fnDeleteRow(nRow);
                        //Ajax to flag as deleted
                        var parameters = { module: 'open_close_status', id: state_id };
                        $.ajax({
                            url: "/ManageComplaint/UpdateStatus",
                            type: "POST",
                            data: parameters,
                            success: function (data) {
                                Swal.fire({
                                    title: "Confirmed",
                                    text: "Complaint has been Closed",
                                    icon: "success",
                                    confirmButtonText: "Ok"
                                });

                                GetComplaintRegistration();
                            },
                            error: function (xhr, textStatus, errorThrown) {
                                //$.unblockUI();

                                Swal.fire({
                                    title: "Failed",
                                    text: "Status could not be updated " + errorThrown,
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

                var isEditing = null;

                //Edit
                $('#complaintsdatatable').on("click", 'a.edit', function (e) {
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


                    $('.modal-body #recordid').val($(nRow).attr("recid"));
                    $('.modal-body #remarks').val(json["remarks"]);

                    $("#capture-record").appendTo("body").modal("show");
                }

                //Delete an Existing Row
                $('#complaintsdatatable').on("click", 'a.delete', function (e) {
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
                            var parameters = { module: 'client', id: rec };
                            $.ajax({
                                url: "/ManageComplaint/Delete",
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
                                    GetComplaintRegistration();
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

    InitiateEditableDataTable.init();

    var InitiateComplaintsDataTable = function () {
        return {
            init: function () {
                //Datatable Initiating
                var oTable = $('#complaintdocumentssdatatable').dataTable({
                    "responsive": true,
                    "createdRow": function (row, data, dataIndex) {
                        $(row).attr("recid", data.id);
                    },

                    "aoColumns": [
                        { "data": "file_name", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "file_number", "autoWidth": true, "sDefaultContent": "n/a" },

                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-primary btn-xs download'> Download </a>"
                        }


                    ]
                });

                var isDownloading = null;

                //
                //downloads
                $('#complaintdocumentssdatatable').on("click", 'a.download', function (e) {
                    e.preventDefault();

                    nRow = $(this).parents('tr')[0];

                    //console.log($(this).parents('tr').attr("recid"));

                    //console.log(nRow);

                    if (isDownloading !== null && isDownloading != nRow) {
                        //restoreRow(oTable, isEditing);
                        downloadFile(oTable, nRow);
                        isDownloading = nRow;
                    } else {
                        downloadFile(oTable, nRow);
                        isDownloading = nRow;
                    }
                });


                function downloadFile(oTable, nRow) {
                    var aData = oTable.fnGetData(nRow);
                    var jqTds = $('>td', nRow);

                    var json = JSON.parse(JSON.stringify(aData));

                    

                    var file = json["file_number"];
                    console.log(file);
                    window.open("/Uploads/" + file, '_blank'); 
                    
                }




                //$('#complaintdocumentssdatatable').on("click", 'a.download', function (e) {
                //    e.preventDefault();

                //    //nRow = $(this).parents('tr')[0];
                //    var nRow = $(this).parents('tr')[0];
                //    var rec = $(this).parents('tr').attr("recid");

                //    var parameters = { id: rec };

                //    console.log(parameters);

                //    $.ajax({
                //        url: "/ManageComplaint/DownloadPDF",
                //        type: "GET",
                //        data: parameters,

                //        success: function (data) {

                //            var result = data.file;
                //            console.log(result);
                //            //window.location = "/uploads/1/" + data.file ;
                //            window.open("/assets/client_documents/" + data.file, '_blank');
                //        }
                //    });

                //});


            }
        };
    }();

    InitiateComplaintsDataTable.init();





    var FormPlugins = function () {
        "use strict";
        return {
            init: function () {
                handleSelectpicker();
            }
        }
    }();

    FormPlugins.init();

    GetComplaintRegistration();

    GetCategory();
    GetCounty();
    GetComplaint_Type();
    
});

handleSelectpicker = function () {
    $(".selectpicker").select2()
}

function GetComplaintRegistration() {
    $.get('GetRecords', { module: 'open_complaint_status' }, function (data) {
        table = $('#complaintsdatatable').dataTable();
        getData(table, data);
    });
}

function GetComplaintDocuments(complaint_id) {
    $.get('GetRecords', { module: 'complaint_files', param: complaint_id }, function (data) {
        table = $('#complaintdocumentssdatatable').dataTable();
        getcomplaintData(table, data);
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

function getcomplaintData(table, jsonstring) {
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
function GetCategory() {
    // Use a variable to store the select element
    var categorySelect = $("#category_id");

    $.get('GetRecords', { module: 'category_record' }, function (data) {
        // Clear the options in the select element
        categorySelect.empty();

        // Add the default "Please Select Category" option
        categorySelect.append(new Option("Please Select Category", "-1"));

        $.each(data, function (index, item) {
            // Add options for each category in the data
            categorySelect.append(new Option(item.category_name, item.id));
        });

        // Bind the change event to the select element
        categorySelect.on("change", function () {
            console.log("category_id:" + $(this).val());
            GetSubcategory($(this).val());
        });
    });
}



    function GetSubcategory(category_id) {
        $.get('GetRecords', { module: 'subcategorybyid', param: category_id }, function (data) {
            console.log(category_id);

            // Clear the select options
            $("#subcategory_id").empty();

            // Add a default option
            $("#subcategory_id").append(new Option("Please Select Sub-Category", "-1"));
            console.log('here');
            // Add subcategory options
            $.each(data, function (index, item) {
                $("#subcategory_id").append(new Option(item.sub_name, item.id));
            });

            // Bind change event
            $("#subcategory_id").on("change", function () {
                console.log("subcategory_id:" + $(this).val());

                // Consider whether you want to make a recursive call here
                // GetSubcategory($(this).val());
            });

            //if (selected_sub_category != -1)
            //    $("#subcategory_id").val(selected_sub_category).trigger("change");

        });
    }


function GetComplaint_Type() {
    // Use a variable to store the select element
    var complaint_type_Select = $("#complaint_type");

    $.get('GetRecords', { module: 'complaint_type_record' }, function (data) {
        // Clear the options in the select element
        complaint_type_Select.empty();

        // Add the default "Please Select Category" option
        complaint_type_Select.append(new Option("Please Select Complaint Type", "-1"));

        $.each(data, function (index, item) {
            // Add options for each category in the data
            complaint_type_Select.append(new Option(item.complaint_name, item.id));
        });

        // Bind the change event to the select element
        complaint_type_Select.on("change", function () {
        });
    });
}


function GetCounty() {
    // Use a variable to store the select element
    var countySelect = $("#county_id");

    $.get('GetRecords', { module: 'county_record' }, function (data) {
        // Clear the options in the select element
        countySelect.empty();

        // Add the default "Please Select Category" option
        countySelect.append(new Option("Please Select County", "-1"));

        $.each(data, function (index, item) {
            // Add options for each category in the data
            countySelect.append(new Option(item.county_name, item.id));
        });

        // Bind the change event to the select element
        countySelect.on("change", function () {
            GetSubcategory($(this).val());
        });
    });
}


function GetSubCounty(county_id) {
    $.get('GetRecords', { module: 'subcountybyid', param: county_id }, function (data) {
        console.log(data);

        // Clear the select options
        $("#sub_county_id").empty();

        // Add a default option
        $("#sub_county_id").append(new Option("Please Select County", "-1"));

        // Add subcategory options
        $.each(data, function (index, item) {
            $("#sub_county_id").append(new Option(item.subcounty_name, item.id));
        });

        // Bind change event
        $("#sub_county_id").on("change", function () {
            console.log("sub_county_id:" + $(this).val());

            // Consider whether you want to make a recursive call here
            GetWard($(this).val());
        });
    });
}


function GetWard(sub_county_id) {

    var a = $(this).closest(".panel");

    console.log(sub_county_id);


    var parameters = { module: 'wardbyid', param: sub_county_id };

    $.ajax({
        url: "/ManageComplaint/GetRecords",
        type: "GET",
        contentType: 'application/json',
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
            console.log(data);
            $("#ward_id").get(0).options.length = 0;
            $("#ward_id").get(0).options[0] = new Option("Please Select ward_id", "-1");

            $.each(data, function (index, item) {
                $("#ward_id").get(0).options[$("#ward_id").get(0).options.length] = new Option(item.ward_name, item.id);
            });

            $("#ward_id").bind("change", function () {
               // GetWard($(this).val());
            });

            if (selected_ward != -1)
                $("#ward_name").val(selected_ward).trigger("change");
        },
        error: function (xhr, textStatus, errorThrown) {
            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

            Swal.fire({
                title: "Failed",
                text: "Product could not be assigned" + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
}

$('#save_action').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var description = document.getElementById('description').value;
    console.log(description);

    var parameters = {
        id: id,
        description: description
    };
    console.log(parameters);

    $.ajax({
        url: "/ManageComplaint/MakeRemarks",
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

            if (data == "Success") {
                GetComplaintRegistration();
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


$('#save').click(function () {
    var a = $(this).closest(".panel");

    var id = document.getElementById('recordid').value;
    var category_id = document.getElementById('category_id').value;
    var subcategory_id = document.getElementById('subcategory_id').value;
    var complaint_type = document.getElementById('complaint_type').value;
    var nature_of_complaint = document.getElementById('nature_of_complaint').value;
    var complaint_description = document.getElementById('complaint_description').value;
    var county_id = document.getElementById('county_id').value;
    var sub_county_id = document.getElementById('sub_county_id').value;
    var ward_id = document.getElementById('ward_id').value;
    var address = document.getElementById('address').value;
    var isanonymous = document.getElementById('isanonymous').value;
    var state_id = document.getElementById('state_id').value;
    var remarks = document.getElementById('remarks').value;

    var parameters = {
        id: id,
        category_id: category_id,
        subcategory_id: subcategory_id,
        complaint_type: complaint_type,
        nature_of_complaint: nature_of_complaint,
        complaint_description: complaint_description,
        county_id: county_id,
        sub_county_id: sub_county_id,
        ward_id: ward_id,
        address: address,
        isanonymous: isanonymous,
        state_id: state_id,
        remarks : remarks
    };


    $.ajax({
        url: "/ManageComplaint/UpdateComplaint",
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

            if (data == "Success") {
                GetComplaintRegistration();
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
    $('#remarks').val("").trigger('change');
});

























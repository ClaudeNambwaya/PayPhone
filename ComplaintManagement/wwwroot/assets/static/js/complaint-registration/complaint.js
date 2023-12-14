
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

                    //"columnDefs": [
                    //    {
                    //        "targets": 6,
                    //        "render": function (data, type, row, meta) {
                    //            return "<div class='input-group-btn input-group-btn-xs'>" +
                    //                "   <button type='button' class='btn btn-primary btn-xs'>Action</button> " +
                    //                "   <button type='button' class='btn btn-primary btn-xs dropdown-toggle' data-toggle='dropdown'> " +
                    //                "       <span class='caret'></span> " +
                    //                "   </button> " +
                    //                "   <ul class='dropdown-menu'> " +
                    //                "       <li><a href='#' class='dropdown-item view'><i class='fas fa-eye'></i> View</a></li> " +
                    //                "       <li class='divider'></li> " +
                    //                "       <li><a href='#' class='dropdown-item edit'><i class='fa fa-edit'></i> Edit</a></li> " +
                    //                "       <li><a href='#' class='dropdown-item delete'><i class='fa fa-trash'></i> Delete</a></li> " +
                    //                "   </ul> " +
                    //                "</div>";
                    //        }
                    //    }
                    //],

                    "aoColumns": [
                        { "data": "category", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "subcategory", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "complaint", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "nature_of_complaint", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "state_id", "autoWidth": true, "sDefaultContent": "n/a" },
                        { "data": "isanonymous", "autoWidth": true, "sDefaultContent": "n/a" },
                        {
                            "bSortable": false,
                            "sDefaultContent": "<a href='#' class='btn btn-info btn-xs view'><i class='fas fa-eye'></i> View</a>"
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
                    $('.modal-body #isanonymous').val(json["isanonymous"]);
                    $('.modal-body #state_id').val(json["state_id"]);
                    $('.modal-body #complaint_description').val(json["complaint_description"]);
                    $('.modal-body #address').val(json["address"]);
                    $('.modal-body #remarks').val(json["remarks"]);

                    //var rec = json["id"];
                    //GetDocuments(rec);
                    //GetProfilePic(rec);

                    $("#view-record").appendTo("body").modal("show");
                }

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
                    $('.modal-body #category_id').val(json["category_id"]).trigger("change");
                    //selected_sub_category = json["subcategory_id"];
                    $('.modal-body #subcategory_id').val(json["subcategory_id"]).trigger("change");
                    $('.modal-body #complaint_type').val(json["complaint_type"]).trigger("change");
                    $('.modal-body #nature_of_complaint').val(json["nature_of_complaint"]);
                    $('.modal-body #county_id').val(json["county_id"]).trigger("change");
                    $('.modal-body #sub_county_id').val(json["sub_county_id"]).trigger("change");
                    $('.modal-body #ward_id').val(json["ward_id"]).trigger("change");
                    $('.modal-body #isanonymous').val(json["isanonymous"]);
                    $('.modal-body #complaint_description').val(json["complaint_description"]);
                    $('.modal-body #address').val(json["address"]);

                    

                    //var category = document.getElementById('category_id').value;
                    //console.log(category);
                    ////selected_sub_county = json["sub_county_id"];
                    ////selected_ward = json["ward_id"];
                    //GetSubcategory(category);

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

    InitiateEditableDataTable.init();

  
        // Add an event listener for the checkbox change event
        $('#isanonymous').on('change', function () {
            // Check if the checkbox is checked
            if ($(this).is(':checked')) {
                // If checked, set the value of isanonymous to true
                $('#isanonymous').val(true);
            } else {
                // If not checked, set the value of isanonymous to false
                $('#isanonymous').val(false);
            }
        });

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

var applicants = [];
handleSelectpicker = function () {
    $(".selectpicker").select2()
}

function GetComplaintRegistration() {
    $.get('GetRecords', { module: 'complaint_record_byId' }, function (data) {
        table = $('#complaintsdatatable').dataTable();
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

function GetCategory() {
    $.get('GetRecords', { module: 'category_record' }, function (data) {

        $("#category_id").get(0).options.length = 0;
        $("#category_id").get(0).options[0] = new Option("Please Select Category", "-1");

        $.each(data, function (index, item) {
            $("#category_id").get(0).options[$("#category_id").get(0).options.length] = new Option(item.category_name, item.id);
        });

        $("#category_id").bind("change", function () {
            console.log("category_id:" + $(this).val());

            GetSubcategory($(this).val());
        });
    });
}


/*$(document).ready(function () {*/
//    function GetSubcategory(category_id) {
//    $.get('GetRecords', { module: 'subcategorybyid', param: category_id }, function (data) {
//        console.log(data);
//        $("#subcategory_id").get(0).options.length = 0;
//        $("#subcategory_id").get(0).options[0] = new Option("Please Select Category", "-1");

//        $.each(data, function (index, item) {
//            $("#subcategory_id").get(0).options[$("#subcategory_id").get(0).options.length] = new Option(item.sub_name, item.id);
//        });

//        $("#subcategory_id").bind("change", function () {
//            console.log("subcategory_id:" + $(this).val());

//           // GetSubcategory($(this).val());
//        });
//    });
//}


/*$(document).ready(function () {*/
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

    // Call the function initially with a default category_id if needed
    // GetSubcategory(initialCategoryID);
/*});*/





//function GetSubcategory(category_id) {

//    var a = $(this).closest(".panel");

//   console.log(category_id);


//    var parameters = { module: 'subcategorybyid', param: category_id };

//    $.ajax({
//        url: "/ComplaintRegistration/GetRecords",
//        type: "GET",
//        contentType: 'application/json',
//        data: parameters,
//        beforeSend: function () {
//            if (!$(a).hasClass("panel-loading")) {
//                var t = $(a).find(".panel-body"),
//                    i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

//                $(a).addClass("panel-loading"), $(t).prepend(i);
//            }
//        },
//        success: function (data) {
//            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
//            console.log(data);
//            $("#subcategory_id").get(0).options.length = 0;
//            $("#subcategory_id").get(0).options[0] = new Option("Please Select Subcategory", "-1");

//            $.each(data, function (index, item) {
//                $("#subcategory_id").get(0).options[$("#subcategory_id").get(0).options.length] = new Option(item.sub_name, item.id);
//            });

//            $("#subcategory_id").bind("change", function () {
//                //GetSectors($(this).val());
//            });

//            if (selected_category != -1)
//                $("#subcategory_id").val(selected_category).trigger("change");
//        },
//        error: function (xhr, textStatus, errorThrown) {
//            $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

//            Swal.fire({
//                title: "Failed",
//                text: "Product could not be assigned" + errorThrown,
//                icon: "error",
//                confirmButtonText: "Ok"
//            });
//        }
//    });
//}

function GetComplaint_Type() {
    $.get('GetRecords', { module: 'complaint_type_record' }, function (data) {
        $("#complaint_type").get(0).options.length = 0;
        $("#complaint_type").get(0).options[0] = new Option("Please Select Complaint Type", "-1");

        $.each(data, function (index, item) {
            $("#complaint_type").get(0).options[$("#complaint_type").get(0).options.length] = new Option(item.complaint_name, item.id);
        });

        $("#complaint_type").bind("change", function () {
            /*GetTopics($(this).val());*/
            //console.log($(this).val() + ' ' + $("#program option:selected").text());
        });
    });
}

function GetCounty() {
    $.get('GetRecords', { module: 'county_record' }, function (data) {

        $("#county_id").get(0).options.length = 0;
        $("#county_id").get(0).options[0] = new Option("Please Select County", "-1");

        $.each(data, function (index, item) {
            $("#county_id").get(0).options[$("#county_id").get(0).options.length] = new Option(item.county_name, item.id);
        });

        $("#county_id").bind("change", function () {
            console.log("county_id:" + $(this).val());

            GetSubCounty($(this).val());
        });
    });
}
//function GetSubCounty(county_id) {
//    $.get('GetRecords', { module: 'subcountybyid', param: county_id }, function (data) {

//        $("#sub_county_id").get(0).options.length = 0;
//        $("#sub_county_id").get(0).options[0] = new Option("Please Select County", "-1");

//        $.each(data, function (index, item) {
//            $("#sub_county_id").get(0).options[$("#sub_county_id").get(0).options.length] = new Option(item.subcounty_name, item.id);
//        });

//        $("#sub_county_id").bind("change", function () {
//            console.log("sub_county_id:" + $(this).val());

//            //GetSubCounty($(this).val());
//        });
//    });
//}


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
        url: "/ComplaintRegistration/GetRecords",
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
            $("#ward_id").get(0).options[0] = new Option("Please Select Ward", "-1");

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

    var cnt = applicants.length;

    var applicant = {
        id: cnt + 1,
        category_id: category_id,
        subcategory_id: subcategory_id,
        complaint_type: complaint_type,
        nature_of_complaint: nature_of_complaint,
        complaint_description: complaint_description,
        county_id: county_id,
        sub_county_id: sub_county_id,
        ward_id: ward_id,
        address: address,
        isanonymous: isanonymous
    };

    applicants.push(applicant);

    const container = document.getElementById('uploadedFiles');
    const complainant_files = container.textContent.trim();

    var parameters = {
        applicant_details: applicants,
        complainant_files: complainant_files
    };
    console.log(parameters);

    $.ajax({
        url: "/ComplaintRegistration/CreateComplaint",
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
    $('#category_id').val("").trigger('change');
    $('#subcategory_id').val("").trigger('change');
    $('#complaint_type').val("").trigger('change');
    $('#nature_of_complaint').val("");
    $('#complaint_description').val("");
    $('#county_id').val("");
    $('#sub_county_id').val("");
    $('#ward_id').val("");
    $('#address').val("");
    $('#isanonymous').val("");
});

























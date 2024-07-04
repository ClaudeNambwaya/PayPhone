/*
Template Name: Color Admin - Responsive Admin Dashboard Template build with Twitter Bootstrap 3.3.7 & Bootstrap 4.0.0-Alpha 6
Version: 3.0.0
Author: Sean Ngu
Website: http://www.seantheme.com/color-admin-v3.0/admin/html/
*/

function GetMyProfileDetails() {
    $.get('GetMyProfileData', function (data) {
        document.getElementById("username").innerHTML = data[0].username;
        document.getElementById("phonenumber").innerHTML = data[0].phonenumber;
        document.getElementById("emailaddress").innerHTML = data[0].emailaddress;
        document.getElementById("profile").innerHTML = data[0].profile;
        document.getElementById("createdon").innerHTML = data[0].createdon;
        document.getElementById("menutoggler").innerHTML = data[0].menulayout;
        document.getElementById("profilepic").src = "/assets/static/img/profile-pics/" + data[0].profilepic;
        document.getElementById("rightprofilepic").src = "/assets/static/img/profile-pics/" + data[0].profilepic;
    });
}

var StartRead = function () {
    var file = document.getElementById('img-file-input').files[0];
    //console.log(file);
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = addImg;
};

function addImg(imgsrc) {
    var img = document.getElementById('profilepic');

    //upload to server
    var parameters = {
        name: 'profilepic',
        value: imgsrc.target.result
    };

    $.ajax({
        url: "UpdateMyProfileData/UserProfile",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(parameters),
        dataType: "json",
        success: function (data) {
            //console.log(data);
            if (data === 'Success') {
                GetMyProfileDetails();
            } else {
                Swal.fire({
                    title: "Failed",
                    text: data,
                    type: "error",
                    confirmButtonText: "Ok"
                });
            }
        }
    });
}

var handleAjaxConsoleLog = function (e, t) {
    var n = [], r;
    n.push(e.type.toUpperCase() + ' url = "' + e.url + '"');
    for (var i in e.data) {
        if (e.data[i] && typeof e.data[i] === "object") {
            r = [];
            for (var s in e.data[i]) {
                r.push(s + ': "' + e.data[i][s] + '"');
            }
            r = "{ " + r.join(", ") + " }";
        }
        else {
            r = '"' + e.data[i] + '"';
        }
        n.push(i + " = " + r);
    }
    n.push("RESPONSE: status = " + t.status);
    if (t.responseText) {
        if ($.isArray(t.responseText)) {
            n.push("[");
            $.each(t.responseText, function (e, t) {
                n.push("{value: " + t.value + ', text: "' + t.text + '"}');
            });
            n.push("]");
        }
        else {
            n.push($.trim(t.responseText));
        }
    }
    n.push("--------------------------------------\n");
    //console.log(n);
};

var handleEditableFormAjaxCall = function () {
    $.mockjaxSettings.responseTime = 500;
    $.mockjax({
        url: "/UserProfile/UpdateMyProfileData",
        response: function (e) {
            //handleAjaxConsoleLog(e, this);

            var parameters = {
                name: e.data.name,
                value: e.data.value
            };

            $.ajax({
                url: "UpdateMyProfileData/UserProfile",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(parameters),
                dataType: "json",
                success: function (data) {
                    if (data === 'Success') {
                        GetMyProfileDetails();
                    } else {
                        Swal.fire({
                            title: "Failed",
                            text: data,
                            type: "error",
                            confirmButtonText: "Ok"
                        });
                    }
                }
            });
        }
    });
};

var handleEditableFieldConstruct = function () {
    var e = window.location.href.match(/c=inline/i) ? "inline" : "popup";
    if (e === "inline") {
        $("[data-editable]").removeClass("active");
        $('[data-editable="inline"]').addClass("active");
    }
    $.fn.editable.defaults.mode = e === "popup";
    $.fn.editable.defaults.inputclass = "form-control input-sm";
    $.fn.editable.defaults.url = "/UserProfile/UpdateMyProfileData";
    //$("#phonenumber").editable();
    $("#phonenumber").editable({
        validate: function (e) {
            if ($.trim(e) === "") {
                return "This field is required";
            }
        }
    });
    $("#emailaddress").editable({
        validate: function (e) {
            if ($.trim(e) === "") {
                return "This field is required";
            }
        }
    });
    $("#password").editable({
        validate: function (e) {
            if ($.trim(e) === "") {
                return "This field is required";
            }
        }
    });
};

$('#menutoggler').click(function () {

    var a = $(this).closest(".panel");

    var parameters = {
        name: 'menu_layout',
        value: ''
    };

    $.ajax({
        url: "UpdateMyProfileData/UserProfile",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(parameters),
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

            if (data === 'Success') {
                GetMyProfileDetails();
                window.location = '/UserProfile/Index';
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
                text: "Operation has not completed " + errorThrown,
                icon: "error",
                confirmButtonText: "Ok"
            });
        }
    });
});

var FormEditable = function () {
    "use strict";
    return {
        init: function () {
            GetMyProfileDetails();
            handleEditableFieldConstruct();
            handleEditableFormAjaxCall();
        }
    };
}();
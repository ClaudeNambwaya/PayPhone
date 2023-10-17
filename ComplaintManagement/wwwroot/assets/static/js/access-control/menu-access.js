$(document).ready(function () {
    App.init();

    var FormPlugins = function () {
        "use strict";
        return {
            init: function () {
                handleSelectpicker();
            }
        }
    }();

    FormPlugins.init();

    GetProfiles();
});

handleSelectpicker = function () {
    $(".selectpicker").select2()
}

function GetProfiles() {
    $.get('GetRecords', { module: 'roles' }, function (data) {
        $("#profile").get(0).options.length = 0;
        $("#profile").get(0).options[0] = new Option("Please Select Role", "-1");

        $.each(data, function (index, item) {
            $("#profile").get(0).options[$("#profile").get(0).options.length] = new Option(item.role_name, item.id);
        });

        $("#profile").bind("change", function () {
            GetMenus($(this).val());
        });
    });
}

function GetMenus(profileid) {
    //using jstree
    $('#tree-menu').jstree("destroy");

    $('#tree-menu').jstree({
        'plugins': ["contextmenu", "ui", "types", "wholerow", "checkbox"],
        'core': {
            'data': {
                "url": "/AccessControl/GetMenusJSTree?profileid=" + profileid,
                "dataType": "json", // needed only if you do not supply JSON headers
                //"success": function (n) {
                //    console.log(n);
                //}
            }
        },
        'types': {
            "default": { 'icon': "fa fa-folder text-success fa-lg" },
            "file": { 'icon': "fa fa-file text-success fa-lg" }
        },
        'progressive_render': true,
        'progressive_unload': false
    });
}

$('#save').click(function () {
    var a = $(this).closest(".panel");

    var profile = document.getElementById('profile').value;

    Swal.fire({
        title: "Are you sure?",
        text: "you want to save this menu?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Proceed!",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {

            //using jstree
            var treeData = $('#tree-menu').jstree(true).get_json('#', { flat: false })
            // set flat:true to get all nodes in 1-level json
            var jsonData = $.parseJSON(JSON.stringify(treeData));

            //console.log(jsonData);

            var menu = [];
            for (var i = 0; i < jsonData.length; i++) {
                var item = jsonData[i];

                var children = [];
                var can_access = false;
                if (item["children"].length > 0) {
                    for (var j = 0; j < item["children"].length; j++) {
                        var childitem = item["children"][j];

                        var childobj = {
                            sub_menu_text: childitem["text"],
                            sub_menu_url: childitem["a_attr"]["href"],
                            can_access: childitem["state"]["selected"]
                        }

                        children.push(childobj);
                    }
                } else {
                    can_access = item["state"]["selected"];
                }

                var obj = {
                    main_menu_name: item["text"],
                    main_menu_url: item["a_attr"]["href"],
                    can_access: can_access,
                    sub_menus: children
                }

                menu.push(obj);
            }

            var parameters = { profileid: profile, menu: menu };
            console.log(parameters);
            console.log(JSON.stringify(parameters));
            $.ajax({
                url: "/AccessControl/CaptureMenuAccess",
                type: "POST",
                contentType: 'application/json',
                data: JSON.stringify(parameters),
                
                dataType: 'json',

                
                beforeSend: function () {
                    if (!$(a).hasClass("panel-loading")) {
                        var t = $(a).find(".panel-body"),
                            i = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                        $(a).addClass("panel-loading"), $(t).prepend(i);
                    }
                },
               
                success: function (data) {
                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                    Swal.fire({
                        title: "Saved",
                        text: "Menu has been saved",
                        icon: "success",
                        confirmButtonText: "Ok"
                    });
                },
                error: function (xhr, textStatus, errorThrown) {
                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();

                    Swal.fire({
                        title: "Failed",
                        text: "Menu could not be saved " + errorThrown,
                        icon: "error",
                        confirmButtonText: "Ok"
                    });
                }
            });

        } else {
            Swal.fire({
                title: "Cancelled",
                text: "Action has been cancelled",
                icon: "info",
                confirmButtonText: "Ok"
            });
        }
    });
});
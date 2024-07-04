var handleFormPasswordIndicator = function () {
    "use strict";
    $("#newpassword").passwordStrength({
        targetDiv: "#passwordStrengthDiv"
    });
},
    FormPlugins = function () {
        "use strict";
        return {
            init: function () {
                handleFormPasswordIndicator();
            }
        };
    }();

// toggle password visibility
$('#password + .glyphicon').on('click', function () {
    $(this).toggleClass('glyphicon-eye-close').toggleClass('glyphicon-eye-open'); // toggle our classes for the eye icon
    $('#password').togglePassword(); // activate the hideShowPassword plugin
});

$('#newpassword + .glyphicon').on('click', function () {
    $(this).toggleClass('glyphicon-eye-close').toggleClass('glyphicon-eye-open'); // toggle our classes for the eye icon
    $('#newpassword').togglePassword(); // activate the hideShowPassword plugin
});

$('#confirmpassword + .glyphicon').on('click', function () {
    $(this).toggleClass('glyphicon-eye-close').toggleClass('glyphicon-eye-open'); // toggle our classes for the eye icon
    $('#confirmpassword').togglePassword(); // activate the hideShowPassword plugin
});
var handleBootstrapWizards = function () {
    "use strict";
    $("#wizard").bwizard({
        validating: function (e, ui) {
            //$("#form_actions").hide();
            if (ui.index == 0) {
                // step-1 confirmation
                var source_str = $("#source_account option:selected").text();
                var source_acc = source_str.substring(source_str.lastIndexOf("[") + 1, source_str.lastIndexOf("]"));
                document.getElementById("confirm_source_account").value = source_acc.trim();

                var dest_str = $("#dest_account option:selected").text();
                var dest_acc = dest_str.substring(dest_str.lastIndexOf("[") + 1, dest_str.lastIndexOf("]"));
                document.getElementById("confirm_dest_account").value = dest_acc.trim();

                var amount = $("#amount").val();
                document.getElementById("confirm_amount").value = amount;

                var reference = $("#reference").val();
                document.getElementById("confirm_reference").value = reference;

                var comments = $("#comments").val();
                document.getElementById("confirm_comments").value = comments;
                
            } else if ((ui.index == 1) && (ui.nextIndex > ui.index)) {
                // step-2 summary
                //invoke ajax request here
                
            }
        }
    });
};

var FormWizard = function () {
    "use strict";
    return {
        init: function () {
            handleBootstrapWizards();
        }
    }
}()
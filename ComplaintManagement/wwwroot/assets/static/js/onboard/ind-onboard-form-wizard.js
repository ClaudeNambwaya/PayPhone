
$(document).ready(function () { 
    App.init();

    PatientFormWizard.init();

    $('.selectpicker').select2({
        style: 'btn-white',
        size: 5
    });

    //$("#client_type").bind("change", function () {

    //    var x = document.getElementById("ind_div");
    //    var xx = document.getElementById("confirm_ind_div");
    //    var y = document.getElementById("comp_div");
    //    var yy = document.getElementById("confirm_comp_div");

    //    var str = $("#client_type option:selected").val();

    //    if (str == 'IND') {
    //        x.style.display = "block";
    //        xx.style.display = "block";
    //        y.style.display = "none";
    //        yy.style.display = "none";
    //    }
    //    else {
    //        x.style.display = "none";
    //        xx.style.display = "none";
    //        y.style.display = "block";
    //        yy.style.display = "block";

    //    }
    //});

});


var applicants = [];


var handlePatientWizards = function () {
    "use strict";
    $("#wizard").bwizard({
        validating: function (e, ui) {
            if (ui.index === 0) {
                //Validate Controls
                // step-1 client Details
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-1")) {
                    return false;
                } else {

                    /***** Applicant details confirmation *****/
                    document.getElementById('confirm_client_type').value = $("#client_type option:selected").text();
                    document.getElementById('confirm_client_name').value = document.getElementById('client_name').value;
                    document.getElementById('confirm_phone_number').value = document.getElementById('phone_number').value;
                    document.getElementById('confirm_email').value = document.getElementById('email').value;
                    document.getElementById('confirm_id_number').value = document.getElementById('id_number').value;
                    document.getElementById('confirm_kra_pin').value = document.getElementById('kra_pin').value;
                    document.getElementById('confirm_physical_address').value = document.getElementById('physical_address').value;
                    document.getElementById('confirm_postal_address').value = document.getElementById('postal_address').value;
                    document.getElementById('confirm_industry').value = document.getElementById('industry').value;
                    document.getElementById('confirm_remarks').value = document.getElementById('remarks').value;

                    /***** Applicant details confirmation *****/

                }
            } else if ((ui.index === 1) && (ui.nextIndex > ui.index)) {
                // step-2 Address Details
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-2")) {
                    return false;
                } else {

                }
            } else if ((ui.index === 2) && (ui.nextIndex > ui.index)) {
                // step-3 Uploads
                if (false === $('form[name="form-wizard"]').parsley().validate("wizard-step-3")) {
                    return false;
                } else {

                    var a = $(this).closest(".panel");

                    Swal.fire({
                        title: "Are you sure?",
                        text: "you want to proceed with onboarding?",
                        icon: "question",
                        showCancelButton: true,
                        confirmButtonText: "Proceed",
                        reverseButtons: true
                    }).then((result) => {
                        if (result.isConfirmed) {
                            var client_type = $("#client_type option:selected").text();
                            var client_name = document.getElementById('client_name').value;
                            var phone_number = document.getElementById('phone_number').value;
                            var email = document.getElementById('email').value;
                            var id_number = document.getElementById('id_number').value;
                            var kra_pin = document.getElementById('kra_pin').value;
                            var physical_address = document.getElementById('physical_address').value;
                            var postal_address = document.getElementById('postal_address').value;
                            var industry = document.getElementById('industry').value;
                            var remarks = document.getElementById('remarks').value;

                           
                            var cnt = applicants.length;

                            var applicant =
                            {
                                id: cnt + 1,
                                client_type: client_type,
                                client_name: client_name,
                                phone_number: phone_number,
                                email: email,
                                id_number: id_number,
                                kra_pin: kra_pin,
                                physical_address: physical_address,
                                postal_address: postal_address,
                                industry: industry,
                                remarks: remarks 
                            };

                            console.log(applicant);

                            //if (document.getElementById('applicantrecordid').value > 0) {
                            //    const index = applicants.findIndex(item => item.id === document.getElementById('applicantrecordid').value);
                            //    applicants.splice(index, 1);
                            //}

                            applicants.push(applicant);

                            const container = document.getElementById('uploadedFiles');
                            const client_files = container.textContent.trim();

                            /*console.log(client_files);*/


                            

                            var parameters = {
                                applicant_details: applicants,
                                client_files: client_files
                            };
                            console.log(parameters);
                            $.ajax({
                                url: "/ClientManagement/OnboardClient",
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
                                    document.getElementById("summary_system_reference").innerHTML = data.system_ref;

                                    var buttons = document.getElementsByClassName("previous");

                                    for (var i = 0; i < buttons.length; i++) {
                                        buttons[i].setAttribute("aria-disabled", "true");
                                        buttons[i].setAttribute("class", "previous disabled");
                                    }

                                    if (data.code === '00') {
                                        document.getElementById("summary_status").innerHTML = "Success";
                                        document.getElementById("summary_status").classList = "label label-success";
                                        Swal.fire({
                                            title: "Success",
                                            text: data.desc,
                                            icon: "success",
                                            confirmButtonText: "Ok"
                                        });
                                    } else {
                                        document.getElementById("summary_status").innerHTML = "Failed";
                                        document.getElementById("summary_status").classList = "label label-danger";
                                        Swal.fire({
                                            title: "Failed",
                                            text: data.desc,
                                            icon: "error",
                                            confirmButtonText: "Ok"
                                        });
                                    }

                                    $(a).removeClass("panel-loading"), $(a).find(".panel-loader").remove();
                                }
                            });
                        } else {
                            document.getElementById("summary_system_reference").innerHTML = "-";
                            document.getElementById("summary_status").innerHTML = "Cancelled";
                            document.getElementById("summary_status").classList = "label label-info";

                            Swal.fire({
                                title: "Cancelled",
                                text: "Registration has been cancelled",
                                icon: "info",
                                confirmButtonText: "Ok"
                            });
                        }
                    });
                }
            }
        }
    });
};

var PatientFormWizard = function () {
    "use strict";
    return {
        init: function () {
            handlePatientWizards();
        }
    };
}();
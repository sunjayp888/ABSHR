(function (jQuery) {

    'use strict';
    var workingDaysValue = true;
    //function displayWorkingDays() {
    //    if (jQuery("#workingDays").html.length === 0) {
    //       jQuery("#employmentError")
    //            .addClass("row alert alert-error")
    //            .html("<ul><li>Unable to find the default working pattern.</li></ul>")
    //            .show();
    //        jQuery("#employmentError")
    //            .parent()
    //            .closest(".row")
    //            .removeClass("hidden");
    //    }
    //    else {
    //        if (jQuery('#ModelStateValue').val()) {
    //            jQuery("#employmentError")
    //                .hide()
    //                .parent()
    //                .closest(".row")
    //                .addClass("hidden");
    //        }
    //    }
    //}
    function displayWorkingDays() {
 
        if (!workingDaysValue) {
        jQuery("#submit").prop("disabled", true);
            jQuery("#employmentError")
            .addClass("row alert alert-error")
           .html("<ul><li>Unable to find the default working pattern.</li></ul>")
            .show();
            jQuery("#employmentError")
                .parent()
                .closest(".row")
                .removeClass("hidden");
        }
        else {
            var absencePolicyPeriodsCount = jQuery('#AbsencePolicyPeriodsCount').val();
            if (absencePolicyPeriodsCount!=0) {
                           
                jQuery("#submit").prop("disabled", false);
                jQuery('#ModelStateValue').val = true;
                jQuery("#employmentError")
                    .hide()
                    .parent()
                    .closest(".row")
                    .addClass("hidden");
            } else {
                           
                jQuery("#submit").prop("disabled", true);
                jQuery("#employmentError")
                .addClass("row alert alert-error")
               .html("<ul><li>Unable to find the default Absence Periods.</li></ul>")
                .show();
                jQuery("#employmentError")
                    .parent()
                    .closest(".row")
                    .removeClass("hidden");

            }
            
        }
    }
    function loadWorkingDays() {

        if (!jQuery("#EmploymentViewModel_Employment_AbsencePolicyId").val() == '') {
            var absencePolicyId = jQuery("#EmploymentViewModel_Employment_AbsencePolicyId").val();
            jQuery.ajax({
                url: '/Employment/GetWorkingPatternRecord?absencePolicyId=' + absencePolicyId + '&htmlFieldPrefix=EmploymentViewModel.WorkingPatternDays',
                type: 'GET',
                data: "",
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data == "" || data == null) {
                        jQuery("#workingDays").empty();
                        workingDaysValue = false;
                        displayWorkingDays();
                    } else {
                        jQuery("#workingDays").html(data);
                        workingDaysValue = true;
                        displayWorkingDays();
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    jQuery("#workingDays").empty();
                    displayWorkingDays();
                }
            });
        }
        else {
            jQuery("#workingDays").empty();
            displayWorkingDays();
        }
    }

    jQuery(function () {

        jQuery(".begin .date, .dob .date").daterangepicker({
            autoApply: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            opens: 'left',
            locale: {
                format: 'DD MMMM YYYY'
            }
        });

        $(".end .date, .termination .date, .previousemploymentend .date").daterangepicker({
            autoApply: true,
            singleDatePicker: true,
            autoUpdateInput: false,
            showCustomRangeLabel: false,
            showDropdowns: true,
            opens: 'left',
            locale: {
                format: 'DD MMMM YYYY'
            }
        });
        $(".end .date, .termination .date, .previousemploymentend .date").on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('DD MMMM YYYY'));
        });

        jQuery("#EmploymentViewModel_Employment_AbsencePolicyId").change(function () {
            loadWorkingDays();
        });

        if (jQuery("#EmploymentViewModel_Employment_EmploymentId").val() === 0) {
            loadWorkingDays();
        };
    });
})(window.jQuery);
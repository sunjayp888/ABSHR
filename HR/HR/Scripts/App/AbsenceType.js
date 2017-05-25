(function (jQuery) {


    jQuery(function () {
        jQuery('#absencePolicyEntitlementErrorSummary').hide();
        var hasEntitlement = jQuery("#AbsencePolicyEntitlement_HasEntitlement");

        disableEntitlement(!hasEntitlement.prop("checked"));

        hasEntitlement.on("click", function () {
            disableEntitlement(!jQuery(this).prop("checked"));
        });

    });

    function disableEntitlement(disabled) {
        var frequencyId = jQuery("#AbsencePolicyEntitlement_FrequencyId"),
            entitlement = jQuery("#AbsencePolicyEntitlement_Entitlement"),
            maximumCarryForward = jQuery("#AbsencePolicyEntitlement_MaximumCarryForward");
 
        frequencyId.prop("disabled", disabled);
        entitlement.prop("disabled", disabled);
        maximumCarryForward.prop("disabled", disabled);
        if (!disabled) {
            frequencyId.removeProp("disabled");
            entitlement.removeProp("disabled");
            maximumCarryForward.removeProp("disabled");
        } else {
            jQuery("#AbsencePolicyEntitlement_Entitlement").val('0');
            jQuery("#AbsencePolicyEntitlement_MaximumCarryForward").val('0');
        }
    }

    jQuery("#AbsencePolicyEntitlement_FrequencyId").on("change", function () {
        jQuery("#FrequencyId").val($(this).val());
    });

})(window.jQuery);
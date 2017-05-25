(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsencePolicyAbsenceTypeController', AbsencePolicyAbsenceTypeController);

    AbsencePolicyAbsenceTypeController.$inject = ['$window', '$filter', 'AbsencePolicyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function AbsencePolicyAbsenceTypeController($window, $filter, AbsencePolicyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */

        var vm = this;
        vm.initialise = initialise;
        vm.absencePolicyAbsenceTypes = [];
        vm.absenceTypeEntitlements = [];
        vm.ddAbsenceType = 0;
        vm.absenceTypeCount = 0;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.assigning = false;
        vm.changeAbsenceType = changeAbsenceType;
        vm.retrieveUnassignedAbsencePolicyAbsenceTypes = retrieveUnassignedAbsencePolicyAbsenceTypes;
        vm.retrieveAbsencePolicyEntitlements = retrieveAbsencePolicyEntitlements;
        vm.unassignAbsencePolicyAbsenceType = unassignAbsencePolicyAbsenceType;
        vm.isAbsencesAssignedToAbsencePolicyAbsenceType = isAbsencesAssignedToAbsencePolicyAbsenceType;
        vm.unassignAbsencePolicyAbsenceTypeClass = unassignAbsencePolicyAbsenceTypeClass;
        vm.assignAbsencePolicyAbsenceType = assignAbsencePolicyAbsenceType;
        vm.editAbsencePolicyEntitlement = editAbsencePolicyEntitlement;
        vm.openAbsencePolicyEntitlementForm = openAbsencePolicyEntitlementForm;
        vm.updateAbsencePolicyEntitlement = updateAbsencePolicyEntitlement;


        function initialise(absencePolicyId) {
            vm.absencePolicyId = absencePolicyId;
            order("Name").then(function () {

                retrieveAbsencePolicyEntitlements();
            });
        }

        function retrieveAbsencePolicyEntitlements() {
            return AbsencePolicyService.retrieveAbsencePolicyEntitlements(vm.absencePolicyId)
               .then(function (response) {
                   vm.absenceTypeError = false;
                   vm.absenceTypeEntitlements = response.data.Items;
                   if (vm.absenceTypeEntitlements.length > 0) {
                       vm.absenceTypeCount = vm.absenceTypeEntitlements.length;
                      
                   } else {
                       vm.absenceTypeError = true;
                       vm.absenceTypeCount = 0;
                     

                   }
                   vm.paging.totalPages = response.data.TotalPages;
                   vm.paging.totalResults = response.data.TotalResults;
               });
        }

        function retrieveUnassignedAbsencePolicyAbsenceTypes() {
            return AbsencePolicyService.retrieveUnassignedAbsencePolicyAbsenceTypes(vm.absencePolicyId)
               .then(function (response) {
                   vm.ddAbsenceTypes = response.data;
                   vm.ddAbsenceType = response.data[0];
                   vm.assigning = vm.ddAbsenceTypes.length == 0;
                   return vm.ddAbsenceTypes;
               });
        }

        function editAbsencePolicyEntitlement(absencePolicyEntitlementId) {
            return AbsencePolicyService.editAbsencePolicyEntitlement(vm.absencePolicyId, absencePolicyEntitlementId)
               .then(function (response) {
                   jQuery("#absencePolicyEntitlementModalBody").html(response.data);
                   $('#absencePolicyEntitlementErrorSummary').hide();
                   $("#absencePolicyEntitlementModal").modal('show');
               });
        }

        function openAbsencePolicyEntitlementForm(absencePolicyEntitlementId, absenceType) {
            vm.absenceType = absenceType;
            editAbsencePolicyEntitlement(absencePolicyEntitlementId);
        }

        function unassignAbsencePolicyAbsenceTypeClass(absenceTypeEntitlement) {
            return absenceTypeEntitlement.CanUnassign ? '' : 'link-disabled';
        }

        function unassignAbsencePolicyAbsenceType(absencePolicyEntitlement) {
            if (absencePolicyEntitlement.CanUnassign) {
                return AbsencePolicyService.unassignAbsencePolicyAbsenceType(vm.absencePolicyId, absencePolicyEntitlement.AbsenceTypeId)
                    .then(function () {
                        initialise(vm.absencePolicyId);
                    });
            }
        }

        function isAbsencesAssignedToAbsencePolicyAbsenceType(absenceTypeId) {
            vm.loadingActions = true;
            vm.absenceTypeId = absenceTypeId;
            AbsencePolicyService.isAbsencesAssignedToAbsencePolicyAbsenceType(vm.absencePolicyId, absenceTypeId).then(function (response) {
                $filter('filter')(vm.absenceTypeEntitlements, { AbsenceTypeId: vm.absenceTypeId })[0]["CanUnassign"] = !response.data;
                vm.loadingActions = false;
            });
        }

        function assignAbsencePolicyAbsenceType() {
            vm.assigning = true;
            return AbsencePolicyService.assignAbsencePolicyAbsenceType(vm.absencePolicyId, vm.ddAbsenceType.AbsenceTypeId)
              .then(function () {
                  retrieveAbsencePolicyEntitlements();
                  retrieveUnassignedAbsencePolicyAbsenceTypes();
              });
        }

        function changeAbsenceType(ddAbsenceType) {
            vm.ddAbsenceType = ddAbsenceType;
        }

        function pageChanged() {
            return retrieveUnassignedAbsencePolicyAbsenceTypes();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveUnassignedAbsencePolicyAbsenceTypes();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function updateAbsencePolicyEntitlement() {
            vm.absencePolicyId = $('#AbsencePolicyEntitlement_AbsencePolicyId').val();
            var absencePolicyEntitlement =
                {
                    AbsencePolicyEntitlementId: $('#AbsencePolicyEntitlement_AbsencePolicyEntitlementId').val(),
                    AbsenceTypeId: $('#AbsencePolicyEntitlement_AbsenceTypeId').val(),
                    FrequencyId: $('#AbsencePolicyEntitlement_FrequencyId').val(),
                    Entitlement: $('#AbsencePolicyEntitlement_Entitlement').val(),
                    MaximumCarryForward: $('#AbsencePolicyEntitlement_MaximumCarryForward').val(),
                    IsUnplanned: $('#AbsencePolicyEntitlement_IsUnplanned').is(":checked"),
                    IsPaid: $('#AbsencePolicyEntitlement_IsPaid').is(":checked"),
                    AbsencePolicyId: $('#AbsencePolicyEntitlement_AbsencePolicyId').val(),
                    HasEntitlement: $('#AbsencePolicyEntitlement_HasEntitlement').is(":checked"),
                    OrganisationId: $('#AbsencePolicyEntitlement_OrganisationId').val()
                }


            return AbsencePolicyService.updateAbsencePolicyEntitlement(absencePolicyEntitlement).then(function (response) {
                if (response.data === "") {
                    $("#absencePolicyEntitlementModal").modal('hide');
                    initialise(vm.absencePolicyId);
                } else {
                    $('#absencePolicyEntitlementErrorSummary').show();
                    vm.Errors = response.data;
                }
            });
        }
    }
})();

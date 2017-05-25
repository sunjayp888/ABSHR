(function () {
    'use strict';

    angular
        .module('HR')
        .controller('PersonnelAbsenceEntitlementController', PersonnelAbsenceEntitlementController);

    PersonnelAbsenceEntitlementController.$inject = ['$window', 'AbsencePolicyService', 'PersonnelAbsenceEntitlementService'];

    function PersonnelAbsenceEntitlementController($window, AbsencePolicyService, PersonnelAbsenceEntitlementService) {
        /* jshint validthis:true */
        var vm = this;
        vm.init = init;
        vm.personnelId = 0;
        vm.absencePeriod = 0;
        vm.absencePeriods = [];
        vm.personnelAbsenceEntitlements = [];
        vm.personnelAbsenceEntitlementLink = personnelAbsenceEntitlementLink;
        vm.changeAbsencePeriod = changeAbsencePeriod;


        function init(personnelId) {
            vm.personnelId = personnelId;
            retrieveAbsencePeriods().then(function () {
                retrievePersonnelAbsenceEntitlements();
            });            
        }

        function retrievePersonnelAbsenceEntitlements() {
            return PersonnelAbsenceEntitlementService.retrievePersonnelAbsenceEntitlements(vm.personnelId, vm.absencePeriod.AbsencePeriodId)
                .then(function (response) {
                    vm.personnelAbsenceEntitlements = response.data;
                    return vm.personnelAbsenceEntitlements;
                });
        }

        function retrieveAbsencePeriods() {
            return AbsencePolicyService.retrieveAbsencePeriodsByPersonnel(vm.personnelId)
                .then(function (response) {
                    vm.absencePeriods = response.data;
                    var today = moment();
                    for (var i = 0, l = response.data.length; i < l; i++) {
                        if (today.isBetween(response.data[i].StartDate, response.data[i].EndDate, 'days', '[]')) {
                            vm.absencePeriod = response.data[i].AbsencePeriod;
                            break;
                        }
                    }
                    return vm.absencePeriods;
                });
        }

        function personnelAbsenceEntitlementLink(action, personnelId, personnelAbsenceEntitlementId) {
            $window.location.href = "/PersonnelAbsenceEntitlement/" + action + "/" + personnelId + "/" + personnelAbsenceEntitlementId;
        }

        function changeAbsencePeriod(absencePeriod) {
            vm.absencePeriod = absencePeriod;
            retrievePersonnelAbsenceEntitlements();
        }

    }
})();

(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsenceController', AbsenceController);

    AbsenceController.$inject = ['$window', '$filter', 'AbsenceService','AbsencePolicyService'];

    function AbsenceController($window, $filter, AbsenceService, AbsencePolicyService) {
        /* jshint validthis:true */
        var vm = this;
        vm.init = init;
        vm.personnelId = 0;
        vm.absencePeriod = 0;
        vm.absences = [];
        vm.absencePeriods = [];
        vm.absenceLink = absenceLink;
        vm.canApproveAbsence = canApproveAbsence;
        vm.changeAbsencePeriod = changeAbsencePeriod;
        vm.cancelAbsence = cancelAbsence;
        vm.approveAbsence = approveAbsence;
        vm.declineAbsence = declineAbsence;

        function init(personnelId) {
            vm.personnelId = personnelId;
            retrieveAbsencePolicyAbsencePeriodsByPersonnel().then(function () {
                retrieveAbsences();
            });
        }

        function retrieveAbsencePeriods() {
            return AbsenceService.retrieveAbsencePeriods(vm.personnelId)
                .then(function (response) {
                    vm.absencePeriods = response.data;
                    var today = moment();
                    for (var i = 0, l = response.data.length; i < l; i++) {
                        if (today.isBetween(response.data[i].StartDate, response.data[i].EndDate, 'days', '[]')) {
                            vm.absencePeriod = response.data[i];
                            break;
                        }

                    }
                    return vm.absencePeriods;
                });
        }

        function retrieveAbsences() {
            return AbsenceService.retrieveAbsences(vm.personnelId, vm.absencePeriod.AbsencePeriodId)
                .then(function (response) {
                    vm.absences = response.data;
                    return vm.absences;
                });
        }

        function canApproveAbsence(absence) {
            return AbsenceService.canApproveAbsence(absence.PersonnelId, absence.AbsenceId)
                .then(function (response) {
                    absence.CanApproveAbsence = response.data;
                    return response.data;
                });
        }

        function changeAbsencePeriod(absencePeriod) {
            vm.absencePeriod = absencePeriod;
            retrieveAbsences();
        }

        function absenceLink(action, personnelId, absenceId) {
            $window.location.href = "/Absence/" + action + "/" + personnelId + "/" + absenceId;
        }

        function cancelAbsence(personnelId, absenceId) {
            return AbsenceService.cancelAbsence(personnelId, absenceId)
                .then(function (response) {
                    $window.location.reload(true);
                });
        }

        function approveAbsence(personnelId, absenceId) {
            return AbsenceService.approveAbsence(personnelId, absenceId)
                .then(function (response) {
                    $window.location.reload(true);
                });
        }

        function declineAbsence(personnelId, absenceId) {
            return AbsenceService.declineAbsence(personnelId, absenceId)
                .then(function (response) {
                    $window.location.reload(true);
                });
        }

        function retrieveAbsencePolicyAbsencePeriodsByPersonnel() {
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

    }
})();

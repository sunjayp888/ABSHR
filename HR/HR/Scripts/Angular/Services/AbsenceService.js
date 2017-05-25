(function () {
    'use strict';

    angular
        .module('HR')
        .factory('AbsenceService', AbsenceService);

    AbsenceService.$inject = ['$http'];

    function AbsenceService($http) {
        var service = {
            retrieveAbsences: retrieveAbsences,
            retrieveAbsencePeriods: retrieveAbsencePeriods,
            canApproveAbsence: canApproveAbsence,
            cancelAbsence: cancelAbsence,
            approveAbsence: approveAbsence,
            declineAbsence: declineAbsence
        };

        return service;

        function retrieveAbsences(personnelId, absencePeriodId) {

            var url = "/Absence/List",
                data = {
                    personnelId: personnelId,
                    absencePeriodId: absencePeriodId
                };

            return $http.post(url, data);
        }

        function retrieveAbsencePeriods(personnelId) {

            var url = "/Absence/Periods",
                data = {
                    personnelId: personnelId
                };

            return $http.post(url, data);
        }

        function canApproveAbsence(personnelId, id) {

            var url = "/Absence/CanApproveAbsence/" + personnelId + "/" + id;

            return $http.post(url);

        }

        function cancelAbsence(personnelId, absenceId) {

            var url = "/Absence/Cancel/" + personnelId + "/" + absenceId;

            return $http.post(url);
        }

        function approveAbsence(personnelId, absenceId) {

            var url = "/Absence/Approve/" + personnelId + "/" + absenceId;

            return $http.post(url);
        }

        function declineAbsence(personnelId, absenceId) {

            var url = "/Absence/Decline/" + personnelId + "/" + absenceId;

            return $http.post(url);
        }

    }
})();
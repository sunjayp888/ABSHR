(function () {
    'use strict';

    angular
        .module('HR')
        .factory('AbsenceForApprovalService', AbsenceForApprovalService);

    AbsenceForApprovalService.$inject = ['$http'];

    function AbsenceForApprovalService($http) {
        var service = {
            approveAbsence: approveAbsence,
            declineAbsence: declineAbsence,
            retrievAbsenceeForApprovals: retrievAbsenceeForApprovals
        };

        return service;

        function approveAbsence(absence) {

            var url = "/Absence/ApproveAbsence",
                data = {
                    absence: absence
                };

            return $http.post(url, data);
        }

        function declineAbsence(absence) {

            var url = "/Absence/DeclineAbsence",
                data = {
                    absence: absence
                };

            return $http.post(url, data);
        }

        function retrievAbsenceeForApprovals(Paging, OrderBy) {

            var url = "/Absence/ListAbsenceForApprovals",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

    }
})();
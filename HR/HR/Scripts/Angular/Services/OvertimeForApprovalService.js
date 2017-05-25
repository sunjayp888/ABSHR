(function () {
    'use strict';

    angular
        .module('HR')
        .factory('OvertimeForApprovalService', OvertimeForApprovalService);

    OvertimeForApprovalService.$inject = ['$http'];

    function OvertimeForApprovalService($http) {
        var service = {
            approveOvertime: approveOvertime,
            declineOvertime: declineOvertime,
            retrievOvertimeeForApprovals: retrievOvertimeeForApprovals
        };

        return service;

        function approveOvertime(overtime) {

            var url = "/Overtime/ApproveOvertime",
                data = {
                    overtime: overtime
                };

            return $http.post(url, data);
        }

        function declineOvertime(overtime) {

            var url = "/Overtime/DeclineOvertime",
                data = {
                    overtime: overtime
                };

            return $http.post(url, data);
        }

        function retrievOvertimeeForApprovals(Paging, OrderBy) {

            var url = "/Overtime/ListOvertimeForApprovals",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

    }
})();
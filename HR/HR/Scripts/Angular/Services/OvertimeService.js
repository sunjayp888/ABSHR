(function () {
    'use strict';

    angular
        .module('HR')
        .factory('OvertimeService', OvertimeService);

    OvertimeService.$inject = ['$http'];

    function OvertimeService($http) {
        var service = {
            retrieveOvertimes: retrieveOvertimes,
            approveOvertime: approveOvertime,
            canApproveOvertime: canApproveOvertime,
            declineOvertime: declineOvertime,
            deleteOvertime: deleteOvertime
        };

        return service;

        function retrieveOvertimes(PersonnelId, Paging, OrderBy) {

            var url = "/Overtime/List/" + PersonnelId,
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function approveOvertime(PersonnelId, Id) {
            var url = "/Overtime/Approve/" + PersonnelId + "/" + Id
            return $http.post(url);
        }

        function canApproveOvertime(PersonnelId, Id) {
            var url = "/Overtime/CanApproveOvertime/" + PersonnelId + "/" + Id
            return $http.post(url);
        }

        function declineOvertime(PersonnelId, Id) {
            var url = "/Overtime/Decline/" + PersonnelId + "/" + Id;
            return $http.post(url);
        }

        function deleteOvertime(id) {
            var url = "/Overtime/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
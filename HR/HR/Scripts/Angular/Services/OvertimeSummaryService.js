(function () {
    'use strict';

    angular
        .module('HR')
        .factory('OvertimeSummaryService', OvertimeSummaryService);

    OvertimeSummaryService.$inject = ['$http'];

    function OvertimeSummaryService($http) {
        var service = {
            CreateOvertimeAdjustment: CreateOvertimeAdjustment,
            retrieveOvertimeSummaries: retrieveOvertimeSummaries
        };

        return service;

        function CreateOvertimeAdjustment(overtimeSummary, comment) {
            var url = "/Overtime/CreateOvertimeAdjustment",
              data = {
                  overtimeSummary: overtimeSummary,
                  comment: comment
              };
            return $http.post(url, data);
        }

        function retrieveOvertimeSummaries(companyIds, departmentIds, teamIds, Paging, OrderBy) {

            var url = "/Overtime/ListOvertimeSummaries",
                data = {
                    companyIds: companyIds,
                    departmentIds: departmentIds,
                    teamIds: teamIds,
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

    }
})();
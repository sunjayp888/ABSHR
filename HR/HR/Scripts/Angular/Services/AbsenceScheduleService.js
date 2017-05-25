(function () {
    'use strict';

    angular
        .module('HR')
        .factory('AbsenceScheduleService', AbsenceScheduleService);

    AbsenceScheduleService.$inject = ['$http'];

    function AbsenceScheduleService($http) {
        var service = {
            retrieveAbsenceSchedule: retrieveAbsenceSchedule
        };

        return service;

        function retrieveAbsenceSchedule(beginDate, personnelFilter, colourBy) {

            var url = "/Absence/Schedule",
                data = {
                    beginDate: beginDate,
                    personnelFilter: personnelFilter,
                    showColourBy: colourBy
                };

            return $http.post(url, data);

        }

    }
})();
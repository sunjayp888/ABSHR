(function () {
    'use strict';

    angular
        .module('HR')
        .factory('AbsencePeriodService', AbsencePeriodService);

    AbsencePeriodService.$inject = ['$http'];

    function AbsencePeriodService($http) {
        var service = {
            retrieveAbsencePeriods: retrieveAbsencePeriods,
            canDeleteAbsencePeriod: canDeleteAbsencePeriod,
            deleteAbsencePeriod: deleteAbsencePeriod
        };

        return service;

        function retrieveAbsencePeriods(Paging, OrderBy) {

            var url = "/AbsencePeriod/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

         function deleteAbsencePeriod(id) {
             var url = "/AbsencePeriod/Delete",
              data = {
                 id: id
              };
            return $http.post(url, data); 
        }

         function canDeleteAbsencePeriod(id) {
            var url = "/AbsencePeriod/CanDeleteAbsencePeriod",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }
    }
})();
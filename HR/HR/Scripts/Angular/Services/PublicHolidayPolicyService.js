(function () {
    'use strict';

    angular
        .module('HR')
        .factory('PublicHolidayPolicyService', PublicHolidayPolicyService);

    PublicHolidayPolicyService.$inject = ['$http'];

    function PublicHolidayPolicyService($http) {
        var service = {
            retrievePublicHolidayPolicies: retrievePublicHolidayPolicies,
            canDeletePublicHolidayPolicy: canDeletePublicHolidayPolicy,
            deletePublicHolidayPolicy: deletePublicHolidayPolicy,
            retrievePublicHolidays: retrievePublicHolidays,
            savePublicHoliday: savePublicHoliday,
            deletePublicHoliday: deletePublicHoliday,
            clonePublicHolidayPolicy: clonePublicHolidayPolicy
        };

        return service;

        function retrievePublicHolidayPolicies(Paging, OrderBy) {

            var url = "/PublicHolidayPolicy/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function clonePublicHolidayPolicy(publicHolidayPolicyId) {

            var url = "/PublicHolidayPolicy/ClonePublicHolidayPolicy",
                data = {
                    publicHolidayPolicyId: publicHolidayPolicyId
                };

            return $http.post(url, data);
        }

        function canDeletePublicHolidayPolicy(id) {
            var url = "/PublicHolidayPolicy/CanDeletePublicHolidayPolicy",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function deletePublicHolidayPolicy(id) {
            var url = "/PublicHolidayPolicy/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function retrievePublicHolidays(publicHolidayPolicyId, year, Paging, OrderBy) {
            var url = "/PublicHolidayPolicy/RetrievePublicHolidays",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy),
                    publicHolidayPolicyId: publicHolidayPolicyId,
                    year: year
                };
            return $http.post(url, data);
        }

        function savePublicHoliday(action, data) {
            var url = "/PublicHolidayPolicy/CreatePublicHoliday";
            if (action === 'Edit')
                url = "/PublicHolidayPolicy/UpdatePublicHoliday";
            return $http.post(url, data);
        }

        function deletePublicHoliday(publicHolidayId) {
            var url = "/PublicHolidayPolicy/DeletePublicHoliday",
              data = {
                  publicHolidayId: publicHolidayId
              };

            return $http.post(url, data);
        }
    }

})();
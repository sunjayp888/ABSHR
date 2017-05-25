(function () {
    'use strict';

    angular
        .module('HR')
        .factory('JobTitleService', JobTitleService);

    JobTitleService.$inject = ['$http'];

    function JobTitleService($http) {
        var service = {
            retrieveJobTitles: retrieveJobTitles,
            isJobTitleMappingExists: isJobTitleMappingExists,
            deleteJobTitle: deleteJobTitle
        };

        return service;

        function retrieveJobTitles(Paging, OrderBy) {

            var url = "/JobTitle/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteJobTitle(id) {
            var url = "/JobTitle/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isJobTitleMappingExists(id) {
            var url = "/JobTitle/CanDeleteJobTitle",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
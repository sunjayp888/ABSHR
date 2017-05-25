(function () {
    'use strict';

    angular
        .module('HR')
        .factory('SiteService', SiteService);

    SiteService.$inject = ['$http'];

    function SiteService($http) {
        var service = {
            retrieveSites: retrieveSites,
            isSiteMappingExists: isSiteMappingExists,
            deleteSite: deleteSite
        };

        return service;

        function retrieveSites(Paging, OrderBy) {

            var url = "/Site/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteSite(id) {
            var url = "/Site/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isSiteMappingExists(id) {
            var url = "/Site/CanDeleteSite",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }
    }
})();
(function () {
    'use strict';

    angular.module('HR').factory('BuildingService', BuildingService);

    BuildingService.$inject = ['$http'];

    function BuildingService($http) {
        var service = {
            retrieveBuildings: retreiveBuildings,
            isBuildingMappingExists: isBuildingMappingExists,
            deleteBuilding: deleteBuilding
         
        };
        return service;

        function retreiveBuildings(Paging, OrderBy) {
            var url = "/Building/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };
            return $http.post(url, data);
        }

        function deleteBuilding(id) {
            var url = "/Building/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isBuildingMappingExists(id) {
            var url = "/Building/CanDeleteBuilding",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }


    }
})();
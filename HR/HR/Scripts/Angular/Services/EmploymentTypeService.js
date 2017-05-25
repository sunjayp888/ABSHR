(function () {
    'use strict';

    angular
        .module('HR')
        .factory('EmploymentTypeService', EmploymentTypeService);

    EmploymentTypeService.$inject = ['$http'];

    function EmploymentTypeService($http) {
        var service = {
            retrieveEmploymentTypes: retrieveEmploymentTypes,
            isEmploymentTypeMappingExists: isEmploymentTypeMappingExists,
            deleteEmploymentType: deleteEmploymentType
        };

        return service;

        function retrieveEmploymentTypes(Paging, OrderBy) {

            var url = "/EmploymentType/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteEmploymentType(id) {
            var url = "/EmploymentType/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isEmploymentTypeMappingExists(id) {
            var url = "/EmploymentType/CanDeleteEmploymentType",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
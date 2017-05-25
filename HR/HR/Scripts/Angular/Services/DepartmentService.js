(function () {
    'use strict';

    angular
        .module('HR')
        .factory('DepartmentService', DepartmentService);

    DepartmentService.$inject = ['$http'];

    function DepartmentService($http) {
        var service = {
            retrieveListDepartmentFilters : retrieveListDepartmentFilters,
            retrieveDepartments: retrieveDepartments,
            isDepartmentMappingExists: isDepartmentMappingExists,
            deleteDepartment: deleteDepartment
        };

        return service;

        function retrieveListDepartmentFilters() {

            var url = "/Department/ListTeamFilters";

            return $http.post(url);
        }

        function retrieveDepartments(Paging, OrderBy) {

            var url = "/Department/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }


        function deleteDepartment(id) {
            var url = "/Department/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isDepartmentMappingExists(id) {
            var url = "/Department/CanDeleteDepartment",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }
    }
})();
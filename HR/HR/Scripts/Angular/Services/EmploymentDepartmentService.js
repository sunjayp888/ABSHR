(function () {
    'use strict';

    angular
        .module('HR')
        .factory('EmploymentDepartmentService', EmploymentDepartmentService);

    EmploymentDepartmentService.$inject = ['$http'];

    function EmploymentDepartmentService($http) {
        var service = {
            createEmploymentDepartment: createEmploymentDepartment,
            retrieveEmploymentDepartments: retrieveEmploymentDepartments,
            deleteEmploymentDepartment: deleteEmploymentDepartment
        };

        return service;

        function createEmploymentDepartment(employmentId, departmentId) {

            var url = "/EmploymentDepartment/Create",
                data = {
                    EmploymentId: employmentId,
                    DepartmentId: departmentId
                };

            return $http.post(url, data);
        }

        function retrieveEmploymentDepartments(employmentId) {

            var url = "/EmploymentDepartment/List",
                data = {
                    employmentId: employmentId
                };

            return $http.post(url, data);
        }

        function deleteEmploymentDepartment(employmentId, departmentId) {
            var url = "/EmploymentDepartment/Delete",
              data = {
                  employmentId: employmentId,
                  departmentId: departmentId
              };
            return $http.post(url, data);
        }

    }
})();
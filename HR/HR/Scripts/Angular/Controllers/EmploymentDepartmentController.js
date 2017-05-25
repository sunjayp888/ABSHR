(function () {
    'use strict';

    angular
        .module('HR')
        .controller('EmploymentDepartmentController', EmploymentDepartmentController);

    EmploymentDepartmentController.$inject = ['EmploymentDepartmentService', 'DepartmentService', '$filter'];

    function EmploymentDepartmentController(EmploymentDepartmentService, DepartmentService, $filter) {
        /* jshint validthis:true */
        var vm = this;
        vm.employmentId;
        vm.departments = [];
        vm.employmentDepartments = [];
        vm.initialise = initialise;
        vm.createEmploymentDepartment = createEmploymentDepartment;
        vm.deleteEmploymentDepartment = deleteEmploymentDepartment;

        function initialise(employmentId) {
            vm.employmentId = employmentId;
            retrieveDepartments();
        }

        function retrieveDepartments() {
            return DepartmentService.retrieveListDepartmentFilters()
                .then(function (response) {
                    vm.departments = response.data;
                    retrieveEmploymentDepartments();
                    return vm.departments;
                });
        }

        function retrieveEmploymentDepartments() {
            return EmploymentDepartmentService.retrieveEmploymentDepartments(vm.employmentId)
                .then(function (response) {
                    //vm.employmentDepartments = response.data.Items;
                    angular.forEach(response.data, function (item) {
                        var department = $filter('filter')(vm.departments, { DepartmentId: item.DepartmentId }, true)[0];
                        vm.employmentDepartments.push({ DepartmentId: department.DepartmentId, Name: department.Name, Hex: department.Hex });
                    });
                    return vm.employmentDepartments;
                });
        }

        function createEmploymentDepartment($item) {
            return EmploymentDepartmentService.createEmploymentDepartment(vm.employmentId, $item.DepartmentId)
              .then(function () {
              });
        }

        function deleteEmploymentDepartment($item) {
            return EmploymentDepartmentService.deleteEmploymentDepartment(vm.employmentId, $item.DepartmentId)
              .then(function () {
              });
        }

        
    }
})();

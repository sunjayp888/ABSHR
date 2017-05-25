(function () {
    'use strict';

    angular
        .module('HR')
        .controller('DepartmentController', DepartmentController);

    DepartmentController.$inject = ['$window', 'DepartmentService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function DepartmentController($window, DepartmentService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.departments = [];
        vm.selectedDepartments = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editDepartment = editDepartment;
        vm.isDepartmentMappingExists = isDepartmentMappingExists;
        vm.deleteDepartment = deleteDepartment;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveDepartments() {
            return DepartmentService.retrieveDepartments(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.departments = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.departments;
                });
        }

        function isDepartmentMappingExists(id) {
            vm.loadingActions = true;
            vm.IsDepartmentMapped = true;
            DepartmentService.isDepartmentMappingExists(id).then(function (response) { vm.IsDepartmentMapped = response.data, vm.loadingActions = false });
        }

        function deleteDepartment(id) {
            return DepartmentService.deleteDepartment(id)
              .then(function () {
                  initialise();
              });
        }


        function pageChanged() {
            return retrieveDepartments();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveDepartments();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editDepartment(id) {
            $window.location.href = "/Department/Edit/" + id;
        }
    }

})();

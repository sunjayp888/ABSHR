(function () {
    'use strict';

    angular
        .module('HR')
        .controller('EmploymentTypeController', EmploymentTypeController);

    EmploymentTypeController.$inject = ['$window', 'EmploymentTypeService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function EmploymentTypeController($window, EmploymentTypeService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.employmentTypes = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editEmploymentType = editEmploymentType;
        vm.isEmploymentTypeMappingExists = isEmploymentTypeMappingExists;
        vm.deleteEmploymentType = deleteEmploymentType;
        vm.initialise = initialise;

        function initialise() {
            order("Name");
        }

        function retrieveEmploymentTypes() {
            return EmploymentTypeService.retrieveEmploymentTypes(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.employmentTypes = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.employmentTypes;
                });
        }

        function isEmploymentTypeMappingExists(id) {
            vm.loadingActions = true;
            vm.IsEmploymentTypeMapped = true;
            EmploymentTypeService.isEmploymentTypeMappingExists(id).then(function (response) { vm.IsEmploymentTypeMapped = response.data, vm.loadingActions = false });
        }

        function deleteEmploymentType(id) {
            return EmploymentTypeService.deleteEmploymentType(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveEmploymentTypes();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveEmploymentTypes();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editEmploymentType(id) {
            $window.location.href = "/EmploymentType/Edit/" + id;            
        }
        
    }
})();

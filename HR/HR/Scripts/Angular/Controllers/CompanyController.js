(function () {
    'use strict';

    angular
        .module('HR')
        .controller('CompanyController', CompanyController);

    CompanyController.$inject = ['$window', 'CompanyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function CompanyController($window, CompanyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.company = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editCompany = editCompany;
        vm.isCompanyMappingExists = isCompanyMappingExists;
        vm.deleteCompany = deleteCompany;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveCompany() {
            return CompanyService.retrieveCompany(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.company = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.company;
                });
        }

        function isCompanyMappingExists(id) {
            vm.loadingActions = true;
            vm.IsCompanyMapped = true;
            CompanyService.isCompanyMappingExists(id).then(function(response) { vm.IsCompanyMapped = response.data, vm.loadingActions = false });
        }

        function deleteCompany(id) {
            return CompanyService.deleteCompany(id)
              .then(function () {
                initialise();
            });
        }

        function pageChanged() {
            return retrieveCompany();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveCompany();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editCompany(id) {
            $window.location.href = "/Company/Edit/" + id;
        }
    }

})();

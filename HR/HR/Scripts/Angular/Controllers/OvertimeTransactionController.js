(function () {
    'use strict';

    angular
        .module('HR')
        .controller('OvertimeTransactionController', OvertimeTransactionController);

    OvertimeTransactionController.$inject = ['$window', 'OvertimeTransactionService', 'Paging', 'OrderService', 'OrderBy', 'Order', 'OrganisationalChartService'];

    function OvertimeTransactionController($window, OvertimeTransactionService, Paging, OrderService, OrderBy, Order, OrganisationalChartService) {
        /* jshint validthis:true */
        var vm = this;

        vm.overtimeFilter = {
            Begin: null,
            End: null,
            CompanyIds: null,
            DepartmentIds: null,
            TeamIds: null,
            IsPaid: true,
            IsToil: true,
            IsRequested: true,
            IsDeclined: true,
            IsApproved: true,
            IsInApproval: true

        };

        vm.overtimeTransactions = [];
        vm.companies = [];
        vm.departments = [];
        vm.errors = [];
        vm.teams = [];

        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        
        vm.filterSelected = filterSelected;
        vm.orderClass = orderClass;
        vm.initialise = initialise;
        vm.retrieveOvertimeTransactions = retrieveOvertimeTransactions;

        function filterSelected() {
            vm.overtimeFilter.CompanyIds = $.map(vm.companies, function (value, index) { return value.CompanyId; });
            vm.overtimeFilter.DepartmentIds = $.map(vm.departments, function (value, index) { return value.DepartmentId; });
            vm.overtimeFilter.TeamIds = $.map(vm.teams, function (value, index) { return value.TeamId; });
        }

        function initialise() {
            vm.orderBy = OrderService.order(vm.orderBy, "OvertimeId");
            order("OvertimeId");
            retrieveOrganisationalChartViewModel();
        }

        function retrieveOrganisationalChartViewModel() {
            return OrganisationalChartService.retrieveOrganisationalChartViewModel()
                .then(function (response) {
                    vm.organisationalChartViewModel = response.data;
                });
        }

        function retrieveOvertimeTransactions() {
            return OvertimeTransactionService.retrieveOvertimeTransactions(vm.overtimeFilter, vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.overtimeTransactions = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.overtimeTransactions;
                });
        }

        function pageChanged() {
            return retrieveOvertimeTransactions();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveOvertimeTransactions();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }
        
    }
})();

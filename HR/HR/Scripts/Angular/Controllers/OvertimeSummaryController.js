(function () {
    'use strict';

    angular
        .module('HR')
        .controller('OvertimeSummaryController', OvertimeSummaryController);

    OvertimeSummaryController.$inject = ['$window', 'OvertimeSummaryService', 'Paging', 'OrderService', 'OrderBy', 'Order', 'OrganisationalChartService'];

    function OvertimeSummaryController($window, OvertimeSummaryService, Paging, OrderService, OrderBy, Order, OrganisationalChartService) {
        /* jshint validthis:true */
        var vm = this;

        vm.overtimeSummary;
        vm.comment;
        vm.companyIds;
        vm.departmentIds;
        vm.teamIds;
        vm.overtimeSummaries = [];
        vm.companies = [];
        vm.departments = [];
        vm.errors = [];
        vm.teams = [];

        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        
        vm.createOvertimeAdjustment = createOvertimeAdjustment;
        vm.overtimeAdjustmentModal = overtimeAdjustmentModal;
        vm.filterSelected = filterSelected;
        vm.orderClass = orderClass;
        vm.initialise = initialise;

        function createOvertimeAdjustment() {

            vm.Errors = [];
            if ((vm.overtimeSummary.PaidHours !== 0 || vm.overtimeSummary.TOILHours !== 0) && vm.comment !== undefined && vm.comment !== '') {
                return OvertimeSummaryService.CreateOvertimeAdjustment(vm.overtimeSummary, vm.comment)
                    .then(function (response) {
                        if (response.data.length === 0) {
                            retrieveOvertimeSummaries();
                        }
                        $("#overtimeSummaryModal").modal('hide');
                    });

            }
            else {
                if (vm.overtimeSummary.PaidHours === 0 && vm.overtimeSummary.TOILHours === 0) {
                    vm.Errors.push('Paid or Toil Hours should not be empty');
                }
                if (vm.comment === undefined || vm.comment === '') {
                    vm.Errors.push('Comment Needed');
                }
            }
        }


        function overtimeAdjustmentModal(overtimeSummary) {
            vm.overtimeSummary = angular.copy(overtimeSummary);
            vm.comment = "";
            vm.overtimeSummary.PaidHours = 0;
            vm.overtimeSummary.TOILHours = 0;
            $("#overtimeSummaryModal").modal('show');
        }

        function filterSelected() {
            vm.companyIds = $.map(vm.companies, function (value, index) { return value.CompanyId; });
            vm.departmentIds = $.map(vm.departments, function (value, index) { return value.DepartmentId; });
            vm.teamIds = $.map(vm.teams, function (value, index) { return value.TeamId; });
            return retrieveOvertimeSummaries();
        }

        function initialise() {
            vm.orderBy = OrderService.order(vm.orderBy, "Forenames");
            order("Surname");
            retrieveOrganisationalChartViewModel();
        }

        function retrieveOrganisationalChartViewModel() {
            return OrganisationalChartService.retrieveOrganisationalChartViewModel()
                .then(function (response) {
                    vm.organisationalChartViewModel = response.data;
                });
        }

        function retrieveOvertimeSummaries() {
            return OvertimeSummaryService.retrieveOvertimeSummaries(vm.companyIds, vm.departmentIds, vm.teamIds, vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.overtimeSummaries = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.overtimeSummaries;
                });
        }

        function pageChanged() {
            return retrieveOvertimeSummaries();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveOvertimeSummaries();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }
        
    }
})();

(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsencePolicyAbsencePeriodController', AbsencePolicyAbsencePeriodController);

    AbsencePolicyAbsencePeriodController.$inject = ['$window', '$filter', 'AbsencePolicyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function AbsencePolicyAbsencePeriodController($window, $filter, AbsencePolicyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.initialise = initialise;
        vm.absencePolicyId = 0;
        vm.absencePeriodCount = 0;
        vm.absencePeriods = [];
        vm.ddAbsencePeriod = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.absencePeriod = 0;
        vm.absencePolicyPeriodId = 0;
        vm.loadingActions = false;
        vm.createAbsencePolicyAbsencePeriod = createAbsencePolicyAbsencePeriod;
        vm.changeAbsencePeriod = changeAbsencePeriod;
        vm.unassignAbsencePeriod = unassignAbsencePeriod;
        vm.unassignAbsencePeriodClass = unassignAbsencePeriodClass;
        vm.isAbsencesAssignedToAbsencePolicyPeriod = isAbsencesAssignedToAbsencePolicyPeriod;

        vm.assigning = false;

        function initialise(absencePolicyId) {
            vm.absencePolicyId = absencePolicyId;
            order("AbsencePeriod.StartDate").then(function () {
                retrieveUnassignedAbsencePeriods();
            });
        }

        function createAbsencePolicyAbsencePeriod() {
            vm.assigning = true;
            return AbsencePolicyService.createAbsencePolicyAbsencePeriod(vm.absencePolicyId, vm.absencePeriod.AbsencePeriodId)
               .then(function () {
                   initialise(vm.absencePolicyId);
               });
        }

        function retrieveUnassignedAbsencePeriods() {
            return AbsencePolicyService.retrieveUnassignedAbsencePeriods(vm.absencePolicyId)
               .then(function (response) {
                   vm.ddAbsencePeriod = response.data;
                   vm.absencePeriod = response.data[0];
                   vm.assigning = vm.ddAbsencePeriod.length == 0;
                   return vm.ddAbsencePeriod;
               });
        }

        function retrieveAbsencePeriods() {
            return AbsencePolicyService.retrieveAbsencePeriods(vm.absencePolicyId, vm.paging, vm.orderBy)
                .then(function (response) {
                vm.absencePeriodError = false;
                    vm.absencePeriods = response.data.Items;
                    if (vm.absencePeriods.length > 0) {
                        vm.absencePeriodCount = vm.absencePeriods.length;
                    } else {
                        vm.absencePeriodError = true;
                        vm.absencePeriodCount = 0;
                    }
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.absencePeriods;
                });
        }

        function changeAbsencePeriod(absencePeriod) {
            vm.absencePeriod = absencePeriod;
        }

        function pageChanged() {
            return retrieveAbsencePeriods();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveAbsencePeriods();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function unassignAbsencePeriod(absencePolicyPeriod) {
            if (absencePolicyPeriod.CanUnassign) {
                vm.absencePolicyPeriodId = absencePolicyPeriod.AbsencePolicyPeriodId;
                return AbsencePolicyService.unassignAbsencePeriod(vm.absencePolicyPeriodId)
                    .then(function () {
                        initialise(vm.absencePolicyId);
                    });
            }
        }

        function unassignAbsencePeriodClass(absencePolicyPeriod) {
            return absencePolicyPeriod.CanUnassign ? '' : 'link-disabled';
        }

        function isAbsencesAssignedToAbsencePolicyPeriod(absencePolicyPeriodId) {
            vm.loadingActions = true;
            vm.absencePolicyPeriodId = absencePolicyPeriodId;
            return AbsencePolicyService.isAbsencesAssignedToAbsencePolicyPeriod(vm.absencePolicyPeriodId)
               .then(function (response) {
                   $filter('filter')(vm.absencePeriods, { AbsencePolicyPeriodId: vm.absencePolicyPeriodId })[0]["CanUnassign"] = !response.data;
                   vm.loadingActions = false;
               });
        }
    }
})();
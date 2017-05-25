(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsencePeriodController', AbsencePeriodController);

    AbsencePeriodController.$inject = ['$window', 'AbsencePeriodService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function AbsencePeriodController($window, AbsencePeriodService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.absencePeriods = [];
        vm.retrieveAbsencePeriods = retrieveAbsencePeriods;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editAbsencePeriod = editAbsencePeriod;
        vm.canDeleteAbsencePeriod = canDeleteAbsencePeriod;
        vm.deleteAbsencePeriod = deleteAbsencePeriod;
        initialise();

        function initialise() {
            order("StartDate");
        }

        function retrieveAbsencePeriods() {
            return AbsencePeriodService.retrieveAbsencePeriods(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.absencePeriods = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.absencePeriods;
                });
        }

        function canDeleteAbsencePeriod(absencePeriodId) {
            vm.loadingActions = true;
            vm.CanDeleteAbsencePeriod = false;
            AbsencePeriodService.canDeleteAbsencePeriod(absencePeriodId).then(function (response) {
                vm.CanDeleteAbsencePeriod = response.data;
                vm.loadingActions = false;
            });
        }

        function deleteAbsencePeriod(absencePeriodId) {
            return AbsencePeriodService.deleteAbsencePeriod(absencePeriodId)
              .then(function () {
                  initialise();
              });
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

        function editAbsencePeriod(id) {
            $window.location.href = "/AbsencePeriod/Edit/" + id;
        }

    }
})();

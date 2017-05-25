(function () {
    'use strict';

    angular
        .module('HR')
        .controller('OvertimeForApprovalController', OvertimeForApprovalController);

    OvertimeForApprovalController.$inject = ['$window', 'OvertimeForApprovalService', 'OvertimeService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function OvertimeForApprovalController($window, OvertimeForApprovalService, OvertimeService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.overtimeForApprovals = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.approveOvertime = approveOvertime;
        vm.declineOvertime = declineOvertime;
        vm.deleteOvertime = deleteOvertime;
        vm.editOvertime = editOvertime;
        vm.initialise = initialise;

        function initialise() {
            vm.orderBy = OrderService.order(vm.orderBy, "Date");
            order("Date");
        }

        function approveOvertime(overtimeForApprovals) {
            return OvertimeForApprovalService.approveOvertime(overtimeForApprovals)
            .then(function (response) {
                if (response.data.length === 0) {
                    retrieveForApprovalOvertimes();
                }
            });
        }

        function declineOvertime(overtimeForApprovals) {
            return OvertimeForApprovalService.declineOvertime(overtimeForApprovals)
            .then(function (response) {
                if (response.data.length === 0) {
                    retrieveForApprovalOvertimes();
                }
            });
        }

        function deleteOvertime(id) {
            return OvertimeService.deleteOvertime(id)
              .then(function () {
                  retrieveOvertimes();
              });
        }

        function retrieveForApprovalOvertimes() {
            return OvertimeForApprovalService.retrievOvertimeeForApprovals(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.overtimeForApprovals = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.overtimeForApprovals;
                });
        }

        function pageChanged() {
            return retrieveForApprovalOvertimes();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveForApprovalOvertimes();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editOvertime(personnelId, id) {
            $window.location.href = "/Overtime/Edit/" + personnelId + "/" + id;
        }
        
    }
})();

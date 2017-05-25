(function () {
    'use strict';

    angular
        .module('HR')
        .controller('OvertimeController', OvertimeController);

    OvertimeController.$inject = ['$window', 'OvertimeService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function OvertimeController($window, OvertimeService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.personnelId;
        vm.overtimes = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editOvertime = editOvertime;
        vm.approveOvertime = approveOvertime;
        vm.canApproveOvertime = canApproveOvertime;
        vm.declineOvertime = declineOvertime;
        vm.deleteOvertime = deleteOvertime;
        vm.initialise = initialise;

        function initialise(personnelId) {
            vm.personnelId = personnelId;
            vm.orderBy = OrderService.order(vm.orderBy, "OvertimeId");
            order("OvertimeId");
        }

        function retrieveOvertimes() {
            return OvertimeService.retrieveOvertimes(vm.personnelId, vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.overtimes = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.overtimes;
                });
        }

        function approveOvertime(id) {
            return OvertimeService.approveOvertime(vm.personnelId, id)
              .then(function () {
                  $window.location.reload(true);
              });
        }

        function canApproveOvertime(overtime) {
            return OvertimeService.canApproveOvertime(vm.personnelId, overtime.OvertimeId)
                .then(function (response) {
                    overtime.CanApproveOvertime = response.data;
                    return response.data;
                });
        }

        function declineOvertime(id) {
            return OvertimeService.declineOvertime(vm.personnelId, id)
              .then(function () {
                  $window.location.reload(true);
              });
        }

        function deleteOvertime(id) {
            return OvertimeService.deleteOvertime(id)
              .then(function () {
                  retrieveOvertimes();
              });
        }

        function pageChanged() {
            return retrieveOvertimes();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveOvertimes();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editOvertime(id) {
            $window.location.href = "/Overtime/Edit/" + vm.personnelId + "/" + id;
        }
        
    }
})();

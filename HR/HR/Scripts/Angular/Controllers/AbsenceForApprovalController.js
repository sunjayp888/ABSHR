(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsenceForApprovalController', AbsenceForApprovalController);

    AbsenceForApprovalController.$inject = ['$window', 'AbsenceForApprovalService', 'AbsenceService', 'Paging', 'OrderService', 'OrderBy', 'Order', '$rootScope'];

    function AbsenceForApprovalController($window, AbsenceForApprovalService, AbsenceService, Paging, OrderService, OrderBy, Order, $rootScope) {
        /* jshint validthis:true */
        var vm = this;
        vm.absenceForApprovals = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.approveAbsence = approveAbsence;
        vm.cancelAbsence = cancelAbsence;
        vm.declineAbsence = declineAbsence;
        vm.editAbsence = editAbsence;
        vm.initialise = initialise;

        function initialise() {
            order("DateFrom");
        }

        function approveAbsence(absenceForApprovals) {
            return AbsenceForApprovalService.approveAbsence(absenceForApprovals)
            .then(function (response) {
                if (response.data.length === 0) {
                    retrieveForApprovalAbsences();
                    $rootScope.$emit("UpdateAbsencePlanner");
                }
            });
        }

        function cancelAbsence(personnelId, absenceId) {
            return AbsenceService.cancelAbsence(personnelId, absenceId)
                .then(function (response) {
                    $window.location.reload(true);
                });
        }

        function declineAbsence(absenceForApprovals) {
            return AbsenceForApprovalService.declineAbsence(absenceForApprovals)
            .then(function (response) {
                if (response.data.length === 0) {
                    retrieveForApprovalAbsences();
                    $rootScope.$emit("UpdateAbsencePlanner");
                }
            });
        }

        function retrieveForApprovalAbsences() {
            return AbsenceForApprovalService.retrievAbsenceeForApprovals(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.absenceForApprovals = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.absenceForApprovals;
                });
        }

        function pageChanged() {
            return retrieveForApprovalAbsences();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveForApprovalAbsences();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editAbsence(personnelId ,id) {
            $window.location.href = "/Absence/Edit/" + personnelId + "/" + id;
        }

        $rootScope.$on("UpdateAbsenceGrid", function () {
            retrieveForApprovalAbsences();
        });
        
    }
})();

(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsencePolicyController', AbsencePolicyController);

    AbsencePolicyController.$inject = ['$window', 'AbsencePolicyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function AbsencePolicyController($window, AbsencePolicyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.absencePolicyId = 0;
        vm.absencePolicies = [];
        vm.retrieveAbsencePolicies = retrieveAbsencePolicies;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editAbsencePolicy = editAbsencePolicy;
        vm.isAbsencePolicyMappingExists = isAbsencePolicyMappingExists;
        vm.deleteAbsencePolicy = deleteAbsencePolicy;
        vm.cloneAbsencePolicy = cloneAbsencePolicy;
        initialise();

        function initialise() {
            order("Name");
        }

        function isAbsencePolicyMappingExists(id) {
            vm.absencePolicyId = id;
            vm.loadingActions = true;
            vm.IsAbsencePolicyMapped = true;
            AbsencePolicyService.isAbsencePolicyMappingExists(vm.absencePolicyId).then(function (response) { vm.IsAbsencePolicyMapped = response.data, vm.loadingActions = false; });

        }


        function retrieveAbsencePolicies() {
            return AbsencePolicyService.retrieveAbsencePolicies(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.absencePolicies = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.absencePolicies;
                });
        }

        function deleteAbsencePolicy(id) {
            vm.absencePolicyId = id;
            return AbsencePolicyService.deleteAbsencePolicy(vm.absencePolicyId)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveAbsencePolicies();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveAbsencePolicies();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editAbsencePolicy(id) {
            vm.absencePolicyId = id;
            $window.location.href = "/AbsencePolicy/Edit/" + vm.absencePolicyId;
        }

        function cloneAbsencePolicy(id) {
            vm.absencePolicyId = id;
            return AbsencePolicyService.cloneAbsencePolicy(vm.absencePolicyId).then(function (response) {
                //retrieveAbsencePolicies();
                $window.location.href = "/AbsencePolicy/Edit/" + response.data.AbsencePolicyId;
                return response;
            });
        }

    }
})();

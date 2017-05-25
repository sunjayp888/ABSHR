(function () {
    'use strict';

    angular
        .module('HR')
        .controller('PublicHolidayPolicyController', PublicHolidayPolicyController);

    PublicHolidayPolicyController.$inject = ['$window','PublicHolidayPolicyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function PublicHolidayPolicyController($window, PublicHolidayPolicyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.PublicHolidayPolicies = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editPublicHolidayPolicy = editPublicHolidayPolicy;
        vm.canDeletePublicHolidayPolicy = canDeletePublicHolidayPolicy;
        vm.deletePublicHolidayPolicy = deletePublicHolidayPolicy;
        vm.clonePublicHolidayPolicy = clonePublicHolidayPolicy;
        initialise();

        function initialise() {
            order("Name");
        }

        function retrievePublicHolidayPolicies() {
            return PublicHolidayPolicyService.retrievePublicHolidayPolicies(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.PublicHolidayPolicies = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.PublicHolidayPolicies;
                });
        }

        function canDeletePublicHolidayPolicy(id) {
            vm.loadingActions = true;
            vm.CanDeletePublicHolidayPolicy = false;
            PublicHolidayPolicyService.canDeletePublicHolidayPolicy(id).then(function (response) { vm.CanDeletePublicHolidayPolicy = response.data, vm.loadingActions = false });
        }

        function deletePublicHolidayPolicy(id) {
            return PublicHolidayPolicyService.deletePublicHolidayPolicy(id)
              .then(function () {
                  retrievePublicHolidayPolicies();
              });
        }

        function clonePublicHolidayPolicy(id) {
            return PublicHolidayPolicyService.clonePublicHolidayPolicy(id).then(function (response) {
                //retrievePublicHolidayPolicies();
                $window.location.href = "/PublicHolidayPolicy/Edit/" + response.data.PublicHolidayPolicyId;
                return response;
            });
        }

        function pageChanged() {
            return retrievePublicHolidayPolicies();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrievePublicHolidayPolicies();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editPublicHolidayPolicy(id) {
            $window.location.href = "/PublicHolidayPolicy/Edit/" + id;
        }
    }
})();

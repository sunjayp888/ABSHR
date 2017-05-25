(function () {
    'use strict';

    angular
        .module('HR')
        .controller('SiteController', SiteController);

    SiteController.$inject = ['$window', 'SiteService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function SiteController($window, SiteService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.sites = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editSite = editSite;
        vm.isSiteMappingExists = isSiteMappingExists;
        vm.deleteSite = deleteSite;


        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveSites() {
            return SiteService.retrieveSites(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.sites = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.sites;
                });
        }

        function isSiteMappingExists(id) {
            vm.loadingActions = true;
            vm.IsSiteMapped = true;
            SiteService.isSiteMappingExists(id).then(function (response) { vm.IsSiteMapped = response.data, vm.loadingActions = false });
        }

        function deleteSite(id) {
            return SiteService.deleteSite(id)
              .then(function () {
                  initialise();
              });
        }


        function pageChanged() {
            return retrieveSites();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveSites();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }
        
        function editSite(id) {
            $window.location.href = "/Site/Edit/" + id;
        }
    }
})();

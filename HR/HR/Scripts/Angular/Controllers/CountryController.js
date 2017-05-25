(function () {
    'use strict';

    angular
        .module('HR')
        .controller('CountryController', CountryController);

    CountryController.$inject = ['$window', 'CountryService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function CountryController($window, CountryService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.countries = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editCountry = editCountry;
        vm.canDeleteCountry = canDeleteCountry;
        vm.deleteCountry = deleteCountry;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveCountries() {
            return CountryService.retrieveCountries(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.countries = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.countries;
                });
        }

        function canDeleteCountry(id) {
            vm.loadingActions = true;
            vm.CanDeleteCountry = true;
            CountryService.canDeleteCountry(id).then(function (response) { vm.CanDeleteCountry = response.data, vm.loadingActions = false });
        }

        function deleteCountry(id) {
            return CountryService.deleteCountry(id)
              .then(function () {
                  initialise();
              });
        }


        function pageChanged() {
            return retrieveCountries();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveCountries();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editCountry(id) {
            $window.location.href = "/Country/Edit/" + id;
        }
    }
})();

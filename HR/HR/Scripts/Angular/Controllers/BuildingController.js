(function () {
    'use strict';

    angular
        .module('HR')
        .controller('BuildingController', BuildingController);

    BuildingController.$inject = ['$window', 'BuildingService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function BuildingController($window, BuildingService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.buildings = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editBuilding = editBuilding;
        vm.isBuildingMappingExists = isBuildingMappingExists;
        vm.deleteBuilding = deleteBuilding;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveBuildings() {
            return BuildingService.retrieveBuildings(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.buildings = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.buildings;
                });
        }

        function isBuildingMappingExists(id) {
            vm.loadingActions = true;
            vm.IsBuildingMapped = true;
            BuildingService.isBuildingMappingExists(id).then(function (response) { vm.IsBuildingMapped = response.data, vm.loadingActions = false; });
        }

        function deleteBuilding(id) {
            return BuildingService.deleteBuilding(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveBuildings();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveBuildings();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editBuilding(id) {
            $window.location.href = "/Building/Edit/" + id;
        }

    }
})();

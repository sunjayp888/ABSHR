(function () {
    'use strict';

    angular
        .module('HR')
        .controller('ApprovalModelController', ApprovalModelController);

    ApprovalModelController.$inject = ['$window', 'ApprovalModelService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function ApprovalModelController($window, ApprovalModelService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.approvalModels = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editApprovalModel = editApprovalModel;
        vm.isApprovalModelMappingExists = isApprovalModelMappingExists;
        vm.deleteApprovalModel = deleteApprovalModel;
        vm.initialise = initialise;

        function initialise() {
            order("Name");
        }

        function retrieveApprovalModels() {
            return ApprovalModelService.retrieveApprovalModels(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.approvalModels = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.approvalModels;
                });
        }

        function isApprovalModelMappingExists(id) {
            vm.loadingActions = true;
            vm.IsApprovalModelMapped = true;
            ApprovalModelService.isApprovalModelMappingExists(id).then(function (response) { vm.IsApprovalModelMapped = response.data, vm.loadingActions = false });
        }

        function deleteApprovalModel(id) {
            return ApprovalModelService.deleteApprovalModel(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveApprovalModels();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveApprovalModels();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editApprovalModel(id) {
            $window.location.href = "/ApprovalModel/Edit/" + id;            
        }
        
    }
})();

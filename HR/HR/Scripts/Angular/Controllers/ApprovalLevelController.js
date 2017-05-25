(function () {
    'use strict';

    angular
        .module('HR')
        .controller('ApprovalLevelController', ApprovalLevelController);

    ApprovalLevelController.$inject = ['$window', 'ApprovalLevelService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function ApprovalLevelController($window, ApprovalLevelService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.approvalLevels = [];
        vm.aprovalModelId;
        vm.levelNumber;
        vm.createApprovalLevel = createApprovalLevel;
        vm.editApprovalLevel = editApprovalLevel;
        vm.isApprovalLevelMappingExists = isApprovalLevelMappingExists;
        vm.deleteApprovalLevel = deleteApprovalLevel;
        vm.initialise = initialise;

        function initialise(approvalModelId) {
            vm.aprovalModelId = approvalModelId;
            retrieveApprovalLevels();
        }

        function createApprovalLevel() {
            return ApprovalLevelService.createApprovalLevel(vm.aprovalModelId, vm.levelNumber)
              .then(function () {
                  initialise();
              });
        }

        function retrieveApprovalLevels() {
            return ApprovalLevelService.retrieveApprovalLevels(vm.aprovalModelId)
                .then(function (response) {
                    vm.approvalLevels = response.data;                 
                    return vm.approvalLevels;
                });
        }

        function isApprovalLevelMappingExists(id) {
            vm.loadingActions = true;
            vm.IsApprovalLevelMapped = true;
            ApprovalLevelService.isApprovalLevelMappingExists(id).then(function (response) { vm.IsApprovalLevelMapped = response.data, vm.loadingActions = false });
        }

        function deleteApprovalLevel(id) {
            return ApprovalLevelService.deleteApprovalLevel(id)
              .then(function () {
                  initialise();
              });
        }

        function editApprovalLevel(id) {
            $window.location.href = "/ApprovalLevel/Edit/" + id;            
        }
        
    }
})();

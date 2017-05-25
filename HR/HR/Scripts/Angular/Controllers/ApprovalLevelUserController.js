(function () {
    'use strict';

    angular
        .module('HR')
        .controller('ApprovalLevelUserController', ApprovalLevelUserController);

    ApprovalLevelUserController.$inject = ['$window', 'ApprovalLevelUserService', 'Paging', 'OrderService', 'OrderBy', 'Order','$filter'];

    function ApprovalLevelUserController($window, ApprovalLevelUserService, Paging, OrderService, OrderBy, Order, $filter) {
        /* jshint validthis:true */
        var vm = this;        
        vm.approvalLevelUsers = [];
        vm.approvalUsers = [];
        vm.filteredApprovalUsers = [];
        vm.aprovalLevelId;
        vm.aspNetUser;
        vm.createApprovalLevelUser = createApprovalLevelUser;
        vm.filterAvailableApprovalUser = filterAvailableApprovalUser;
        vm.deleteApprovalLevelUser = deleteApprovalLevelUser;
        vm.changeAspNetUser = changeAspNetUser;
        vm.initialise = initialise;

        function initialise(approvalModelId) {
            vm.aprovalLevelId = approvalModelId;
            retrieveApprovalLevelUsers();
            retrieveAvailableApprovalUser();
        }

        function createApprovalLevelUser() {
            return ApprovalLevelUserService.createApprovalLevelUser(vm.aprovalLevelId, vm.aspNetUser.AspNetUserId)
              .then(function (response) {
                  if (response.data.length === 0) {
                      retrieveApprovalLevelUsers();
                      retrieveAvailableApprovalUser();
                  }
              });
        }

        function filterAvailableApprovalUser(filter) {
            //if (filter !== "") {
            //    vm.filteredApprovalUsers = $filter('filter')(vm.approvalUsers, { Surname: filter }, true);
            //}
            vm.filteredApprovalUsers = [];
            angular.forEach(vm.approvalUsers, function (item) {
                if (item.Surname.includes(filter)) {
                    vm.filteredApprovalUsers.push(item);
                }
            });
        }

        function retrieveApprovalLevelUsers() {
            return ApprovalLevelUserService.retrieveApprovalLevelUsers(vm.aprovalLevelId)
                .then(function (response) {
                    vm.approvalLevelUsers = response.data;                 
                    return vm.approvalLevelUsers;
                });
        }

        function retrieveAvailableApprovalUser() {
            return ApprovalLevelUserService.retrieveAvailableApprovalUser(vm.aprovalLevelId)
                .then(function (response) {
                    vm.approvalUsers = response.data;
                    return vm.approvalUsers;
                });
        }

        function deleteApprovalLevelUser(id) {
            return ApprovalLevelUserService.deleteApprovalLevelUser(id)
              .then(function () {
                  retrieveApprovalLevelUsers();
                  retrieveAvailableApprovalUser();
              });
        }

        function changeAspNetUser(approvalUser) {
            vm.aspNetUser = approvalUser;
        }
        
    }
})();

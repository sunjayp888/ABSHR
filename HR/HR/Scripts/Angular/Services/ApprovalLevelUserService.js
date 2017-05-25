(function () {
    'use strict';

    angular
        .module('HR')
        .factory('ApprovalLevelUserService', ApprovalLevelUserService);

    ApprovalLevelUserService.$inject = ['$http'];

    function ApprovalLevelUserService($http) {
        var service = {
            createApprovalLevelUser: createApprovalLevelUser,
            retrieveApprovalLevelUsers: retrieveApprovalLevelUsers,
            retrieveAvailableApprovalUser: retrieveAvailableApprovalUser,
            deleteApprovalLevelUser: deleteApprovalLevelUser
        };

        return service;

        function createApprovalLevelUser(approvalLevelId, aspNetUserId) {

            var url = "/ApprovalLevelUser/Create",
                data = {
                    ApprovalLevelId: approvalLevelId,
                    AspNetUserId: aspNetUserId
                };

            return $http.post(url, data);
        }

        function retrieveApprovalLevelUsers(aprovalLevelId) {

            var url = "/ApprovalLevelUser/List",
                data = {
                    id: aprovalLevelId
                };

            return $http.post(url, data);
        }

        function retrieveAvailableApprovalUser(aprovalLevelId) {

            var url = "/ApprovalLevelUser/ListAvailableApprovalUsers",
                data = {
                    id: aprovalLevelId
                };

            return $http.post(url, data);
        }

        function deleteApprovalLevelUser(id) {
            var url = "/ApprovalLevelUser/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
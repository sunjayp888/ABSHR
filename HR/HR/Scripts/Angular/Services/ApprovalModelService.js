(function () {
    'use strict';

    angular
        .module('HR')
        .factory('ApprovalModelService', ApprovalModelService);

    ApprovalModelService.$inject = ['$http'];

    function ApprovalModelService($http) {
        var service = {
            retrieveAllApprovalModels: retrieveAllApprovalModels,
            retrieveApprovalModels: retrieveApprovalModels,
            isApprovalModelMappingExists: isApprovalModelMappingExists,
            deleteApprovalModel: deleteApprovalModel
        };

        return service;

        function retrieveAllApprovalModels() {

            var url = "/ApprovalModel/ListAll";

            return $http.post(url);
        }

        function retrieveApprovalModels(Paging, OrderBy) {

            var url = "/ApprovalModel/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteApprovalModel(id) {
            var url = "/ApprovalModel/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isApprovalModelMappingExists(id) {
            var url = "/ApprovalModel/CanDeleteApprovalModel",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
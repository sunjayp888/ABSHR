(function () {
    'use strict';

    angular
        .module('HR')
        .factory('ApprovalLevelService', ApprovalLevelService);

    ApprovalLevelService.$inject = ['$http'];

    function ApprovalLevelService($http) {
        var service = {
            createApprovalLevel: createApprovalLevel,
            retrieveApprovalLevels: retrieveApprovalLevels,
            isApprovalLevelMappingExists: isApprovalLevelMappingExists,
            deleteApprovalLevel: deleteApprovalLevel
        };

        return service;

        function createApprovalLevel(aprovalModelId, levelNumber) {

            var url = "/ApprovalLevel/Create",
                data = {
                    ApprovalModelId: aprovalModelId,
                    LevelNumber: levelNumber
                };

            return $http.post(url, data);
        }

        function retrieveApprovalLevels(id) {

            var url = "/ApprovalLevel/List",
                data = {
                    id: id
                };

            return $http.post(url, data);
        }

        function deleteApprovalLevel(id) {
            var url = "/ApprovalLevel/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isApprovalLevelMappingExists(id) {
            var url = "/ApprovalLevel/CanDeleteApprovalLevel",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
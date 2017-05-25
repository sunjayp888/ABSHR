(function () {
    'use strict';

    angular
        .module('HR')
        .factory('TeamService', TeamService);

    TeamService.$inject = ['$http'];

    function TeamService($http) {
        var service = {
            retrieveListTeamFilters: retrieveListTeamFilters,
            retrieveTeams: retrieveTeams,
            isTeamMappingExists: isTeamMappingExists,
            deleteTeam: deleteTeam
        };

        return service;

        function retrieveListTeamFilters() {

            var url = "/Team/ListTeamFilters"

            return $http.post(url);
        }

        function retrieveTeams(Paging, OrderBy) {

            var url = "/Team/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteTeam(id) {
            var url = "/Team/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function isTeamMappingExists(id) {
            var url = "/Team/CanDeleteTeam",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
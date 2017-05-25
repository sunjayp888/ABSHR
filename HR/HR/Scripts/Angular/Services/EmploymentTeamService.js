(function () {
    'use strict';

    angular
        .module('HR')
        .factory('EmploymentTeamService', EmploymentTeamService);

    EmploymentTeamService.$inject = ['$http'];

    function EmploymentTeamService($http) {
        var service = {
            createEmploymentTeam: createEmploymentTeam,
            retrieveEmploymentTeams: retrieveEmploymentTeams,
            deleteEmploymentTeam: deleteEmploymentTeam
        };

        return service;

        function createEmploymentTeam(employmentId, teamId) {

            var url = "/EmploymentTeam/Create",
                data = {
                    EmploymentId: employmentId,
                    TeamId: teamId
                };

            return $http.post(url, data);
        }

        function retrieveEmploymentTeams(employmentId) {

            var url = "/EmploymentTeam/List",
                data = {
                    employmentId: employmentId
                };

            return $http.post(url, data);
        }

        function deleteEmploymentTeam(employmentId, teamId) {
            var url = "/EmploymentTeam/Delete",
              data = {
                  employmentId: employmentId,
                  teamId: teamId
              };
            return $http.post(url, data);
        }

    }
})();
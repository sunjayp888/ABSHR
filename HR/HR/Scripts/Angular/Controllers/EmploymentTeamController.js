(function () {
    'use strict';

    angular
        .module('HR')
        .controller('EmploymentTeamController', EmploymentTeamController);

    EmploymentTeamController.$inject = ['EmploymentTeamService', 'TeamService', '$filter'];

    function EmploymentTeamController(EmploymentTeamService, TeamService, $filter) {
        /* jshint validthis:true */
        var vm = this;
        vm.employmentId;
        vm.teams = [];
        vm.employmentTeams = [];
        vm.initialise = initialise;
        vm.createEmploymentTeam = createEmploymentTeam;
        vm.deleteEmploymentTeam = deleteEmploymentTeam;

        function initialise(employmentId) {
            vm.employmentId = employmentId
            retrieveTeams();
        }

        function retrieveTeams() {
            return TeamService.retrieveListTeamFilters()
                .then(function (response) {
                    vm.teams = response.data;
                    retrieveEmploymentTeams();
                    return vm.teams;
                });
        }

        function retrieveEmploymentTeams() {
            return EmploymentTeamService.retrieveEmploymentTeams(vm.employmentId)
                .then(function (response) {
                    //vm.employmentTeams = response.data.Items;
                    angular.forEach(response.data, function (item) {
                        var team = $filter('filter')(vm.teams, { TeamId: item.TeamId }, true)[0];
                        vm.employmentTeams.push({ TeamId: team.TeamId, Name: team.Name, Hex: team.Hex });
                    });
                    return vm.employmentTeams;
                });
        }

        function createEmploymentTeam($item) {
            return EmploymentTeamService.createEmploymentTeam(vm.employmentId, $item.TeamId)
              .then(function () {
              });
        }

        function deleteEmploymentTeam($item) {
            return EmploymentTeamService.deleteEmploymentTeam(vm.employmentId, $item.TeamId)
              .then(function () {
              });
        }

        
    }
})();

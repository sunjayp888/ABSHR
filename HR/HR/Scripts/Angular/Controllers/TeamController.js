(function () {
    'use strict';

    angular
        .module('HR')
        .controller('TeamController', TeamController);

    TeamController.$inject = ['$window', 'TeamService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function TeamController($window, TeamService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.teams = [];
        vm.selectedTeams = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editTeam = editTeam;
        vm.isTeamMappingExists = isTeamMappingExists;
        vm.deleteTeam = deleteTeam;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveTeams() {
            return TeamService.retrieveTeams(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.teams = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.teams;
                });
        }

        function isTeamMappingExists(id) {
            vm.loadingActions = true;
            vm.IsTeamMapped = true;
            TeamService.isTeamMappingExists(id).then(function (response) { vm.IsTeamMapped = response.data, vm.loadingActions = false });
        }

        function deleteTeam(id) {
            return TeamService.deleteTeam(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveTeams();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveTeams();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editTeam(id) {
            $window.location.href = "/Team/Edit/" + id;            
        }
        
    }
})();

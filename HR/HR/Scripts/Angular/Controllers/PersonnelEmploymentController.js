(function () {
    'use strict';

    angular
        .module('HR')
        .controller('PersonnelEmploymentController', PersonnelEmploymentController);

    PersonnelEmploymentController.$inject = ['$window', 'TeamService', 'DepartmentService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function PersonnelEmploymentController($window, TeamService, DepartmentService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.teams = [];
        vm.orderBy = new OrderBy;
        vm.paging = new Paging;
        //vm.createEmploymentTeam = createEmploymentTeam;
        //vm.createEmploymentDepartment = createEmploymentDepartment;
        //vm.deleteEmploymentDepartment = deleteEmploymentDepartment;
        //vm.deleteEmploymentTeam = deleteEmploymentTeam;
        vm.departments = [];
        vm.SelectedDepartmentIds = [];
        vm.SelectedTeamIds = [];
        

        initialise();

        function initialise() {
            order("Name");
            retrieveDepartment();
        }

        function retrieveTeams() {
            return TeamService.retrieveTeams(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.teams = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    angular.forEach(function (item) {
                        var team = $filter('filter')(vm.teams, { TeamId: item }, true);
                        vm.Teams.push({ TeamId: item, Name: team[0].Name, Hex: team[0].Hex });
                    });
                    return vm.teams;
                });
        }

        function retrieveDepartment() {
            return DepartmentService.retrieveDepartments(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.departments = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    angular.forEach(function (item) {
                        var department = $filter('filter')(vm.departments, { DepartmentId: item }, true);
                        vm.Departments.push({ DepartmentId: item, Name: department[0].Name, Hex: department[0].Hex });
                    });
                    return vm.departments;
                });
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveTeams();
        }

        //function createEmploymentTeam($item) {
        //    vm.SelectedTeamIds.push($item.TeamId);
        //    $('#SelectedTeamIdsJson').val(vm.SelectedTeamIds.toString());
        //}

        //function createEmploymentDepartment($item) {
        //    vm.SelectedDepartmentIds.push($item.DepartmentId);
        //    $('#SelectedDepartmentIdsJson').val(vm.SelectedDepartmentIds.toString());
        //}

        //function deleteEmploymentDepartment($item) {
        //    vm.SelectedDepartmentIds.splice($.inArray($item.DepartmentId, vm.SelectedDepartmentIds), 1);
        //    $('#SelectedDepartmentIdsJson').val(vm.SelectedDepartmentIds.toString());
        //}

        //function deleteEmploymentTeam($item) {
        //    vm.SelectedTeamIds.splice($.inArray($item.TeamId, vm.SelectedTeamIds), 1);
        //    $('#SelectedTeamIdsJson').val(vm.SelectedTeamIds.toString());
        //}

    }
})();

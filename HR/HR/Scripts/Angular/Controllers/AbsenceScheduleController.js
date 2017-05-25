(function () {
    'use strict';

    angular
        .module('HR')
        .controller('AbsenceScheduleController', AbsenceScheduleController);

    AbsenceScheduleController.$inject = ['$window', '$location', '$filter', 'AbsenceScheduleService', 'AbsenceService', 'OrganisationalChartService', 'moment', '$rootScope'];

    function AbsenceScheduleController($window, $location, $filter, AbsenceScheduleService, AbsenceService, OrganisationalChartService, moment, $rootScope) {
        /* jshint validthis:true */
        var vm = this;
        vm.beginDate = new moment().startOf('isoWeek').format();
        vm.personnelId = null;

        vm.personnelFilter = {
            CompanyIds: null,
            DepartmentIds: null,
            TeamIds: null

        };

        vm.personnelFilter.DepartmentIds = null;
        vm.personnelFilter.TeamIds = null;
        vm.organisationalChartViewModel;        
        vm.showColourBy;
        vm.chartColours;
        vm.companies = [];
        vm.departments = [];
        vm.teams = [];
        vm.schedule = null;
        vm.initialise = initialise;
        vm.retrieveAbsenceSchedule = retrieveAbsenceSchedule;
        vm.absenceLink = absenceLink;
        vm.canApproveAbsence = canApproveAbsence;
        vm.cancelAbsence = cancelAbsence;
        vm.approveAbsence = approveAbsence;
        vm.declineAbsence = declineAbsence;
        vm.filterSelected = filterSelected;

        function initialise(personnelId, companyIds, departmentIds, teamIds) {
            vm.personnelId = personnelId;
            vm.personnelFilter.CompanyIds = companyIds;
            vm.personnelFilter.DepartmentIds = departmentIds;
            vm.personnelFilter.TeamIds = teamIds;
            vm.chartColours = [{ id: 0, text: 'Company' }, { id: 1, text: 'Department' }, { id: 2, text: 'Team' }];
            vm.showColourBy = vm.chartColours[0];
            retrieveOrganisationalChartViewModel();
            retrieveAbsenceSchedule(vm.beginDate);
        }

        function retrieveAbsenceSchedule(date) {
            vm.beginDate = date;
            return AbsenceScheduleService.retrieveAbsenceSchedule(vm.beginDate, vm.personnelFilter, vm.showColourBy.id)
                .then(function (response) {
                    vm.schedule = response.data;
                    return vm.schedule;
            });
        }

        function absenceLink(action, personnelId, absenceId) {
            $window.location.href = "/Absence/" + action + "/" + personnelId + "/" + absenceId;
        }

        function canApproveAbsence(personnelId, slot) {
            return AbsenceService.canApproveAbsence(personnelId, slot.AbsenceId)
                .then(function (response) {
                    slot.CanApprove = response.data;
                    return response.data;
                });
        }

        function cancelAbsence(personnelId, absenceId) {
            return AbsenceService.cancelAbsence(personnelId, absenceId)
                .then(function (response) {
                    $window.location.reload(true);
                });
        }

        function approveAbsence(personnelId, absenceId) {
            return AbsenceService.approveAbsence(personnelId, absenceId)
                .then(function (response) {
                    //$window.location.reload(true);
                    retrieveAbsenceSchedule(vm.beginDate)
                    $rootScope.$emit("UpdateAbsenceGrid");
                });
        }

        function declineAbsence(personnelId, absenceId) {
            return AbsenceService.declineAbsence(personnelId, absenceId)
                .then(function (response) {
                    //$window.location.reload(true);
                    retrieveAbsenceSchedule(vm.beginDate)
                    $rootScope.$emit("UpdateAbsenceGrid");
                });
        }

        function retrieveOrganisationalChartViewModel() {
            return OrganisationalChartService.retrieveOrganisationalChartViewModel()
                .then(function (response) {
                    vm.organisationalChartViewModel = response.data;

                    angular.forEach(vm.personnelFilter.CompanyIds, function (item) {
                        var company = $filter('filter')(vm.organisationalChartViewModel.PersonnelDetailFilter.Company, { CompanyId: item }, true);
                        vm.companies.push({ CompanyId: item, Name: company[0].Name, Hex: company[0].Hex });
                    });

                    angular.forEach(vm.personnelFilter.DepartmentIds, function (item) {
                        var department = $filter('filter')(vm.organisationalChartViewModel.PersonnelDetailFilter.Department, { DepartmentId: item }, true);
                        vm.departments.push({ DepartmentId: item, Name: department[0].Name, Hex: department[0].Hex });
                    });

                    angular.forEach(vm.personnelFilter.TeamIds, function (item) {
                        var team = $filter('filter')(vm.organisationalChartViewModel.PersonnelDetailFilter.Team, { TeamId: item }, true);
                        vm.teams.push({ TeamId: item, Name: team[0].Name, Hex: team[0].Hex });
                    });

                });
        }

        function filterSelected() {
            vm.personnelFilter.CompanyIds = $.map(vm.companies, function (value, index) { return value.CompanyId; });
            vm.personnelFilter.DepartmentIds = $.map(vm.departments, function (value, index) { return value.DepartmentId; });
            vm.personnelFilter.TeamIds = $.map(vm.teams, function (value, index) { return value.TeamId; });
            retrieveAbsenceSchedule(vm.beginDate);
        }

        $rootScope.$on("UpdateAbsencePlanner", function () {
            retrieveAbsenceSchedule(vm.beginDate)
        });
    }
})();

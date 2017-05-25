(function () {
    'use strict';

    angular
        .module('HR')
        .controller('EmploymentController', EmploymentController);

    EmploymentController.$inject = ['$window', '$filter', 'EmploymentService'];

    function EmploymentController($window, $filter, EmploymentService) {
        /* jshint validthis:true */
        var vm = this;
        vm.init = init;
        vm.personnelId = 0;
        vm.employments = [];
        vm.employmentLink = employmentLink;
        vm.updatePreviousEmployment = updatePreviousEmployment;
        vm.deletePersonnelEmployment = deletePersonnelEmployment;
        vm.canDeletePersonnelEmployment = canDeletePersonnelEmployment;
       

        function init(personnelId) {
            vm.personnelId = personnelId;
            retrieveEmployments();
        }

        function retrieveEmployments() {
            return EmploymentService.retrieveEmployments(vm.personnelId)
                .then(function (response) {
                    vm.employments = response.data;
                    return vm.employments;
                });
        }

      

        function employmentLink(action, personnelId, employmentId) {
            $window.location.href = "/Employment/" + action + "/" + personnelId + "/" + employmentId;
        }

        function updatePreviousEmployment(employmentEndDate, personnelId, employmentId) {
            return EmploymentService.updatePreviousEmployment(employmentEndDate, personnelId, employmentId).then(function (response) {
                if (response.data === '') {
                    window.location.href = '../Create/' + personnelId;
                } else {
                    $('#previousEmploymentEndDateErrorSummary').show();
                    vm.Errors = response.data;
                }
            });
        }

        function canDeletePersonnelEmployment(id) {
            vm.loadingActions = true;
            vm.CanDeletePersonnelEmployment = false;
            EmploymentService.canDeletePersonnelEmployment(vm.personnelId, id).then(function (response) { vm.CanDeletePersonnelEmployment = response.data, vm.loadingActions = false });
        }

        function deletePersonnelEmployment(id) {
            EmploymentService.deletePersonnelEmployment(vm.personnelId, id).then(function (response) {
                $window.location.reload();
            });
        }
    }
})();

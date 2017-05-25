(function () {
    'use strict';

    angular
        .module('HR')
        .factory('EmploymentService', EmploymentService);

    EmploymentService.$inject = ['$http'];

    function EmploymentService($http) {

        var service = {
            retrieveEmployments: retrieveEmployments,
            updatePreviousEmployment: updatePreviousEmployment,
            canDeletePersonnelEmployment:canDeletePersonnelEmployment,
            deletePersonnelEmployment: deletePersonnelEmployment
        };

        return service;

        function retrieveEmployments(personnelId) {

            var url = "/Employment/List",
                data = {
                    personnelId: personnelId
                };

            return $http.post(url, data);
        }

        function updatePreviousEmployment(employmentEndDate, personnelId, employmentId) {
            var url = "/Employment/UpdatePreviousEmployment",
               data = {
                   employmentEndDate: employmentEndDate,
                   personnelId: personnelId,
                   employmentId: employmentId
               };

            return $http.post(url, data);
        }

        function canDeletePersonnelEmployment(personnelId, employmentId) {
            var url = "/Employment/CanDeletePersonnelEmployment",
               data = {
                   personnelId: personnelId,
                   employmentId: employmentId
               };

            return $http.post(url, data);
        }

        function deletePersonnelEmployment(personnelId, employmentId) {
            var url = "/Employment/DeletePersonnelEmployment",
               data = {
                   personnelId: personnelId,
                   employmentId: employmentId
               };

            return $http.post(url, data);
        }
    }
})();
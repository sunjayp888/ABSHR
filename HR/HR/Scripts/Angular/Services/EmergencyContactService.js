(function () {
    'use strict';

    angular
        .module('HR')
        .factory('EmergencyContactService', EmergencyContactService);

    EmergencyContactService.$inject = ['$http'];

    function EmergencyContactService($http) {

        var service = {
            retrieveEmergencyContacts: retrieveEmergencyContacts
        };

        return service;

        function retrieveEmergencyContacts(personnelId) {

            var url = "/EmergencyContact/List",
                data = {
                    personnelId: personnelId
                };

            return $http.post(url, data);
        }
    }
})();
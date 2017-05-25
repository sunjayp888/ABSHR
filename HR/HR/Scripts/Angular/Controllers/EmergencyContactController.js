(function () {
    'use strict';

    angular
        .module('HR')
        .controller('EmergencyContactController', EmergencyContactController);

    EmergencyContactController.$inject = ['$window', '$filter', 'EmergencyContactService'];

    function EmergencyContactController($window, $filter, EmergencyContactService) {
        /* jshint validthis:true */
        var vm = this;
        vm.init = init;
        vm.personnelId = 0;
        vm.emergencyContacts = [];
        vm.editEmergencyContact = editEmergencyContact;

        function init(personnelId) {
            vm.personnelId = personnelId;
            retrieveEmergencyContacts();
        }

        function retrieveEmergencyContacts() {
            return EmergencyContactService.retrieveEmergencyContacts(vm.personnelId)
                .then(function (response) {
                    vm.emergencyContacts = response.data;
                    return vm.emergencyContacts;
                });
        }

        function editEmergencyContact(id) {
            $window.location.href = "/EmergencyContact/Edit/" + id;
        }
    }
})();

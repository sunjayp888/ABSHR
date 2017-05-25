(function () {
    'use strict';

    angular
        .module('HR')
        .factory('PersonnelAbsenceEntitlementService', PersonnelAbsenceEntitlementService);

    PersonnelAbsenceEntitlementService.$inject = ['$http'];

    function PersonnelAbsenceEntitlementService($http) {

        var service = {
            retrievePersonnelAbsenceEntitlements: retrievePersonnelAbsenceEntitlements
        };

        return service;

        function retrievePersonnelAbsenceEntitlements(personnelId, absencePeriodId) {

            var url = "/PersonnelAbsenceEntitlement/List",
                data = {
                    personnelId: personnelId,
                    absencePeriodId: absencePeriodId
                };

            return $http.post(url, data);
        }
    }
})();
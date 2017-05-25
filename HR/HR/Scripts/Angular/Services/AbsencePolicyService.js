(function () {
    'use strict';

    angular
        .module('HR')
        .factory('AbsencePolicyService', AbsencePolicyService);

    AbsencePolicyService.$inject = ['$http'];

    function AbsencePolicyService($http) {
        var service = {
            retrieveUnassignedAbsencePolicyAbsenceTypes: retrieveUnassignedAbsencePolicyAbsenceTypes,
            retrieveAbsencePolicyEntitlements:retrieveAbsencePolicyEntitlements,
            retrieveAbsencePolicies: retrieveAbsencePolicies,
            isAbsencePolicyMappingExists: isAbsencePolicyMappingExists,
            deleteAbsencePolicy: deleteAbsencePolicy,
            assignAbsencePolicyAbsenceType: assignAbsencePolicyAbsenceType,
            editAbsencePolicyEntitlement:editAbsencePolicyEntitlement,
            updateAbsencePolicyEntitlement:updateAbsencePolicyEntitlement,
             isAbsencesAssignedToAbsencePolicyAbsenceType: isAbsencesAssignedToAbsencePolicyAbsenceType,
            unassignAbsencePolicyAbsenceType: unassignAbsencePolicyAbsenceType,
            retrieveUnassignedAbsencePeriods: retrieveUnassignedAbsencePeriods,
            createAbsencePolicyAbsencePeriod: createAbsencePolicyAbsencePeriod,
            retrieveAbsencePeriods: retrieveAbsencePeriods,
            unassignAbsencePeriod: unassignAbsencePeriod,
            isAbsencesAssignedToAbsencePolicyPeriod: isAbsencesAssignedToAbsencePolicyPeriod,
            cloneAbsencePolicy: cloneAbsencePolicy,
            retrieveAbsencePeriodsByPersonnel: retrieveAbsencePeriodsByPersonnel
        };

        return service;

        function retrieveAbsencePolicies(Paging, OrderBy) {

            var url = "/AbsencePolicy/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function deleteAbsencePolicy(absencePolicyId) {
            var url = "/AbsencePolicy/Delete",
             data = {
                 absencePolicyId: absencePolicyId
             };
            return $http.post(url, data);
        }

        function isAbsencePolicyMappingExists(absencePolicyId) {
            var url = "/AbsencePolicy/CanDeleteAbsencePolicy",
              data = {
                  absencePolicyId: absencePolicyId
              };
            return $http.post(url, data);
        }

        function retrieveUnassignedAbsencePolicyAbsenceTypes(absencePolicyId) {
            var url = "/AbsencePolicy/UnassignedAbsencePolicyAbsenceTypes",
                data = {
                    absencePolicyId: absencePolicyId
                };

            return $http.post(url, data);
        }

        function retrieveAbsencePolicyEntitlements(absencePolicyId) {            
            var url = "/AbsencePolicy/AbsencePolicyEntitlements",
                data = {
                    absencePolicyId: absencePolicyId
                };

            return $http.post(url, data);
        }

        function assignAbsencePolicyAbsenceType(absencePolicyId, absenceTypeId) {
            var url = "/AbsencePolicy/AssignAbsencePolicyAbsenceType",
                data = {
                    absencePolicyId: absencePolicyId,
                    absenceTypeId: absenceTypeId
                };

            return $http.post(url, data);
        }

        function editAbsencePolicyEntitlement(absencePolicyId, absencePolicyEntitlementId) {
            var url = "/AbsencePolicy/AbsencePolicyEntitlement";
            var data = {
                    absencePolicyId: absencePolicyId,
                    absencePolicyEntitlementId: absencePolicyEntitlementId
                };
            return $http.post(url, data);
        }

        function updateAbsencePolicyEntitlement(absencePolicyEntitlement) {
            var url = "/AbsencePolicy/UpdateAbsencePolicyEntitlement";
            var data = {
                absencePolicyEntitlement: absencePolicyEntitlement
            };
            return $http.post(url, data);
        }

        function isAbsencesAssignedToAbsencePolicyAbsenceType(absencePolicyId, absenceTypeId) {
            var url = "/AbsencePolicy/IsAbsencesAssignedToAbsencePolicyAbsenceType",
                data = {
                    absencePolicyId: absencePolicyId,
                    absenceTypeId: absenceTypeId
                };
            return $http.post(url, data);
        }

        function unassignAbsencePolicyAbsenceType(absencePolicyId, absenceTypeId) {
            var url = "/AbsencePolicy/UnassignAbsencePolicyAbsenceType",
                data = {
                    absencePolicyId: absencePolicyId,
                    absenceTypeId: absenceTypeId
                };
            return $http.post(url, data);
        }

        function createAbsencePolicyAbsencePeriod(absencePolicyId, absencePeriodId) {
            var url = "/AbsencePolicy/CreateAbsencePolicyPeriod",
                data = {
                    AbsencePolicyId:absencePolicyId,
                    AbsencePeriodId: absencePeriodId
                };

            return $http.post(url, data);
        }

        function retrieveUnassignedAbsencePeriods(absencePolicyId) {
            var url = "/AbsencePolicy/UnassignedAbsencePeriods",
                data = {
                    absencePolicyId: absencePolicyId
                };
            return $http.post(url, data);
        }

        function retrieveAbsencePeriods(absencePolicyId, Paging, OrderBy) {
            var url = "/AbsencePolicy/AbsencePeriods",
                data = {
                    absencePolicyId: absencePolicyId,
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };
            return $http.post(url, data);
        }

        function retrieveAbsencePeriodsByPersonnel(personnelId, Paging, OrderBy) {
            var url = "/AbsencePolicy/AbsencePeriodsByPersonnel",
                data = {
                    personnelId: personnelId,
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };
            return $http.post(url, data);
        }
        
        function unassignAbsencePeriod(absencePolicyPeriodId) {
            var url = "/AbsencePolicy/UnassignAbsencePeriod",
                data = {
                    absencePolicyPeriodId: absencePolicyPeriodId
                };
            return $http.post(url, data);
        }

        function isAbsencesAssignedToAbsencePolicyPeriod(absencePolicyPeriodId) {
            var url = "/AbsencePolicy/IsAbsencesAssignedToAbsencePolicyPeriod",
                data = {
                    absencePolicyPeriodId: absencePolicyPeriodId
                };
            return $http.post(url, data);
        }

        function cloneAbsencePolicy(absencePolicyId) {

            var url = "/AbsencePolicy/Clone",
                data = {
                    absencePolicyId: absencePolicyId
                };

            return $http.post(url, data);
        }
    }
})();
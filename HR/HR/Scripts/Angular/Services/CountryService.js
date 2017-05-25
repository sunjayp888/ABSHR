(function () {
    'use strict';

    angular
        .module('HR')
        .factory('CountryService', CountryService);

    CountryService.$inject = ['$http'];

    function CountryService($http) {
        var service = {
            retrieveCountries: retrieveCountries,
            retrieveYear: retrieveYear,
            retrieveCountryPublicHolidays: retrieveCountryPublicHolidays,
            IsPublicHolidayAlreadyAssigned: IsPublicHolidayAlreadyAssigned,
            createPublicHoliday: createPublicHoliday,
            deletePublicHoliday: deletePublicHoliday,
            retrieveUnassignedAbsencePeriods: retrieveUnassignedAbsencePeriods,
            retrieveCountryAbsenceTypes: retrieveCountryAbsenceTypes,
            assignCountryAbsenceType: assignCountryAbsenceType,
            canDeleteCountryAbsenceType : canDeleteCountryAbsenceType,
            unassignCountryAbsenceType: unassignCountryAbsenceType,
            canDeleteCountry: canDeleteCountry,
            deleteCountry: deleteCountry
        };

        return service;

        function retrieveCountries(Paging, OrderBy) {

            var url = "/Country/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function retrieveYear(countryId) {

            var url = "/Country/GetYear",
                data = {
                    countryId: countryId
                };

            return $http.post(url, data);
        }
        
        function retrieveCountryPublicHolidays(Paging, OrderBy, countryId, year) {
            var url = "/Country/RetrievePublicHolidays",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy),
                    countryId: countryId,
                    year: year
                };

            return $http.post(url, data);
        }

        function deletePublicHoliday(countryPublicHolidayId) {
            var url = "/Country/DeletePublicHoliday",
              data = {
                  countryPublicHolidayId: countryPublicHolidayId
              };

            return $http.post(url, data);
        }

        function createPublicHoliday(data) {
            var url = "/Country/CreateCountryPublicHoliday";
            return $http.post(url, data);
        }

        function IsPublicHolidayAlreadyAssigned(countryId, date) {
            var url = "/Country/IsPublicHolidayAlreadyAssigned",
              data = {
                  countryId: countryId,
                  date: date
              };
            return $http.post(url, data);
        }


        function retrieveUnassignedAbsencePeriods(countryId) {
            var url = "/Country/UnassignedCountryAbsenceTypes",
            data = {
                countryId: countryId
            };

            return $http.post(url, data);
        }

        function retrieveCountryAbsenceTypes(Paging, OrderBy, countryId) {
            var url = "/Country/CountryAbsenceTypes",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy),
                    countryId: countryId
                };

            return $http.post(url, data);
        }

        function assignCountryAbsenceType(countryId, absenceTypeId) {
            var url = "/Country/AssignCountryAbsenceType",
            data = {
                countryId: countryId,
                absenceTypeId: absenceTypeId
            };
            return $http.post(url, data);
        }

        function unassignCountryAbsenceType(countryAbsenceTypeId) {
            var url = "/Country/UnassignCountryAbsenceType",
              data = {
                  countryAbsenceTypeId: countryAbsenceTypeId
              };
            return $http.post(url, data);
        }

        function deleteCountry(id) {
            var url = "/Country/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function canDeleteCountry(id) {
            var url = "/Country/CanDeleteCountry",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function canDeleteCountryAbsenceType(countryAbsenceTypeId) {
            var url = "/Country/CanDeleteCountryAbsenceType",
             data = {
                 countryAbsenceTypeId: countryAbsenceTypeId
             };
            return $http.post(url, data);
        }
    }

})();
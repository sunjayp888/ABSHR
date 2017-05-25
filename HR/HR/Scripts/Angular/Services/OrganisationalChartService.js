(function () {
    'use strict';

    angular
        .module('HR')
        .factory('OrganisationalChartService', OrganisationalChartService);

    OrganisationalChartService.$inject = ['$http'];

    function OrganisationalChartService($http) {
        var service = {
            retrieveOrganisationalChart: retrieveOrganisationalChart,
            retrieveOrganisationalChartViewModel: retrieveOrganisationalChartViewModel
        };

        return service;

        function retrieveOrganisationalChart(personnelFilter, colourBy) {
            var url = "/OrganisationalChart/List",
                data = {
                    personnelFilter: personnelFilter,
                    showColourBy: colourBy
                };

            return $http.post(url, data);
        }

        function retrieveOrganisationalChartViewModel() {

            var url = "/OrganisationalChart/OrganisationalChartViewModel";

            return $http.post(url);
        }

    }
})();
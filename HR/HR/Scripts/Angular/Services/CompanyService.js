(function () {
    'use strict';

    angular
        .module('HR')
        .factory('CompanyService', CompanyService);

    CompanyService.$inject = ['$http'];

    function CompanyService($http) {
        var service = {
            retrieveCompany: retrieveCompany,
            isCompanyMappingExists: isCompanyMappingExists,
            deleteCompany: deleteCompany,
            retrieveSiteBuilding: retrieveSiteBuilding,
            retrieveUnassignedSiteBuilding: retrieveUnassignedSiteBuilding,
            assignCompanyBuilding: assignCompanyBuilding,
            unassignSiteBuilding: unassignSiteBuilding,
            companyBuildingHasEmployment: companyBuildingHasEmployment,
           
        };

        return service;

        function retrieveCompany(Paging, OrderBy) {
            var url = "/Company/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };
            return $http.post(url, data);
        }

        function deleteCompany(id) {
            var url = "/Company/Delete",
              data = {
                 id: id
              };
            return $http.post(url, data); 
        }

        function isCompanyMappingExists(id) {
            var url = "/Company/CanDeleteCompany",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }
        //CompanyBuilding
        function retrieveSiteBuilding(companyId) {

            var url = "/Company/CompanyBuilding",
                data = {
                   
                    companyId: companyId
                    //paging: Paging,
                    //orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function retrieveUnassignedSiteBuilding(companyId) {
            var url = "/Company/UnassignedSiteBuilding",
                data = {
                    companyId: companyId
                };

            return $http.post(url, data);
        }

        function assignCompanyBuilding(companyId, buildingId) {

            var url = "/Company/AssignCompanyBuilding",
                data = {
                    companyId: companyId,
                    buildingId: buildingId
                };

            return $http.post(url, data);
        }

     
        function companyBuildingHasEmployment(companyId, buildingId) {

            var url = "/Company/CanDeleteCompanyBuilding",
                data = {
                    companyId: companyId,
                    buildingId: buildingId
                };

            return $http.post(url, data);
        }

        function unassignSiteBuilding(companyBuildingId) {
            var url = "/Company/UnassignSiteBuilding",
              data = {
                  companyBuildingId: companyBuildingId
              };
            return $http.post(url, data);
        }
    }


})();
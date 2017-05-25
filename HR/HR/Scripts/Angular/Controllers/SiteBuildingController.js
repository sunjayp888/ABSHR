(function () {
    'use strict';

    angular
        .module('HR')
        .controller('SiteBuildingController', SiteBuildingController);

    SiteBuildingController.$inject = ['$window', '$filter', 'CompanyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function SiteBuildingController($window, $filter, CompanyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.initialise = initialise;
        vm.buildingId = 0;
        vm.companyId = 0;
        vm.siteId = 0;
        vm.companyBuildingId = 0;
        vm.buildingName = "";
        vm.siteName = "";
        vm.SiteBuilding = [];
        vm.ddSiteBuilding = [];
        vm.siteBuildingDefault = 0;
        vm.divisionCountryAbsencePeriodId = 0;
        vm.loadingActions = false;
        vm.unassignSiteBuilding = unassignSiteBuilding;
        vm.unassignSiteBuildingClass = unassignSiteBuildingClass;
        //vm.SiteBuildingHas = SiteBuildingHas;
        vm.SetCompanyBuilding = SetCompanyBuilding;
        vm.assignCompanyBuilding = assignCompanyBuilding;
        vm.companyBuildingHasEmployment =companyBuildingHasEmployment;
        vm.assigning = false;

        function initialise(companyId) {
          
            vm.companyId = companyId;
            retrieveSiteBuilding();
            retrieveUnassignedSiteBuilding();
        }

        function retrieveSiteBuilding() {
            return CompanyService.retrieveSiteBuilding(vm.companyId)
                .then(function (response) {
                    vm.SiteBuilding = response.data;
                    return vm.SiteBuilding;
                });
        }

        function retrieveUnassignedSiteBuilding() {
            return CompanyService.retrieveUnassignedSiteBuilding(vm.companyId)
               .then(function (response) {
                   vm.ddSiteBuilding = response.data;
                   vm.siteBuildingDefault = response.data[0];
                   vm.buildingId = vm.siteBuildingDefault.BuildingId;
                   vm.siteId = vm.siteBuildingDefault.Site.SiteId;
                   vm.assigning = vm.ddSiteBuilding.length == 0;
                   return vm.ddSiteBuilding;
               });
        }

        function unassignSiteBuilding(siteBuilding) {
            if (siteBuilding.CanAssign) {
                vm.companyBuildingId = siteBuilding.CompanyBuildingId;

                return CompanyService.unassignSiteBuilding(vm.companyBuildingId)
                   .then(function (response) {
                       initialise(vm.companyId);
                   });
            }
        }

        function unassignSiteBuildingClass(siteBuilding) {
            return !siteBuilding.CanAssign ? 'link-disabled' : '';
        }

        function companyBuildingHasEmployment(companyBuildingId, buildingId) {
            vm.loadingActions = true;
            vm.companyBuildingId = companyBuildingId;
            vm.buildingId = buildingId;
            return CompanyService.companyBuildingHasEmployment(vm.companyId,vm.buildingId)
               .then(function (response) {
                   $filter('filter')(vm.SiteBuilding, { CompanyBuildingId: vm.companyBuildingId })[0]["CanAssign"] = !response.data;
                   vm.loadingActions = false;
               });
        }

        function assignCompanyBuilding() {
            vm.assigning = true;
            return CompanyService.assignCompanyBuilding(vm.companyId, vm.buildingId)
               .then(function (response) {
                   initialise(vm.companyId);
               });
        }

        function SetCompanyBuilding(siteBuilding) {
            vm.siteBuildingDefault = siteBuilding;
            vm.buildingId = vm.siteBuildingDefault.BuildingId;
            vm.siteId = vm.siteBuildingDefault.Site.SiteId;
            vm.siteBuildingDefault = siteBuilding;

        }
    }
})();

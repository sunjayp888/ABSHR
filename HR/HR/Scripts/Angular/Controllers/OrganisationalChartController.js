(function () {
    'use strict';

    angular
        .module('HR')
        .controller('OrganisationalChartController', OrganisationalChartController);

    OrganisationalChartController.$inject = ['$window', 'OrganisationalChartService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function OrganisationalChartController($window, OrganisationalChartService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.organisationalChartViewModel;


        vm.personnelFilter = {
            CompanyIds: null,
            DepartmentIds: null,
            TeamIds: null
        };

        vm.showColourBy;
        vm.chartColours;
        vm.companies;
        vm.departments;
        vm.teams;
        vm.legends;
        vm.initialise = initialise;
        vm.retrieveOrganisationalChart = retrieveOrganisationalChart;
        vm.filterSelected = filterSelected;

        function initialise() {
            retrieveOrganisationalChartViewModel();
            vm.chartColours = [{ id: 0, text: 'Company' }, { id: 1, text: 'Department' }, { id: 2, text: 'Team' }];
            vm.showColourBy = vm.chartColours[0];
            retrieveOrganisationalChart();
        }

        function setLinktoPersonnelPage(sender, args) {
            
            args.content[1] = '<path class="get-box" d="M0 0 H 500 V 220 H 0"></path>';
            var argContent = '';
            var colourStart = 0;
            var colourCount = 0;
            angular.forEach(args.node.data.Colours, function (item) {
                colourCount += 1;
                if (colourCount <= 11) {
                    argContent += '<path class="get-box" d="M ' + colourStart + ' 220 H ' + (colourStart + 40) + ' V 260 H ' + colourStart + '" style="fill:#' + item.Hex + '; stroke:#' + item.Hex + '"></path>';
                    colourStart += 45;
                }
            });
            args.content[1] += argContent;

            args.content[2] = args.content[2].replace(args.content[2], (args.node.data.showLink) ? '<a href="/Personnel/Profile/' + args.node.id + '">' + args.content[2] + '</a>'
                : args.content[2]);
        }

        function retrieveOrganisationalChart() {
            return OrganisationalChartService.retrieveOrganisationalChart(vm.personnelFilter, vm.showColourBy.id)
                .then(function (response) {

                    var organizationalChartElement = document.getElementById("organisationalChart");
                    var orgChart = new getOrgChart(organizationalChartElement, {
                        enablePrint: false,
                        enableZoom: true,
                        scale: 0.5,
                        enableSearch: true,
                        theme: "annabel",
                        primaryFields: ["Name", "JobTitle"],
                        photoFields: ["Photo"],
                        linkType: "B",
                        orientation: getOrgChart.RO_TOP,
                        enableEdit: false,
                        enableDetailsView: false,
                        dataSource: (response.data.ChartItems.length == 0) ? null : response.data.ChartItems,
                        renderNodeEvent: setLinktoPersonnelPage
                    });

                });
        }


        function retrieveOrganisationalChartViewModel() {
            return OrganisationalChartService.retrieveOrganisationalChartViewModel()
                .then(function (response) {
                    vm.organisationalChartViewModel = response.data;
                });
        }

        function filterSelected() {
            vm.personnelFilter.CompanyIds = $.map(vm.companies, function (value, index) { return value.CompanyId });
            vm.personnelFilter.DepartmentIds = $.map(vm.departments, function (value, index) { return value.DepartmentId });
            vm.personnelFilter.TeamIds = $.map(vm.teams, function (value, index) { return value.TeamId });
            retrieveOrganisationalChart();
        }
    }
})();

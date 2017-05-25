(function () {
    'use strict';

    angular
        .module('HR')
        .controller('JobTitleController', JobTitleController);

    JobTitleController.$inject = ['$window', 'JobTitleService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function JobTitleController($window, JobTitleService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.jobTitles = [];
        vm.selectedjobTitles = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editJobTitle = editJobTitle;
        vm.isJobTitleMappingExists = isJobTitleMappingExists;
        vm.deleteJobTitle = deleteJobTitle;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrievejobTitles() {
            return JobTitleService.retrieveJobTitles(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.jobTitles = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.jobTitles;
                });
        }

        function isJobTitleMappingExists(id) {
            vm.loadingActions = true;
            vm.IsJobTitleMapped = true;
            JobTitleService.isJobTitleMappingExists(id).then(function (response) { vm.IsJobTitleMapped = response.data, vm.loadingActions = false });
        }

        function deleteJobTitle(id) {
            return JobTitleService.deleteJobTitle(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrievejobTitles();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrievejobTitles();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editJobTitle(id) {
            $window.location.href = "/JobTitle/Edit/" + id;            
        }
        
    }
})();

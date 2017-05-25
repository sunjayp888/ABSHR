(function () {
    'use strict';

    angular
        .module('HR')
        .controller('JobGradeController', JobGradeController);

    JobGradeController.$inject = ['$window', 'JobGradeService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function JobGradeController($window, JobGradeService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;        
        vm.jobGrades = [];
        vm.selectedJobGrades = [];
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.editJobGrade = editJobGrade;
        vm.isJobGradeMappingExists = isJobGradeMappingExists;
        vm.deleteJobGrade = deleteJobGrade;

        initialise();

        function initialise() {
            order("Name");
        }

        function retrieveJobGrades() {
            return JobGradeService.retrieveJobGrades(vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.jobGrades = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.jobGrades;
                });
        }

        function isJobGradeMappingExists(id) {
            vm.loadingActions = true;
            vm.IsJobGradeMapped = true;
            JobGradeService.isJobGradeMappingExists(id).then(function (response) { vm.IsJobGradeMapped = response.data, vm.loadingActions = false });
        }

        function deleteJobGrade(id) {
            return JobGradeService.deleteJobGrade(id)
              .then(function () {
                  initialise();
              });
        }

        function pageChanged() {
            return retrieveJobGrades();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveJobGrades();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editJobGrade(id) {
            $window.location.href = "/JobGrade/Edit/" + id;            
        }
        
    }
})();

(function () {
    'use strict';

    angular
        .module('HR')
        .controller('JobTitleJobGradeController', JobTitleJobGradeController);

    JobTitleJobGradeController.$inject = ['$window', '$filter', 'JobGradeService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function JobTitleJobGradeController($window, $filter, JobGradeService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.initialise = initialise;
        vm.assigning = false;
        vm.ddJobGradeTypes = [];
        vm.ddJobGradeType = 0;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.jobTitleId = 0;
        vm.orderClass = orderClass;
        vm.assigning = false;
        vm.jobGrades = [];
        vm.changeGradeType = changeGradeType;
        vm.retrieveUnassignedJobGradeTypes = retrieveUnassignedJobGradeTypes;
        vm.retrieveJobTitleJobGrades = retrieveJobTitleJobGrades;                
        vm.unassignJobTitleJobGrade = unassignJobTitleJobGrade;
        vm.IsJobTitleJobGradeMappingExist = IsJobTitleJobGradeMappingExist;        
        vm.assignJobTitleJobGrade = assignJobTitleJobGrade;
        vm.editJobGrade = editJobGrade;
        vm.loadingActions = false;
        vm.unassignJobTitleJobGradeClass = unassignJobTitleJobGradeClass;


        function initialise(jobTitleId) {            
            vm.jobTitleId = jobTitleId;
            order("Name").then(function () {
                retrieveJobTitleJobGrades();                
                retrieveUnassignedJobGradeTypes();                
            });
        }


        function retrieveUnassignedJobGradeTypes() {            
            return JobGradeService.retrieveUnassignedJobGrades(vm.jobTitleId)
               .then(function (response) {
                   vm.ddJobGradeTypes = response.data;
                   vm.ddJobGradeType = response.data[0];
                   vm.assigning = vm.ddJobGradeTypes.length == 0;                   
                   return vm.ddJobGradeTypes;
               });
        }

        function retrieveJobTitleJobGrades() {            
            return JobGradeService.retrieveJobTitleJobGrades(vm.jobTitleId,vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.jobGrades = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.jobGrades;
                });
        }
        function unassignJobTitleJobGradeClass(jobGrade) {
            return jobGrade.CanUnassign ? 'link-disabled' : '';
        }
        function IsJobTitleJobGradeMappingExist(jobGradeId) {
            vm.loadingActions = true;
            vm.jobGradeId = jobGradeId;
            JobGradeService.IsJobTitleJobGradeMappingExist(vm.jobTitleId, jobGradeId).then(function (response) {
                $filter('filter')(vm.jobGrades, { JobGradeId: vm.jobGradeId })[0]["CanUnassign"] = response.data;
                vm.loadingActions = false;
            });
           
        }
     
        
        function unassignJobTitleJobGrade(jobTitleGrade) {            
            //if (jobTitleGrade.jobGrades.CanUnassign) {
                return JobGradeService.unassignJobTitleJobGrade(vm.jobTitleId, jobTitleGrade.JobGradeId)
                    .then(function () {
                        initialise(vm.jobTitleId);
                    });
            }
         
        //}

       
 

        function assignJobTitleJobGrade() {            
            vm.assigning = true;
            
            return JobGradeService.assignJobTitleJobGrade(vm.jobTitleId, vm.ddJobGradeType.JobGradeId)
              .then(function () {
                  retrieveJobTitleJobGrades();
                  retrieveUnassignedJobGradeTypes();
              });
        }

        function changeGradeType(ddJobGradeType) {
            vm.ddJobGradeType = ddJobGradeType;
        }

        function pageChanged() {
            return retrieveUnassignedJobGradeTypes();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrieveUnassignedJobGradeTypes();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }

        function editJobGrade(id) {
            $window.location.href = "/JobGrade/Edit/" + id + "?returnUrl=" + $window.location.pathname;
        }
        
    }
})();

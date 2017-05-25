(function () {
    'use strict';

    angular
        .module('HR')
        .factory('JobGradeService', JobGradeService);

    JobGradeService.$inject = ['$http'];

    function JobGradeService($http) {
        var service = {
            retrieveJobGrades: retrieveJobGrades,
            retrieveUnassignedJobGrades: retrieveUnassignedJobGrades,
            retrieveJobTitleJobGrades : retrieveJobTitleJobGrades,
            assignJobTitleJobGrade: assignJobTitleJobGrade,
            unassignJobTitleJobGrade: unassignJobTitleJobGrade,
            isJobGradeMappingExists: isJobGradeMappingExists,
            IsJobTitleJobGradeMappingExist : IsJobTitleJobGradeMappingExist,
            deleteJobGrade: deleteJobGrade
        };

        return service;

        function retrieveJobGrades(Paging, OrderBy) {

            var url = "/JobGrade/List",
                data = {
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

        function retrieveUnassignedJobGrades(jobTitleId) {
            var url = "/JobTitle/UnassignedJobGrades",
                data = {
                    jobTitleId: jobTitleId
                };
            return $http.post(url, data);
        }

        function assignJobTitleJobGrade(jobTitleId, jobGradeId) {
            var url = "/JobTitle/assignJobTitleJobGrade",
                data = {
                    jobTitleId: jobTitleId,
                    jobGradeId: jobGradeId
                };

            return $http.post(url, data);
        }

        function retrieveJobTitleJobGrades(jobTitleId) {            
            var url = "/JobTitle/JobTitleJobGrades",
                data = {
                    jobTitleId: jobTitleId
                };

            return $http.post(url, data);
        }

        function unassignJobTitleJobGrade(jobTitleId, jobGradeId) {            
            var url = "/JobTitle/UnassignJobTitleJobGrade",
                data = {
                    jobTitleId: jobTitleId,
                    jobGradeId: jobGradeId
                };
            return $http.post(url, data);
        }
      
        function deleteJobGrade(id) {
            var url = "/JobGrade/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }
        
        function isJobGradeMappingExists(id) {
            var url = "/JobGrade/CanDeleteJobGrade",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

        function IsJobTitleJobGradeMappingExist(jobTitleId, jobGradeId) {            
            var url = "/JobTitle/CanDeleteJobTitleJobGrade",
              data = {
                  jobTitleId: jobTitleId,
                  jobGradeId: jobGradeId
              };
            return $http.post(url, data);
        }

    }
})();
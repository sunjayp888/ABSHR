(function () {
    'use strict';

    angular
        .module('HR')
        .controller('JobTitleDocumentController', JobTitleDocumentController);

    JobTitleDocumentController.$inject = ['$window', 'JobTitleDocumentService', 'Paging'];

    function JobTitleDocumentController($window, JobTitleDocumentService, Paging) {
        /* jshint validthis:true */
        var vm = this;        
        vm.jobTitleDocuments = [];
        vm.jobTitleId;
        vm.name;
        vm.documentFile;
        vm.IsJobTitleDocumentMapped;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.Errors = [];
        vm.createJobTitleDocument = createJobTitleDocument;
        vm.removeError = removeError;
        //vm.editJobTitleDocument = editJobTitleDocument;
        vm.deleteJobTitleDocument = deleteJobTitleDocument;
        vm.initialise = initialise;

        function initialise(jobTitleId) {
            vm.jobTitleId = jobTitleId;
            retrieveJobTitleDocuments();
        }

        function createJobTitleDocument() {
            vm.Errors = [];
            if (vm.documentFile !== undefined && vm.name !== undefined) {
                var documentName = vm.documentFile.name;
                var documentByteString;
                var reader = new FileReader();
                return JobTitleDocumentService.createJobTitleDocument(vm.jobTitleId, vm.name, vm.documentFile).then(function (response) {
                    if (response.data.length === 0) {
                        $("#jobTitleDocumentModal").modal('hide');
                        vm.documentFile = null;
                        vm.name = "";
                        retrieveJobTitleDocuments();
                    }
                    else {
                        vm.Errors = response.data;
                    }
                });
            }
            else {
                if (vm.name === undefined) {
                    vm.Errors.push('File name required');
                }
                if (vm.documentFile === undefined) {
                    vm.Errors.push('No file available');
                }
            }
        }

        function removeError() {
            vm.Errors.length = 0;
        }

        function retrieveJobTitleDocuments() {
            return JobTitleDocumentService.retrieveJobTitleDocuments(vm.jobTitleId, vm.paging)
                .then(function (response) {
                    vm.jobTitleDocuments = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;                    
                    return vm.jobTitleDocuments;
                });
        }

        function deleteJobTitleDocument(id) {
            return JobTitleDocumentService.deleteJobTitleDocument(id)
              .then(function () {
                  retrieveJobTitleDocuments();
              });
        }

        function pageChanged() {
            return retrieveJobTitleDocuments();
        }
                
    }
})();

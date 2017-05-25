(function () {
    'use strict';

    angular
        .module('HR')
        .factory('JobTitleDocumentService', JobTitleDocumentService);

    JobTitleDocumentService.$inject = ['$http'];

    function JobTitleDocumentService($http) {
        var service = {
            createJobTitleDocument: createJobTitleDocument,
            retrieveJobTitleDocuments: retrieveJobTitleDocuments,
            deleteJobTitleDocument: deleteJobTitleDocument
        };

        return service;

        function createJobTitleDocument(jobTitleId, name, attachment) {

            var url = "/JobTitleDocument/Create",
                data = {
                    JobTitleId: jobTitleId,
                    Name: name,
                    Attachment: attachment
                };

            //return $http.post(url, data);

            var getModelAsFormData = function (data) {
                var dataAsFormData = new FormData();
                angular.forEach(data, function (value, key) {
                    dataAsFormData.append(key, value);
                });
                return dataAsFormData;
            };

            return $http({
                url: url,
                method: "POST",
                data: getModelAsFormData(data),
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            });
        }

        function retrieveJobTitleDocuments(jobTitleId, Paging) {

            var url = "/JobTitleDocument/List",
                data = {
                    id: jobTitleId,
                    paging: Paging
                };

            return $http.post(url, data);
        }

        function deleteJobTitleDocument(id) {
            var url = "/JobTitleDocument/Delete",
              data = {
                  id: id
              };
            return $http.post(url, data);
        }

    }
})();
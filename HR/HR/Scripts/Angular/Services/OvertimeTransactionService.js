(function () {
    'use strict';

    angular
        .module('HR')
        .factory('OvertimeTransactionService', OvertimeTransactionService);

    OvertimeTransactionService.$inject = ['$http'];

    function OvertimeTransactionService($http) {
        var service = {
            retrieveOvertimeTransactions: retrieveOvertimeTransactions
        };

        return service;

        function retrieveOvertimeTransactions(OvertimeFilter, Paging, OrderBy) {

            var url = "/OvertimeTransaction/List",
                data = {
                    overtimeFilter: OvertimeFilter,
                    paging: Paging,
                    orderBy: new Array(OrderBy)
                };

            return $http.post(url, data);
        }

    }
})();
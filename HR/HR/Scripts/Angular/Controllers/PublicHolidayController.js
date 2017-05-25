(function () {
    'use strict';

    angular
        .module('HR')
        .controller('PublicHolidayController', PublicHolidayController);

    PublicHolidayController.$inject = ['$window', 'PublicHolidayPolicyService', 'Paging', 'OrderService', 'OrderBy', 'Order'];

    function PublicHolidayController($window, PublicHolidayPolicyService, Paging, OrderService, OrderBy, Order) {
        /* jshint validthis:true */
        var vm = this;
        vm.initialise = initialise;
        vm.PublicHolidays = [];
        vm.isCreate = true;
        vm.paging = new Paging;
        vm.pageChanged = pageChanged;
        vm.orderBy = new OrderBy;
        vm.order = order;
        vm.orderClass = orderClass;
        vm.changeYear = changeYear;
        vm.year = moment().year();
        vm.years = [moment().year() - 1, moment().year(), moment().year() + 1];
        vm.savePublicHoliday = savePublicHoliday;
        vm.deletePublicHoliday = deletePublicHoliday;
        vm.openPublicHolidayModal = openPublicHolidayModal;
        vm.editPublicHolidayModal = editPublicHolidayModal;


        function initialise(publicHolidayPolicyId) {
            vm.publicHolidayPolicyId = publicHolidayPolicyId;
            order("Name");
        }

        function retrievePublicHolidays() {
            return PublicHolidayPolicyService.retrievePublicHolidays(vm.publicHolidayPolicyId, vm.year, vm.paging, vm.orderBy)
                .then(function (response) {
                    vm.PublicHolidays = response.data.Items;
                    vm.paging.totalPages = response.data.TotalPages;
                    vm.paging.totalResults = response.data.TotalResults;
                    return vm.PublicHolidays;
                });
        }

        function savePublicHoliday(action) {
            var data = {
                PublicHolidayPolicyId: vm.publicHolidayPolicyId,
                PublicHolidayId: vm.publicHolidayId,
                Name: vm.name,
                Date: vm.date
            };
            return PublicHolidayPolicyService.savePublicHoliday(action, data)
               .then(function (response) {
                   if (response.data === '' || response.data.Succeeded === true) {
                       $("#publicHolidayModal").modal('hide');
                       initialise(vm.publicHolidayPolicyId);
                   } else {
                       $('#publicHolidayErrorSummary').show();
                       vm.Errors = response.data;
                   }
               });
        }

        function deletePublicHoliday(publicHolidayId) {
            return PublicHolidayPolicyService.deletePublicHoliday(publicHolidayId)
              .then(function () {
                  retrievePublicHolidays();
              });
        }

        function editPublicHolidayModal(id) {
            vm.isCreate = false;
            var publicHoliday = vm.PublicHolidays.filter(function (obj) { return obj.PublicHolidayId === id; });
            $('#publicHolidayErrorSummary').hide();
            $("#publicHolidayModal").modal('show');
            vm.name = publicHoliday[0].Name;
            vm.date = moment(publicHoliday[0].Date).format("DD MMMM YYYY");
            vm.publicHolidayId = id;
        }

        function openPublicHolidayModal() {
            $('#publicHolidayErrorSummary').hide();
            $("#PublicHoliday_Name").val('');
            $("#publicHolidayModal").modal('show');
            vm.isCreate = true;
        }

        function changeYear(year) {
            vm.year = year;
            retrievePublicHolidays();
        }

        function pageChanged() {
            return retrievePublicHolidays();
        }

        function order(property) {
            vm.orderBy = OrderService.order(vm.orderBy, property);
            return retrievePublicHolidays();
        }

        function orderClass(property) {
            return OrderService.orderClass(vm.orderBy, property);
        }
    }
})();

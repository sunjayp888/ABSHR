(function (jQuery) {
    'use strict';

    angular
        .module('HR')
        .controller('dailyScheduleController', dailyScheduleController);

    dailyScheduleController.$inject = ['$scope', 'moment'];

    function dailyScheduleController($scope, moment) {
        var vm = this;

        vm.dateLabel = null;
        vm.days = [];
        vm.days = [];
        vm.retrieveAbsenceClass = retrieveAbsenceClass;
        vm.changeDate = changeDate;

        $scope.$watch(
            function (scope) {
                return { beginDate: scope.vm.beginDate, items: scope.vm.items };
            },
            function (newValue, oldValue) {
                if (newValue && newValue.beginDate && newValue.items)
                    createDays();
            }, true);

        function createDays() {
            vm.days = retrieveDays(vm.beginDate);
        }

        function retrieveDays(beginDate) {
            var date = new moment(beginDate),
                days = [];

            vm.dateLabel = new moment(beginDate).add(4, 'd').format('MMMM YYYY');

            for (var i = 1; i <= 7; i++) {
                var day = {
                    name: date.format('ddd'),
                    number: date.format('Do'),
                    isDisplayMonth: vm.dateLabel === date.format('MMMM YYYY')
                };
                days.push(day);
                date.add(1, 'd');
            }

            return days;
        }

        function retrieveAbsenceClass(slot) {
            var absenceClass = [],
                isHalf = slot.days % 1 !== 0;

            if (slot.isAbsence)
                absenceClass.push("absence");
            else
                absenceClass.push("publicholiday");


            absenceClass.push(slot.cssClass);
            //if (Math.floor(slot.days) == 0)
            //    absenceClass.push("absence-days" + (isHalf ? "-half" : ""));
            //else
            absenceClass.push("absence-days-" + Math.floor(slot.days) + (isHalf ? "-half" : ""));

            absenceClass.push("absence-begins-" + slot.begins + "-" + (slot.beginsPM ? "pm" : "am"));

            if (slot.overflowsBefore)
                absenceClass.push("absence-overflows-before");

            if (slot.overflowsAfter)
                absenceClass.push("absence-overflows-after");

            if (slot.status !== null)
                //absenceClass.push(slot.status.name.toLowerCase());
                absenceClass.push(slot.status.name.toLowerCase());
            else
                absenceClass.push("Holiday");

            return absenceClass.join(" ");
        }

        function changeDate(days) {
            vm.beginDate = new moment(vm.beginDate).add(days, 'd').format();
            vm.dateChanged(vm.beginDate);
        }

    }

    jQuery(function () {
        jQuery(document).on("click", ".absence .dropdown-toggle", function () {
            var btn = jQuery(this);
            dropDownFixPosition(btn, btn.siblings(".dropdown-menu"));
        });

        jQuery(window).on("load scroll resize", function () {
            jQuery(".absence .dropdown-menu").each(function () {
                var dropdown = jQuery(this);
                dropDownFixPosition(dropdown.siblings(), dropdown);
            });
        });
    });

    function dropDownFixPosition(button, dropdown) {
        var dropDownTop = button[0].getBoundingClientRect().top + button.outerHeight();
        dropdown.css('top', dropDownTop + "px");
        dropdown.css('left', button[0].getBoundingClientRect().left + "px");
    }

})(window.jQuery);

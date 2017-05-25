(function () {
    'use strict';

    angular
        .module('HR')
        .directive('dailyScheduleLegend', dailyScheduleLegend);

    function dailyScheduleLegend() {
        // Usage:
        //     <daily-schedule-legend></daily-schedule-legend>
        // Creates:
        // 

        var directive = {
            templateUrl: '/Scripts/Angular/Directives/Views/_scheduleLegend.html',
            restrict: 'E',
            controller: 'dailyScheduleLegendController',
            controllerAs: 'vm',            
            bindToController: {
                items: '='
            },
            transclude: true
        };

        return directive;

    }

})();
(function () {
    'use strict';

    angular
        .module('HR')
        .filter('Select', Select);
    
    function Select() {
        return SelectFilter;

        function SelectFilter(input, propName) {
            var out = [];

            if (angular.isArray(input)) {

                input.forEach(function (item) {
                    out.push(item[propName]);
                });
            } else {
                
                out = input[propName];
            }

            return out;
        }
    }
})();
(function () {
    'use strict';

    angular
        .module('HR')
        .controller('TileController', TileController);

    TileController.$inject = ['$window', '$location', '$anchorScroll'];

    function TileController($window, $location, $anchorScroll) {
        /* jshint validthis:true */
        var vm = this;
        vm.scroll = scroll;

        function scroll(location) {
            $location.hash(location);
            $anchorScroll();
        }
    }
})();

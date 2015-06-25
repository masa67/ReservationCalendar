
/*global angular */
(function () {
    'use strict';

    angular
        .module('Locale', [])
        .factory('locale', function () {

            return {
                getLang: function () {
                    return 'en-gb';
                }
            };
        });
}());


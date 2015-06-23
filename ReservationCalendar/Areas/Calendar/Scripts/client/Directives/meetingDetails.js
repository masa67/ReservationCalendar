
/*global angular, $ */
(function () {
    'use strict';

    angular
        .module('MeetingDetails', [])
        .directive('meetingDetails', ['globalDialogs', function (globalDialogs) {
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/meetingDetails.html',
                link: function (scope, elem) {
                    var modalEl = $('#meeting-details');

                    scope.id = 10;

                    //globalDialogs.add
                }
            };
        }]);
}());
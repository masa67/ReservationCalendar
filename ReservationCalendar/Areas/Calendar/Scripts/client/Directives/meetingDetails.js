
/*global app */
app.directive('meetingDetails', [ function () {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/meetingDetails.html',
        link: function (scope, elem) {
            scope.id = 10;
        }
    };
}]);


app.controller('CalendarCtrl', ['$scope', function ($scope) {
}])
.directive('reservationCalendar', function () {
    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem, attr) {
        }
    };
});
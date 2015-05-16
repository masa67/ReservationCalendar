
app.controller('CalendarCtrl', ['$scope', function ($scope) {
}])
.directive('reservationCalendar', function ($window, $resource) {
    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem, attr) {

            if (!jQuery().fullCalendar) {
                return;
            }

            var date = new Date();
            var d = date.getDate();
            var m = date.getMonth();
            var y = date.getFullYear();

            var h = {};

            var child = elem.find('.calendar');
            var calBody = angular.element(elem.find('.calendar-body'));
            var calBody2 = $('.calendar-body');

            var calEvents = [];

            var RBook = $resource('/ReservationBookAbs/Details/0?data=true');
            scope.rBook = RBook.get(function () {
                var i, tSlots = scope.rBook.combinedCalendar.timeSlots, tSlot, ts;

                calEvents = [];
                for (i = 0; i < tSlots.length; i++) {
                    tSlot = tSlots[i];

                    ts = {
                        title: tSlot.description,
                        start: tSlot.startTime,
                        backgroundColor:
                            (tSlot.timeSlotStatus === 2)
                                ? Metronic.getBrandColor('green')
                                : Metronic.getBrandColor('yellow'),
                    };

                    if (tSlot.fullDay) {
                        ts.allDay = true;
                    } else {
                        ts.allDay = false;
                        ts.end = tSlot.endTime
                    }

                    calEvents.push(ts);
                }

                redrawCal();
            });
            
            scope.isMobile = false;

            scope.$watch(
                function () {
                    return child[0].offsetWidth;
                },
                redrawCal
            );

            angular.element($window).bind('resize', function () {
                scope.$apply();
            });

            function redrawCal(width) {
                if (width) {
                    if (Metronic.isRTL()) {
                        if (width <= 720) {
                            scope.isMobile = true;
                            h = {
                                right: 'title, prev, next',
                                center: '',
                                left: 'agendaDay, agendaWeek, month, today'
                            };
                        } else {
                            scope.isMobile = false;
                            h = {
                                right: 'title',
                                center: '',
                                left: 'agendaDay, agendaWeek, month, today, prev,next'
                            };
                        }
                    } else {
                        if (width <= 720) {
                            scope.isMobile = true;
                            h = {
                                left: 'title, prev, next',
                                center: '',
                                right: 'today,month,agendaWeek,agendaDay'
                            };
                        } else {
                            scope.isMobile = false;
                            h = {
                                left: 'title',
                                center: '',
                                right: 'prev,next,today,month,agendaWeek,agendaDay'
                            };
                        }
                    }
                }

                calBody.fullCalendar('destroy'); // destroy the calendar
                calBody.fullCalendar({ //re-initialize the calendar
                    header: h,
                    defaultView: 'month', // change default view with available options from http://arshaw.com/fullcalendar/docs/views/Available_Views/ 
                    slotMinutes: 15,
                    editable: true,
                    events: calEvents

                        /*
                        [{
                        title: 'All Day Event',
                        start: new Date(y, m, 1),
                        backgroundColor: Metronic.getBrandColor('yellow')
                    }, {
                        title: 'Long Event',
                        start: new Date(y, m, d - 5),
                        end: new Date(y, m, d - 2),
                        backgroundColor: Metronic.getBrandColor('green')
                    }, {
                        title: 'Repeating Event',
                        start: new Date(y, m, d - 3, 16, 0),
                        allDay: false,
                        backgroundColor: Metronic.getBrandColor('red')
                    }, {
                        title: 'Repeating Event',
                        start: new Date(y, m, d + 4, 16, 0),
                        allDay: false,
                        backgroundColor: Metronic.getBrandColor('green')
                    }, {
                        title: 'Meeting',
                        start: new Date(y, m, d, 10, 30),
                        allDay: false,
                    }, {
                        title: 'Lunch',
                        start: new Date(y, m, d, 12, 0),
                        end: new Date(y, m, d, 14, 0),
                        backgroundColor: Metronic.getBrandColor('grey'),
                        allDay: false,
                    }, {
                        title: 'Birthday Party',
                        start: new Date(y, m, d + 1, 19, 0),
                        end: new Date(y, m, d + 1, 22, 30),
                        backgroundColor: Metronic.getBrandColor('purple'),
                        allDay: false,
                    }, {
                        title: 'Click for Google',
                        start: new Date(y, m, 28),
                        end: new Date(y, m, 29),
                        backgroundColor: Metronic.getBrandColor('yellow'),
                        url: 'http://google.com/',
                    }]
                        */
                });
            }
        }
    };
});
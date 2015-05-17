
/*global angular, app, calHelpers, calTemplHelpers, jQuery, Metronic */
app.directive('reservationCalendar', function ($window, $resource) {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem) {

            var allTSlots = {},
                child = elem.find('.calendar'),
                calBody = angular.element(elem.find('.calendar-body')),
                calEvents = [],
                combCal = {},
                h = {},
                RBook = $resource('/ReservationBookAbs/Details/0?data=true');

            function redrawCal(width) {
                if (width) {
                    if (Metronic.isRTL()) {
                        h = {
                            right: 'title',
                            center: '',
                            left: 'agendaDay, agendaWeek, month, today, prev, next'
                        };
                        scope.isMobile = (width <= 730) ? true : false;
                    } else {
                        h = {
                            left: 'title',
                            center: '',
                            right: 'prev, next, today,month,agendaWeek,agendaDay'
                        };
                        scope.isMobile = (width <= 730) ? true : false;
                    }
                }

                calBody.fullCalendar('destroy'); // destroy the calendar
                calBody.fullCalendar({ //re-initialize the calendar
                    header: h,
                    defaultView: 'month', // change default view with available options from http://arshaw.com/fullcalendar/docs/views/Available_Views/
                    slotMinutes: 15,
                    editable: true,
                    events: calEvents
                });
            }

            function updateCalEvents() {
                var i, tSlots, tSlot, ts;

                if (scope.model.calMode === 'combined') {
                    tSlots = combCal.timeSlots;
                } else {
                    tSlots = allTSlots;
                }

                calEvents = [];
                for (i = 0; i < tSlots.length; i += 1) {
                    tSlot = tSlots[i];

                    ts = {
                        title: tSlot.description,
                        start: tSlot.startTime,
                        backgroundColor:
                            (tSlot.timeSlotStatus === calHelpers.TimeSlotStatus.EXCLUDED) ?
                                    Metronic.getBrandColor('green') :
                                    Metronic.getBrandColor('yellow')
                    };

                    if (tSlot.fullDay) {
                        ts.allDay = true;
                    } else {
                        ts.allDay = false;
                        ts.end = tSlot.endTime;
                    }

                    calEvents.push(ts);
                }

                redrawCal();
            }

            function watchWidthChanges() {
                scope.$watch(
                    function () {
                        return child[0].offsetWidth;
                    },
                    redrawCal
                );

                angular.element($window).bind('resize', function () {
                    scope.$apply();
                });
            }

            scope.model = scope.model || {};
            scope.rBook = RBook.get(function () {
                combCal = calTemplHelpers.createCombinedCalTempl(scope.rBook.calendarLayers);
                allTSlots = calTemplHelpers.listAllTimeSlots(scope.rBook.calendarLayers);
                updateCalEvents();
            });

            scope.model.calMode = 'combined';
            scope.changeCalMode = function () {
                updateCalEvents();
            };

            scope.isMobile = false;
            watchWidthChanges();
        }
    };
})
    .directive('reservationCalendarControls', function () {
        'use strict';

        return {
            restrict: 'E',
            templateUrl: '/Areas/Calendar/Templates/reservationCalendarControls.html',
        };
    });

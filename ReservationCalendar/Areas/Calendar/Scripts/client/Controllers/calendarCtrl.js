
/*global angular, app, calHelpers, calTemplHelpers, jQuery, Metronic */
app.directive('reservationCalendar', function ($window, $resource) {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem) {

            var child = elem.find('.calendar'),
                calBody = angular.element(elem.find('.calendar-body')),
                calEvents = [],
                h = {},
                RBook = $resource('/ReservationBookAbs/Details/0?data=true');

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
                });
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

            if (!jQuery().fullCalendar) {
                return;
            }

            scope.rBook = RBook.get(function () {
                var combCal, i, tSlots, tSlot, ts;

                // tSlots = scope.rBook.combinedCalendar.timeSlots;
                combCal = calTemplHelpers.createCombinedCalTempl(scope.rBook.calendarLayers);
                tSlots = combCal.timeSlots;

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
            });

            scope.isMobile = false;

            watchWidthChanges();
        }
    };
});
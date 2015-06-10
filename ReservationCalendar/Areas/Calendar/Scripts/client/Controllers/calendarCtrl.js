
/*global alert, angular, app, calHelpers, calTemplHelpers, jQuery, Metronic, moment, timeSlotHelpers */
app.directive('reservationCalendar', function ($window, $resource) {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem) {

            var aTS,
                child = elem.find('.calendar'),
                calBody = angular.element(elem.find('.calendar-body')),
                calEvents = [],
                combCal = {},
                fcState = {
                    view: 'agendaWeek'
                },
                h = {},
                RBook = $resource('/ReservationBookAbs/Details/0?data=true');

            function isOverlapping(aTS) {
                var bTS, calEv = calBody.fullCalendar('clientEvents'), ev, i;

                for (i = 0; i < calEv.length; i += 1) {
                    ev = calEv[i];
                    if (!aTS.id || ev.id !== aTS.id) {
                        bTS = {
                            fullDay: !ev.start.hasTime(),
                            startTime: ev.start.unix(),
                            endTime: ev.end && ev.end.unix()
                        };
                        if (timeSlotHelpers.checkOverlap(aTS, bTS) !== calHelpers.TimeSlotOverlap.NONE) {
                            return true;
                        }
                    }
                }
                return false;
            }

            function fcRedraw(width) {
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
                    allDaySlot: false,
                    defaultDate: moment('2015-05-15'),
                    defaultView: fcState.view,
                    editable: true,
                    eventDrop: function (ev, delta, revertFunc, jsEv, ui, view) {
                        alert('event dropped');
                    },
                    eventResize: function (ev, delta, revertFunc, jsEv, ui, view) {
                        console.log(ev);
                        console.log(delta);
                        console.log(view);
                        alert('event resized');
                    },
                    events: calEvents,
                    firstDay: 1,
                    header: h,
                    selectable: true,
                    selectHelper: true,
                    select: function (start, end) {
                        aTS = {
                            fullDay: !start.hasTime(),
                            startTime: start.unix(),
                            endTime: end.unix()
                        };

                        if (!isOverlapping(aTS)) {
                            calBody.fullCalendar('renderEvent',
                                {
                                    title: '',
                                    start: start,
                                    end: end,
                                    allDay: false,
                                    fullDay: !start.hasTime()
                                },
                                true // make the event "stick"
                                );
                        }
                        calBody.fullCalendar('unselect');
                    },
                    slotMinutes: 15,
                    viewRender: function (view) {
                        fcState.view = view.name;
                    },
                    weekends: false
                });
            }

            /*
            function fcUpdateEvents() {
                calBody.fullCalendar({
                    events: calEvents
                });
                calBody.fullCalendar('rerenderEvents');
            }
            */

            function updateCalEvents() {
                var i, tSlots, tSlot, ts;

                if (scope.model.calMode === 'combined') {
                    tSlots = combCal.timeSlots;
                } else {
                    tSlots = calTemplHelpers.listAllTimeSlots(
                        scope.rBook.calendarLayers,
                        scope.model.calendarLayerSelected
                    );
                }

                calEvents = [];
                for (i = 0; i < tSlots.length; i += 1) {
                    tSlot = tSlots[i];

                    ts = {
                        allDay: false,
                        fullDay: tSlot.fullDay,
                        title: tSlot.description,
                        start: moment(1000 * tSlot.startTime).format(),
                        backgroundColor:
                            (tSlot.timeSlotStatus === calHelpers.TimeSlotStatus.EXCLUDED) ?
                                    Metronic.getBrandColor('green') :
                                    Metronic.getBrandColor('yellow')
                    };

                    if (tSlot.fullDay) {
                        ts.end = moment(1000 * tSlot.startTime).add(1, 'days').subtract(1, 'seconds').format();
                    } else {
                        ts.end = moment(1000 * tSlot.endTime).format();
                    }

                    calEvents.push(ts);
                }

                fcRedraw(); // fcUpdateEvents();
            }

            function watchWidthChanges() {
                scope.$watch(
                    function () {
                        return child[0].offsetWidth;
                    },
                    fcRedraw
                );

                angular.element($window).bind('resize', function () {
                    scope.$apply();
                });
            }

            scope.model = scope.model || {};
            scope.model.calendarLayerSelected = [];
            scope.model.editArea = "freeSlots";

            scope.rBook = RBook.get(function () {
                var i, sel;

                calTemplHelpers.sortCalByWeight(scope.rBook.calendarLayers);
                sel = scope.model.calendarLayerSelected;
                sel.length = 0;
                for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                    sel.push(true);
                }

                combCal = calTemplHelpers.createCombinedCalTempl(scope.rBook.calendarLayers);
                updateCalEvents();
            });

            scope.model.calMode = 'combined';

            scope.changeCalLayers = function () {
                updateCalEvents();
            };

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
            templateUrl: '/Areas/Calendar/Templates/reservationCalendarControls.html'
        };
    });

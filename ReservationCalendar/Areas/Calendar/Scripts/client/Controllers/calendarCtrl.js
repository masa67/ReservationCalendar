
/*global alert, angular, app, calHelpers, calTemplHelpers, fullCalendarHelpers, jQuery, Metronic, moment, timeSlotHelpers */
app.directive('reservationCalendar', [ '$window', '$resource', 'rBook', function ($window, $resource, rBook) {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem) {

            var aTS,
                child = elem.find('.calendar'),
                calBody = angular.element(elem.find('.calendar-body')),
                calEvents = [],
                calTplNdx = 2, // maps to editArea 'freeSlots'
                combCal = {},
                fcState = {
                    view: 'agendaWeek'
                },
                h = {},
                origTS,
                RBook = rBook.getRBook(0),
                tSlotsToEdit;

            fullCalendarHelpers.init(calBody);

            function combineAdjWRefresh(aTS) {
                if (timeSlotHelpers.combineAdjacent(aTS, tSlotsToEdit)) {
                    var calTpl = scope.rBook.calendarLayers[calTplNdx],
                        absCalTpl;

                    if (calTpl.calendarDbType !== calHelpers.CalendarDbType.ABSOLUTE) {
                        throw new Error('Handling other than absolute calendars unimplemented');
                    }

                    absCalTpl = {
                        ID: calTpl.dbCalendarTemplateID,
                        Description: calTpl.description,
                        absTimeSlots: calTpl.timeSlots
                    };

                    rBook.saveCalTempl(absCalTpl).then(
                        function () {
                            recalcCalContent();
                        },
                        function () {
                            alert('nok');
                        }
                    );
                }
            }

            function isOverlapping(aTS) {
                return timeSlotHelpers.isOverlapping(aTS, fullCalendarHelpers.eventsToTS());
            }

            function eventMod(ev, delta, revertFunc) {
                /*jslint nomen: true */
                aTS = {
                    id: ev._id,
                    startTime: ev.start.unix(),
                    endTime: ev.end.unix(),
                    origTSlot: ev.origTSlot
                };

                if (isOverlapping(aTS)) {
                    revertFunc();
                } else {
                    ev.origTSlot.startTime = aTS.startTime;
                    ev.origTSlot.endTime = aTS.endTime;
                    combineAdjWRefresh(aTS);
                }
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
                    eventDrop: function (ev, delta, revertFunc) {
                        eventMod(ev, delta, revertFunc);
                    },
                    eventResize: function (ev, delta, revertFunc) {
                        eventMod(ev, delta, revertFunc);
                    },
                    events: calEvents,
                    firstDay: 1,
                    header: h,
                    selectable: true,
                    selectHelper: true,
                    select: function (start, end) {
                        aTS = {
                            startTime: start.unix(),
                            endTime: end.unix()
                        };

                        if (!isOverlapping(aTS)) {
                            origTS = {
                                startTime: start.unix(),
                                endTime: end.unix(),
                                timeSlotStatus: calHelpers.TimeSlotStatus.FREE
                            };
                            tSlotsToEdit.push(origTS);
                            aTS.origTSlot = origTS;

                            if (timeSlotHelpers.combineAdjacent(aTS, tSlotsToEdit)) {
                                recalcCalContent();
                            } else {
                                calBody.fullCalendar('renderEvent',
                                    {
                                        allDay: false,
                                        end: end,
                                        start: start,
                                        title: '',
                                        origTSlot: aTS.origTSlot
                                    },
                                    true // make the event "stick"
                                    );
                            }
                        }
                        calBody.fullCalendar('unselect');
                    },
                    slotMinutes: 15,
                    timezone: 'local',
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

            function recalcCalContent() {
                combCal = calTemplHelpers.createCombinedCalTempl(scope.rBook.calendarLayers);
                updateCalEvents();
            }

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
                        backgroundColor:
                            (tSlot.timeSlotStatus === calHelpers.TimeSlotStatus.EXCLUDED) ?
                                    Metronic.getBrandColor('green') :
                                    Metronic.getBrandColor('yellow'),
                        end: moment(1000 * tSlot.endTime).format(),
                        start: moment(1000 * tSlot.startTime).format(),
                        title: tSlot.description,
                        origTSlot: tSlot.origTSlot // || tSlot for listAllTimeSlots
                    };

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

                recalcCalContent();

                // FFS: misplaced
                tSlotsToEdit = scope.rBook.calendarLayers[calTplNdx].timeSlots;
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
}])
    .directive('reservationCalendarControls', function () {
        'use strict';

        return {
            restrict: 'E',
            templateUrl: '/Areas/Calendar/Templates/reservationCalendarControls.html'
        };
    });

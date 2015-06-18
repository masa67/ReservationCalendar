﻿
/*global alert, angular, app, calHelpers, calTemplHelpers, fullCalendarHelpers, jQuery, Metronic, moment, timeSlotHelpers */
app.directive('reservationCalendar', [ 'rBook', function (rBook) {
    'use strict';

    return {
        restrict: 'E',
        templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
        link: function (scope, elem) {

            var aTS,
                // child = elem.find('.calendar'),
                calBody = angular.element(elem.find('.calendar-body')),
                calEvents = [],
                combCal = {},
                fcState = {
                    view: 'agendaWeek'
                },
                getCalEvents, // function
                h = {},
                maxEditTime = 0,
                minEditTime = 0,
                origTS,
                recalcCalContent, // function
                saveCalLayerDB, // function
                tSlotsDeleted = [],
                tSlotsToEdit,
                updateCalEvents, // function
                updateEditTimes, // function
                useLazyFetching = true;
                // watchWidthChanges; // function

            fullCalendarHelpers.init(calBody);

            function combineAdjWRefresh(aTS) {
                var combRes = timeSlotHelpers.combineAdjacent(aTS, tSlotsToEdit);

                updateEditTimes(combRes.minTime, combRes.maxTime);
                tSlotsDeleted = combRes.delArr;

                saveCalLayerDB();
            }

            function isOverlapping(aTS) {
                return timeSlotHelpers.isOverlapping(aTS, fullCalendarHelpers.eventsToTS());
            }

            /*jslint unparam: true */
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
                    updateEditTimes(
                        Math.min(ev.origTSlot.startTime, aTS.startTime),
                        Math.max(ev.origTSlot.endTime, aTS.endTime)
                    );
                    ev.origTSlot.startTime = aTS.startTime;
                    ev.origTSlot.endTime = aTS.endTime;
                    combineAdjWRefresh(aTS);
                }
            }

            function fcRedraw(width) {
                if (Metronic.isRTL()) {
                    h = {
                        right: 'title',
                        center: '',
                        left: 'agendaDay, agendaWeek, month, today, prev, next'
                    };
                } else {
                    h = {
                        left: 'title',
                        center: '',
                        right: 'prev, next, today,month,agendaWeek,agendaDay'
                    };
                }

                scope.isMobile = (width && width <= 730) ? true : false;

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
                    events: getCalEvents,
                    firstDay: 1,
                    header: h,
                    lazyFetching: useLazyFetching,
                    selectable: true,
                    selectHelper: true,
                    select: function (start, end) {
                        aTS = {
                            startTime: start.unix(),
                            endTime: end.unix()
                        };

                        if (!isOverlapping(aTS)) {
                            origTS = {
                                calDbType: scope.rBook.calendarLayers[scope.model.layerInEdit].calendarDbType,
                                calDbId: scope.rBook.calendarLayers[scope.model.layerInEdit].dbCalendarTemplateID,
                                startTime: start.unix(),
                                endTime: end.unix(),
                                timeSlotStatus: calHelpers.TimeSlotStatus.FREE
                            };
                            updateEditTimes(origTS.startTime, origTS.endTime);
                            tSlotsToEdit.push(origTS);
                            aTS.origTSlot = origTS;

                            combineAdjWRefresh(aTS);
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

            function fcRefreshEvents() {
                calBody.fullCalendar('removeEvents');
                calBody.fullCalendar('addEventSource', calEvents);
            }

            getCalEvents = function (start, end, timezone, callback) {
                rBook.getRBook(1, start.unix(), end.unix()).then(
                    function (ret) {
                        var i, sel;

                        if (!scope.rBook || !useLazyFetching) {
                            scope.rBook = ret;
                        } else {
                            calTemplHelpers.merge(scope.rBook, ret);
                        }

                        calTemplHelpers.sortCalByWeight(scope.rBook.calendarLayers);
                        sel = scope.model.calendarLayerSelected;
                        sel.length = 0;
                        for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                            sel.push(true);
                        }

                        recalcCalContent();

                        // FFS: misplaced
                        tSlotsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;

                        calBody.fullCalendar('removeEvents');
                        callback(calEvents);
                    },
                    function (err) {
                        throw new Error('Reading cal events fails');
                    }
                );
            };

            recalcCalContent = function () {
                combCal = calTemplHelpers.createCombinedCalTempl(scope.rBook.calendarLayers);
                updateCalEvents();
            };

            saveCalLayerDB = function () {
                var calTpl = scope.rBook.calendarLayers[scope.model.layerInEdit], calTplEditReq, delTimeSlots = [], i;

                if (calTpl.calendarDbType !== calHelpers.CalendarDbType.ABSOLUTE) {
                    throw new Error('Handling other than absolute calendars unimplemented');
                }

                for (i = 0; i < tSlotsDeleted.length; i += 1) {
                    if (tSlotsDeleted[i].dbId) {
                        delTimeSlots.push({
                            dbId: tSlotsDeleted[i].dbId,
                            rowVersion: tSlotsDeleted[i].rowVersion
                        });
                    }
                    timeSlotHelpers.delete(tSlotsDeleted[i], tSlotsToEdit);
                }

                calTplEditReq = {
                    calendarTemplate:
                        {
                            dbCalendarTemplateID: calTpl.dbCalendarTemplateID,
                            timeSlots: timeSlotHelpers.between(tSlotsToEdit, minEditTime, maxEditTime)
                        },
                    startTime: minEditTime,
                    endTime: maxEditTime,
                    delTimeSlots: delTimeSlots
                };

                rBook.saveCalTempl(calTplEditReq).then(
                    function (data) {
                        scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots = timeSlotHelpers.replace(
                            scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots,
                            data.Data,
                            minEditTime,
                            maxEditTime
                        );
                        tSlotsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;
                        maxEditTime = 0;
                        minEditTime = 0;
                        tSlotsDeleted.length = 0;
                        recalcCalContent();

                        fcRefreshEvents();
                    },
                    function () {
                        alert('nok');
                    }
                );
            };

            function tSlotLayer(tSlot) {
                var i;

                for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                    if (tSlot.calDbId === scope.rBook.calendarLayers[i].dbCalendarTemplateID) {
                        return i;
                    }
                }
                return -1;
            }

            updateCalEvents = function () {
                var i, tSlots, tSlot, ts, tsLayer;

                if (scope.model.calMode === 'combined') {
                    tSlots = combCal.timeSlots;
                } else {
                    tSlots = calTemplHelpers.createCombinedCalTempl(
                        scope.rBook.calendarLayers,
                        scope.model.calendarLayerSelected
                    ).timeSlots;
                }

                calEvents = [];
                for (i = 0; i < tSlots.length; i += 1) {
                    tSlot = tSlots[i];

                    tsLayer = tSlotLayer(tSlot);

                    ts = {
                        allDay: false,
                        backgroundColor:
                            Metronic.getBrandColor(
                                [ 'green', 'blue', 'yellow' ][tsLayer]
                            ),
                        editable: tsLayer === scope.model.layerInEdit,
                        end: moment(1000 * tSlot.endTime).format(),
                        start: moment(1000 * tSlot.startTime).format(),
                        title: tSlot.description,
                        origTSlot: tSlot.origTSlot // || tSlot for listAllTimeSlots
                    };

                    calEvents.push(ts);
                }

                //fcRedraw(); // fcUpdateEvents();
            };

            updateEditTimes = function (minTime, maxTime) {
                if (minTime && (!minEditTime || (minTime < minEditTime))) {
                    minEditTime = minTime;
                }
                if (maxTime && (!maxEditTime || (maxTime > maxEditTime))) {
                    maxEditTime = maxTime;
                }
            };

            /*
            watchWidthChanges = function () {
                scope.$watch(
                    function () {
                        return child[0].offsetWidth;
                    },
                    fcRedraw
                );

                angular.element($window).bind('resize', function () {
                    scope.$apply();
                });
            };
            */

            scope.model = scope.model || {};
            scope.model.calendarLayerSelected = [];

            scope.model.calMode = 'combined';
            scope.model.layerInEdit = 1;

            scope.changeCalLayers = function () {
                updateCalEvents();
                fcRefreshEvents();
            };

            scope.changeCalMode = function () {
                updateCalEvents();
                fcRefreshEvents();
            };

            scope.changeEditLayer = function () {
                tSlotsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;

                updateCalEvents();
                fcRefreshEvents();
            };

            scope.isMobile = false;

            // watchWidthChanges();
            fcRedraw();
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

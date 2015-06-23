
/*global alert, angular, calHelpers, calLayerHelpers, fullCalendarHelpers, jQuery, Metronic, moment, timeSlotHelpers */
(function () {
    'use strict';

    angular
        .module('ReservationCalendar', [])
        .directive('reservationCalendar', ['rBook', 'globalDialogs', function (rBook, globalDialogs) {
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
                link: function (scope, elem) {

                    var // child = elem.find('.calendar'),
                        calBody = angular.element(elem.find('.calendar-body')),
                        calEvents = [],
                        combCal = {},
                        fcState = {
                            view: 'agendaWeek'
                        },
                        h = {},
                        maxEditTime = 0,
                        minEditTime = 0,
                        saving = false,
                        tsDeleted = [],
                        tsToEdit,
                        useLazyFetching = true,

                    // functions that are used before definition (get rid of Lint warning):
                        combineAdj,
                        getCalEvents,
                        isOverlapping,
                        saveCalLayerDB,
                        tSlotLayer,
                        updateCalEvents,
                        updateCalLayerOnChange,
                        updateEditTimes;
                    // watchWidthChanges; // function

                    fullCalendarHelpers.init(calBody);

                    function addNewTS(start, end) {
                        var ts, tsOrig;

                        if (saving) {
                            calBody.fullCalendar('unselect');
                            return;
                        }
                        saving = true;

                        ts = {
                            startTime: start.unix(),
                            endTime: end.unix()
                        };

                        tsOrig = {
                            calDbType: scope.rBook.calendarLayers[scope.model.layerInEdit].calendarDbType,
                            calDbId: scope.rBook.calendarLayers[scope.model.layerInEdit].dbCalendarLayerID,
                            startTime: start.unix(),
                            endTime: end.unix(),
                            timeSlotStatus: calHelpers.TimeSlotStatus.FREE
                        };
                        ts.tsOrig = tsOrig;

                        if (!isOverlapping(ts)) {
                            updateEditTimes(tsOrig.startTime, tsOrig.endTime);
                            tsToEdit.push(tsOrig);

                            combineAdj(ts);

                            saveCalLayerDB();
                        } else {
                            saving = false;
                        }
                        calBody.fullCalendar('unselect');
                    }

                    function clickCalEvent(ev) {
                        if (saving) {
                            return;
                        }

                        rBook.notifyOnCalLayerChange(updateCalLayerOnChange);

                        globalDialogs.dialogs.meetingDetailsDialog.show({
                            tsOrig: ev.tsOrig,
                            tsToEdit: tsToEdit
                        });
                    }

                    combineAdj = function (ts) {
                        if (scope.rBook.calendarLayers[tSlotLayer(ts.tsOrig)].useMerging) {
                            var combRes = timeSlotHelpers.combineAdjacent(ts, tsToEdit);

                            updateEditTimes(combRes.minTime, combRes.maxTime);
                            tsDeleted = combRes.delArr;
                        }
                    };

                    isOverlapping = function (ts) {
                        ts.dbId = scope.rBook.calendarLayers[scope.model.layerInEdit].dbCalendarLayerID;
                        return timeSlotHelpers.isOverlapping(ts, fullCalendarHelpers.eventsToTS());
                    };

                    /*jslint unparam: true */
                    function eventMod(ev, delta, revertFunc) {
                        var ts;

                        if (saving) {
                            revertFunc();
                            return;
                        }
                        saving = true;

                        /*jslint nomen: true */
                        ts = {
                            id: ev._id,
                            startTime: ev.start.unix(),
                            endTime: ev.end.unix(),
                            tsOrig: ev.tsOrig
                        };

                        if (isOverlapping(ts)) {
                            revertFunc();
                            saving = false;
                        } else {
                            updateEditTimes(
                                Math.min(ev.tsOrig.startTime, ts.startTime),
                                Math.max(ev.tsOrig.endTime, ts.endTime)
                            );
                            ev.tsOrig.startTime = ts.startTime;
                            ev.tsOrig.endTime = ts.endTime;
                            combineAdj(ts);

                            saveCalLayerDB();
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
                            eventClick: clickCalEvent,
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
                            select: addNewTS,
                            slotEventOverlap: true,
                            slotMinutes: 15,
                            timezone: 'local',
                            viewRender: function (view) {
                                fcState.view = view.name;
                            },
                            weekends: false
                        });
                    }

                    getCalEvents = function (start, end, timezone, callback) {
                        rBook.getRBook(1, start.unix(), end.unix()).then(
                            function (ret) {
                                var i, sel;

                                if (!scope.rBook || !useLazyFetching) {
                                    scope.rBook = ret;
                                } else {
                                    calLayerHelpers.merge(scope.rBook, ret);
                                }

                                calLayerHelpers.sortCalByWeight(scope.rBook.calendarLayers);
                                sel = scope.model.calendarLayerSelected;
                                sel.length = 0;
                                for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                                    sel.push(true);
                                }

                                updateCalEvents(false);

                                // FFS: misplaced
                                tsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;

                                calBody.fullCalendar('removeEvents');
                                callback(calEvents);
                            },
                            function (err) {
                                throw new Error('Reading cal events fails');
                            }
                        );
                    };

                    saveCalLayerDB = function () {
                        var calLayer = scope.rBook.calendarLayers[scope.model.layerInEdit], calLayerEditReq, delTimeSlots = [], i;

                        if (calLayer.calendarDbType !== calHelpers.CalendarDbType.ABSOLUTE) {
                            throw new Error('Handling other than absolute calendars unimplemented');
                        }

                        for (i = 0; i < tsDeleted.length; i += 1) {
                            if (tsDeleted[i].dbId) {
                                delTimeSlots.push({
                                    dbId: tsDeleted[i].dbId,
                                    rowVersion: tsDeleted[i].rowVersion
                                });
                            }
                            timeSlotHelpers.delete(tsDeleted[i], tsToEdit);
                        }

                        calLayerEditReq = {
                            calendarLayer: {
                                dbCalendarLayerID: calLayer.dbCalendarLayerID,
                                timeSlots: timeSlotHelpers.between(tsToEdit, minEditTime, maxEditTime)
                            },
                            startTime: minEditTime,
                            endTime: maxEditTime,
                            delTimeSlots: delTimeSlots
                        };

                        rBook.saveCalLayer(calLayerEditReq).then(
                            function (data) {
                                scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots = timeSlotHelpers.replace(
                                    scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots,
                                    data.timeSlots,
                                    data.startTime,
                                    data.endTime
                                );
                                tsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;
                                maxEditTime = 0;
                                minEditTime = 0;
                                tsDeleted.length = 0;
                                updateCalEvents(true);
                                saving = false;
                            }
                        );
                    };

                    tSlotLayer = function (tSlot) {
                        var i;

                        for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                            if (tSlot.calDbId === scope.rBook.calendarLayers[i].dbCalendarLayerID) {
                                return i;
                            }
                        }
                        return -1;
                    };

                    updateCalEvents = function (doRefresh) {
                        var i, tSlots, tSlot, ts, tsLayer;

                        combCal = calLayerHelpers.createCombinedCalLayer(
                            scope.rBook.calendarLayers,
                            (scope.model.calMode === 'combined') ? undefined : scope.model.calendarLayerSelected,
                            true
                        );
                        tSlots = combCal.timeSlots;

                        calEvents = [];
                        for (i = 0; i < tSlots.length; i += 1) {
                            tSlot = tSlots[i];

                            tsLayer = tSlotLayer(tSlot);

                            ts = {
                                allDay: false,
                                backgroundColor: Metronic.getBrandColor(
                                    ['green', 'blue', 'yellow'][tsLayer]
                                ),
                                editable: tsLayer === scope.model.layerInEdit,
                                end: moment(1000 * tSlot.endTime).format(),
                                start: moment(1000 * tSlot.startTime).format(),
                                title: tSlot.description,
                                tsOrig: tSlot.tsOrig // || tSlot for listAllTimeSlots
                            };

                            if (tsLayer !== scope.model.layerInEdit) {
                                ts.rendering = 'background';
                            }

                            calEvents.push(ts);
                        }

                        if (doRefresh) {
                            calBody.fullCalendar('removeEvents');
                            calBody.fullCalendar('addEventSource', calEvents);
                        }
                    };

                    updateCalLayerOnChange = function (data) {
                        scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots = timeSlotHelpers.replace(
                            scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots,
                            data.timeSlots,
                            data.startTime,
                            data.endTime
                        );
                        tsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;
                        maxEditTime = 0;
                        minEditTime = 0;
                        tsDeleted.length = 0;
                        updateCalEvents(true);
                        rBook.notifyOnCalLayerChange(undefined);
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

                    scope.eventHtml = '';
                    scope.isMobile = false;

                    scope.changeCalLayers = function () {
                        updateCalEvents(true);
                    };

                    scope.changeCalMode = function () {
                        updateCalEvents(true);
                    };

                    scope.changeEditLayer = function () {
                        tsToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;
                        updateCalEvents(true);
                    };

                    // watchWidthChanges();
                    fcRedraw();
                }
            };
        }])
        .directive('reservationCalendarControls', function () {
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/reservationCalendarControls.html'
            };
        });
}());

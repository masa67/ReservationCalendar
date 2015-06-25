
/*global alert, angular, calHelpers, calLayerHelpers, fullCalendarHelpers, jQuery, Metronic, moment, timeSlotHelpers */
(function () {
    'use strict';

    angular
        .module('ReservationCalendar', [])
        .directive('reservationCalendar', ['rBook', 'locale', 'globalDialogs', function (rBook, locale, globalDialogs) {
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/reservationCalendar.html',
                link: function (scope, elem) {

                    var // child = elem.find('.calendar'),
                        calBody = angular.element(elem.find('.calendar-body')),
                        calEvents = [],
                        combCal = {},
                        combRes,
                        fcState = {
                            view: 'agendaWeek'
                        },
                        h = {},
                        saving = false,
                        slotDuration,
                        tsToDel = [],
                        tsToUpd = [],
                        calLToEdit,
                        useLazyFetching = true,

                    // functions that are used before definition (get rid of Lint warning):
                        combineAdj,
                        getCalEvents,
                        isOverlapping,
                        minToDuration,
                        saveCalLayerDB,
                        tSlotLayer,
                        updateCalEvents,
                        updateCalLayerOnChange;
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
                            calDbType: scope.rBook.calendarLayers[scope.model.layerInEdit].dbType,
                            calDbId: scope.rBook.calendarLayers[scope.model.layerInEdit].dbId,
                            startTime: start.unix(),
                            endTime: end.unix(),
                            timeSlotStatus: calHelpers.TimeSlotStatus.FREE
                        };
                        ts.tsOrig = tsOrig;

                        if (!isOverlapping(ts)) {
                            combineAdj(ts);

                            if (!combRes || !combRes.delATS) {
                                tsToUpd.push(tsOrig);
                            }

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

                        if (scope.rBook.calendarLayers[scope.model.layerInEdit].layerType ===
                                calHelpers.CalendarLayerType.MEETING) {
                            globalDialogs.dialogs.meetingDetailsDialog.show({
                                tsOrig: ev.tsOrig,
                                calLToEdit: calLToEdit
                            });
                        } else {
                            globalDialogs.dialogs.timeslotDialog.show({
                                tsOrig: ev.tsOrig,
                                calLToEdit: calLToEdit
                            });

                        }
                    }

                    combineAdj = function (ts) {
                        if (scope.rBook.calendarLayers[tSlotLayer(ts.tsOrig)].useMerging) {
                            combRes = timeSlotHelpers.combineAdjacent(ts, calLToEdit);

                            if (combRes.delSlot) {
                                tsToDel.push(combRes.delSlot);
                            }

                            if (combRes.updSlot) {
                                tsToUpd.push(combRes.updSlot);
                            }
                        } else {
                            combRes = undefined;
                        }
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
                            ev.tsOrig.startTime = ts.startTime;
                            ev.tsOrig.endTime = ts.endTime;

                            combineAdj(ts);

                            if (combRes) {
                                if (combRes.delATS) {
                                    tsToDel.push(ts.tsOrig);
                                } else {
                                    if (combRes.updATS) {
                                        tsToUpd.push(ts.tsOrig);
                                    }
                                }
                            } else {
                                tsToUpd.push(ts.tsOrig);
                            }

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

                        slotDuration = minToDuration(scope.rBook.reservationBook.SlotDuration);

                        calBody.fullCalendar('destroy'); // destroy the calendar
                        calBody.fullCalendar({ //re-initialize the calendar
                            allDaySlot: false,
                            buttonText: {
                                prev: '<',
                                next: '>'
                            },
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
                            header: h,
                            lang: locale.getLang(),
                            lazyFetching: useLazyFetching,
                            selectable: true,
                            selectHelper: true,
                            select: addNewTS,
                            slotDuration: slotDuration,
                            slotEventOverlap: true,
                            timezone: 'local',
                            viewRender: function (view) {
                                fcState.view = view.name;
                            },
                            weekends: false,
                            weekNumbers: true
                        });
                    }

                    getCalEvents = function (start, end, timezone, callback) {
                        rBook.getRBookFull(1, start.unix(), end.unix()).then(
                            function (ret) {
                                var i, sel;

                                if (!scope.rBook || !scope.rBook.calendarLayers || !useLazyFetching) {
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
                                calLToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;

                                calBody.fullCalendar('removeEvents');
                                callback(calEvents);
                            },
                            function (err) {
                                throw new Error('Reading cal events fails');
                            }
                        );
                    };

                    function handleCalLEditResp(resp) {
                        var found, i, id, j, uTS;

                        if (resp.updTimeSlots) {
                            for (i = 0; i < resp.updTimeSlots.length; i += 1) {
                                uTS = resp.updTimeSlots[i];
                                found = false;
                                for (j = 0; j < calLToEdit.length; j += 1) {
                                    if (calLToEdit[j].dbId === uTS.dbId) {
                                        calLToEdit[j].rowVersion = uTS.rowVersion;
                                        if (uTS.rowVersionMeeting) {
                                            calLToEdit[j].meeting.rowVersion = uTS.rowVersionMeeting;
                                        }
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found) {
                                    tsToUpd[i].dbId = uTS.dbId;
                                    tsToUpd[i].rowVersion = uTS.rowVersion;
                                    if (uTS.rowVersionMeeting) {
                                        tsToUpd[i].meeting.rowVersion = uTS.rowVersionMeeting;
                                    }
                                    calLToEdit.push(tsToUpd[i]);
                                }
                            }
                        }

                        if (resp.delTimeSlots) {
                            for (i = 0; i < resp.delTimeSlots.length; i += 1) {
                                id = resp.delTimeSlots[i];
                                found = false;
                                for (j = 0; j < calLToEdit.length; j += 1) {
                                    if (calLToEdit[j].dbId === id) {
                                        timeSlotHelpers.delete(calLToEdit[j], calLToEdit);
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found) {
                                    throw new Error('DB update mismatch');
                                }
                            }
                        }

                        tsToDel.length = 0;
                        tsToUpd.length = 0;
                        updateCalEvents(true);
                    }

                    isOverlapping = function (ts) {
                        ts.dbId = scope.rBook.calendarLayers[scope.model.layerInEdit].dbId;
                        return timeSlotHelpers.isOverlapping(ts, fullCalendarHelpers.eventsToTS());
                    };

                    minToDuration = function (pmin) {
                        var hrs = Math.floor(pmin / 60),
                            min = pmin - 60 * hrs;

                        return ((hrs < 10) ? '0' : '') + hrs + ':' + ((min < 10) ? '0' : '') + min + ':00';
                    };

                    saveCalLayerDB = function () {
                        var calLayer = scope.rBook.calendarLayers[scope.model.layerInEdit], calLayerEditReq, delTimeSlots = [], i;

                        if (calLayer.dbType !== calHelpers.CalendarDbType.ABSOLUTE) {
                            throw new Error('Handling other than absolute calendars unimplemented');
                        }

                        for (i = 0; i < tsToDel.length; i += 1) {
                            delTimeSlots.push({
                                dbId: tsToDel[i].dbId,
                                rowVersion: tsToDel[i].rowVersion
                            });
                        }

                        calLayerEditReq = {
                            delTimeSlots: tsToDel,
                            updTimeSlots: tsToUpd
                        };

                        rBook.saveCalLayer(calLayerEditReq).then(
                            function (ret) {
                                if (ret.status === rBook.OpStatusType.OK) {
                                    handleCalLEditResp(ret.data);
                                } else {
                                    // refetch everything after concurrency conflict
                                    fcRedraw();
                                }
                                saving = false;
                            }
                        );
                    };

                    tSlotLayer = function (tSlot) {
                        var i;

                        for (i = 0; i < scope.rBook.calendarLayers.length; i += 1) {
                            if (tSlot.calDbId === scope.rBook.calendarLayers[i].dbId) {
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
                                title: tSlot.meeting ? tSlot.meeting.Title : undefined,
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
                        handleCalLEditResp(data);
                        rBook.notifyOnCalLayerChange(undefined);
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
                        calLToEdit = scope.rBook.calendarLayers[scope.model.layerInEdit].timeSlots;
                        updateCalEvents(true);
                    };

                    // watchWidthChanges();
                    rBook.getRBook(1).then(
                        function (ret) {
                            scope.rBook = ret;
                            fcRedraw();
                        }
                    );
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

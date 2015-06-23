
/*global angular, $, timeSlotHelpers */
(function () {
    'use strict';

    angular
        .module('MeetingDetails', [])
        .directive('meetingDetails', ['globalDialogs', 'rBook', function (globalDialogs, rBook) {

            var retObj;

            function tsDelete(ts, tsToEdit) {
                var calLayerEditReq, delTimeSlots = [], maxEditTime, minEditTime;

                delTimeSlots.push({
                    dbId: ts.dbId,
                    rowVersion: ts.rowVersion
                });
                timeSlotHelpers.delete(ts, tsToEdit);

                minEditTime = ts.startTime;
                maxEditTime = ts.endTime;

                calLayerEditReq = {
                    calendarLayer: {
                        dbCalendarLayerID: ts.calDbId,
                        timeSlots: timeSlotHelpers.between(tsToEdit, minEditTime, maxEditTime)
                    },
                    startTime: minEditTime,
                    endTime: maxEditTime,
                    delTimeSlots: delTimeSlots
                };

                rBook.saveCalLayer(calLayerEditReq).then(
                    function () {
                        retObj.hide();
                    }
                );
            }

            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/meetingDetails.html',
                link: function (scope, elem) {
                    var modalEl = $('#meeting-details'), isVisible, tsToEdit, tsOrig;

                    elem.find('.btn-delete').bind('click', function () {
                        tsDelete(tsOrig, tsToEdit);
                    });

                    modalEl.on('hide.bs.modal', function () {
                        isVisible = false;
                    });

                    retObj = {
                        dialogName: 'meetingDetailsDialog',
                        show: function (par) {

                            tsOrig = par.tsOrig;
                            tsToEdit = par.tsToEdit;

                            modalEl.modal('show');
                            isVisible = true;
                        },
                        hide: function () {
                            modalEl.modal('hide');
                            isVisible = false;
                        },
                        isVisible: function () {
                            return isVisible;
                        }
                    };

                    globalDialogs.add(retObj);
                }
            };
        }]);
}());
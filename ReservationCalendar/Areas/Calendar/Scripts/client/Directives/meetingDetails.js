
/*global angular, $, timeSlotHelpers */
(function () {
    'use strict';

    angular
        .module('MeetingDetails', [])
        .directive('meetingDetails', ['globalDialogs', 'rBook', function (globalDialogs, rBook) {

            var retObj;

            function tsDelete(ts, calLToEdit) {
                var calLayerEditReq, delTimeSlots = [], maxEditTime, minEditTime;

                delTimeSlots.push({
                    dbId: ts.dbId,
                    rowVersion: ts.rowVersion
                });

                calLayerEditReq = {
                    updTimeSlots: [],
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
                    var modalEl = $('#meeting-details'), isVisible, calLToEdit, tsOrig;

                    elem.find('.btn-delete').bind('click', function () {
                        tsDelete(tsOrig, calLToEdit);
                    });

                    modalEl.on('hide.bs.modal', function () {
                        isVisible = false;
                    });

                    retObj = {
                        dialogName: 'meetingDetailsDialog',
                        show: function (par) {

                            tsOrig = par.tsOrig;
                            calLToEdit = par.calLToEdit;

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
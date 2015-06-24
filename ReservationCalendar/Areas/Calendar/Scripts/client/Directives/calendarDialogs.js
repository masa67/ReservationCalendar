
/*global angular, $, timeSlotHelpers */
(function () {
    'use strict';

    function tsDelete(ts, rBook, hideCallback) {
        var calLayerEditReq, delTimeSlots = [];

        delTimeSlots.push({
            dbId: ts.dbId,
            rowVersion: ts.rowVersion
        });

        calLayerEditReq = {
            updTimeSlots: [],
            delTimeSlots: delTimeSlots
        };

        rBook.saveCalLayer(calLayerEditReq).then(hideCallback);
    }

    angular
        .module('CalendarDialogs', [])
        .directive('meetingDetails', ['globalDialogs', 'rBook', function (globalDialogs, rBook) {

            var retObj;

            /*jslint unparam: true */
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/meetingDetails.html',
                link: function (scope, elem) {
                    var modalEl = $('#meeting-details'), isVisible, tsOrig;

                    elem.find('.btn-delete').bind('click', function () {
                        tsDelete(tsOrig, rBook, retObj.hide);
                    });

                    modalEl.on('hide.bs.modal', function () {
                        isVisible = false;
                    });

                    retObj = {
                        dialogName: 'meetingDetailsDialog',
                        show: function (par) {

                            tsOrig = par.tsOrig;

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
        }])
        .directive('timeslotDialog', ['globalDialogs', 'rBook', function (globalDialogs, rBook) {

            var retObj;

            /*jslint unparam: true */
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/timeslotDialog.html',
                link: function (scope, elem) {
                    var modalEl = $('#timeslot-dialog'), isVisible, tsOrig;

                    elem.find('.btn-delete').bind('click', function () {
                        tsDelete(tsOrig, rBook, retObj.hide);
                    });

                    modalEl.on('hide.bs.modal', function () {
                        isVisible = false;
                    });

                    retObj = {
                        dialogName: 'timeslotDialog',
                        show: function (par) {

                            tsOrig = par.tsOrig;

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

/*global angular, $ */
(function () {
    'use strict';

    angular
        .module('MeetingDetails', [])
        .directive('meetingDetails', ['globalDialogs', function (globalDialogs) {
            return {
                restrict: 'E',
                templateUrl: '/Areas/Calendar/Templates/meetingDetails.html',
                link: function (scope, elem) {
                    var modalEl = $('#meeting-details'), isVisible;

                    scope.tsOrig = undefined;

                    elem.find('.btn-delete').bind('click', function (e) {
                        alert('delete ' + scope.tsOrig.dbId);
                    });

                    globalDialogs.add({
                        dialogName: 'meetingDetailsDialog',
                        show: function (par) {
                            scope.tsOrig = par.tsOrig;
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
                    });
                }
            };
        }]);
}());
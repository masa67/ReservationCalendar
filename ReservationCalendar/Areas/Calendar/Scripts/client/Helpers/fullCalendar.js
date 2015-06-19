
var fullCalendarHelpers = (function () {
    'use strict';

    var calBody;

    return {
        init: function (cb) {
            calBody = cb;
        },

        eventsToTS: function () {
            var calEv = calBody.fullCalendar('clientEvents'), ev, i, retArr = [];

            for (i = 0; i < calEv.length; i += 1) {
                ev = calEv[i];
                /*jslint nomen: true */
                retArr.push({
                    id: ev._id,
                    startTime: ev.start.unix(),
                    endTime: ev.end && ev.end.unix(),
                    tsOrig: ev.tsOrig
                });
            }

            return retArr;
        }
    };
}());

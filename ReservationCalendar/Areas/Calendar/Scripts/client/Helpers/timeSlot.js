
/*global calHelpers, moment */
var timeSlotHelpers = (function () {
    'use strict';

    var retObj = {};

    retObj.timestampToLocalDate = function (ts) {
        return moment(1000 * ts).hours(0).minutes(0).seconds(0);
    };

    retObj.checkOverlap = function (aTS, bTS) {
        var aStartDate,
            aStartTime,
            aEndTime,
            bStartDate,
            bStartTime,
            bEndDate,
            bEndTime;

        aStartTime = moment(aTS.startTime).unix();
        aEndTime = moment(aTS.endTime).unix();

        bStartTime = moment(bTS.startTime).unix();
        bEndTime = moment(bTS.endTime).unix();

        if (aStartTime >= bEndTime || aEndTime <= bStartTime) {
            return calHelpers.TimeSlotOverlap.NONE;
        }

        if (aStartTime > bStartTime) {
            if (aEndTime >= bEndTime) {
                return calHelpers.TimeSlotOverlap.LATE_OVERLAP;
            }
            return calHelpers.TimeSlotOverlap.SPLIT_OVERLAP;
        }
        if (aEndTime >= bEndTime) {
            return calHelpers.TimeSlotOverlap.OVERRIDE;
        }
        return calHelpers.TimeSlotOverlap.EARLY_OVERLAP;
    };

    retObj.checkAdjacent = function (aTS, bTS) {
        return (aTS.startTime === bTS.endTime) && (aTS.endTime === bTS.startTime);
    };

    retObj.isOverlapping = function (aTS, tsArr) {
        var i;

        for (i = 0; i < tsArr.length; i += 1) {
            if ((!aTS.id || aTS.id !== tsArr[i].id) &&
                    retObj.checkOverlap(aTS, tsArr[i]) !== calHelpers.TimeSlotOverlap.NONE) {
                return true;
            }
        }
        return false;
    };

    retObj.hasAdjacent = function (aTS, tsArr) {
        var i;

        for (i = 0; i < tsArr.length; i += 1) {

        }
    };

    return retObj;
}());
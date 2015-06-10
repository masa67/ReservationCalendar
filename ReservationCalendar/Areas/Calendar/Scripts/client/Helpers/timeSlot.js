
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

        if (aTS.fullDay || bTS.fullDay) {
            aStartDate = retObj.timestampToLocalDate(aTS.startTime);
            bStartDate = retObj.timestampToLocalDate(bTS.startTime);

            if (aTS.fullDay) {
                if (bTS.fullDay) {
                    if (aStartDate === bStartDate) {
                        return calHelpers.TimeSlotOverlap.OVERRIDE;
                    }
                    return calHelpers.TimeSlotOverlap.NONE;
                }

                if (aStartDate < bStartDate) {
                    return calHelpers.TimeSlotOverlap.NONE;
                }

                bEndDate = retObj.timestampToLocalDate(bTS.endTime);

                if (aStartDate === bStartDate) {
                    if (aStartDate === bEndDate) {
                        return calHelpers.TimeSlotOverlap.OVERRIDE;
                    }
                    return calHelpers.TimeSlotOverlap.EARLY_OVERLAP;
                }
                if (aStartDate > bEndDate) {
                    return calHelpers.TimeSlotOverlap.NONE;
                }
                if (aStartDate === bEndDate) {
                    return calHelpers.TimeSlotOverlap.LATE_OVERLAP;
                }
                return calHelpers.TimeSlotOverlap.SPLIT_OVERLAP;
            }
        }

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

    return retObj;
}());

/*global calHelpers, moment */
var timeSlotHelpers = (function () {
    'use strict';

    var retObj = {};

    retObj.timestampToLocalDate = function (ts) {
        return moment(1000 * ts).hours(0).minutes(0).seconds(0);
    };

    retObj.checkOverlap = function (aTS, bTS) {
        var aStartTime,
            aEndTime,
            bStartTime,
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

    retObj.delete = function (aTS, tsArr) {
        var i;

        for (i = 0; i < tsArr.length; i += 1) {
            if (tsArr[i] === aTS) {
                tsArr.splice(i, 1);
                return;
            }
        }
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

    retObj.combineAdjacent = function (aTS, tsArr) {
        var bTS, i, delArr = [], pre, post;

        for (i = 0; i < tsArr.length && (!pre || !post); i += 1) {
            bTS = tsArr[i];
            if (aTS.startTime === bTS.endTime) {
                pre = bTS;
            } else {
                if (aTS.endTime === bTS.startTime) {
                    post = bTS;
                }
            }
        }

        if (pre && post) {
            pre.endTime = post.endTime;
            delArr.push(aTS);
            delArr.push(post);
        } else {
            if (pre) {
                pre.endTime = aTS.endTime;
                delArr.push(aTS);
            } else {
                if (post) {
                    aTS.endTime = post.endTime;
                    delArr.push(post);
                }
            }
        }

        for (i = 0; i < delArr.length; i += 1) {
            retObj.delete(delArr[i], tsArr);
        }

        return delArr.length;
    };

    return retObj;
}());
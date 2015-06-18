
/*global calHelpers, moment */
var timeSlotHelpers = (function () {
    'use strict';

    var retObj = {};

    retObj.between = function (arr, startTime, endTime) {
        var i, retArr = [];

        for (i = 0; i < arr.length; i += 1) {
            if (!((arr[i].endTime <= startTime) || (arr[i].startTime >= endTime))) {
                retArr.push(arr[i]);
            }
        }

        return retArr;
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

    retObj.combineAdjacent = function (aTS, tsArr) {
        var bTS, i, delArr = [], maxTime, minTime, pre, post;

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

        minTime = aTS.startTime;
        maxTime = aTS.endTime;
        if (pre && post) {
            minTime = pre.startTime;
            maxTime = post.endTime;
            pre.endTime = post.endTime;
            delArr.push(aTS.origTSlot);
            delArr.push(post);
        } else {
            if (pre) {
                minTime = pre.startTime;
                pre.endTime = aTS.endTime;
                delArr.push(aTS.origTSlot);
            } else {
                if (post) {
                    maxTime = post.endTime;
                    aTS.origTSlot.endTime = post.endTime;
                    delArr.push(post);
                }
            }
        }

        return {
            minTime: minTime,
            maxTime: maxTime,
            delArr: delArr
        };
    };

    /*
    retObj.combineOverlapping = function (aTS, tsArr) {
        var bTS, i, j, delArr = [], pre, post;

        for (i = 0; i < tsArr.length; i += 1) {
            bTS = tsArr[i];

            switch (retObj.checkOverlap(aTS, bTS)) {
            case calHelpers.TimeSlotOverlap.LATE_OVERLAP:
                pre = bTS;
                break;
            case calHelpers.TimeSlotOverlap.EARLY_OVERLAP:
                post = bTS;
                break;
            case calHelpers.TimeSlotOverlap.OVERRIDE:
                delArr.push(bTS);
                break;
            case calHelpers.TimeSlotOverlap.SPLIT_OVERLAP:
                delArr.push(aTS.origTSlot);
                break;
            }
        }

        if (pre && post) {
            pre.endTime = post.endTime;
            delArr.push(aTS.origTSlot);
            delArr.push(post);
        } else {
            if (pre) {
                pre.endTime = aTS.endTime;
                delArr.push(aTS.origTSlot);
            } else {
                if (post) {
                    aTS.origTSlot.endTime = post.endTime;
                    delArr.push(post);
                }
            }
        }

        return delArr;
    };
    */

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

    retObj.replace = function (toArr, frArr, startTime, endTime) {
        var i, j;

        for (i = 0, j = 0; i < toArr.length; i += 1) {
            if ((toArr[i].endTime <= startTime) || (toArr[i].startTime >= endTime)) {
                toArr[j] = toArr[i];
                j += 1;
            }
        }
        toArr.length = j;

        return toArr.concat(frArr);
    };

    retObj.timestampToLocalDate = function (ts) {
        return moment(1000 * ts).hours(0).minutes(0).seconds(0);
    };

    return retObj;
}());
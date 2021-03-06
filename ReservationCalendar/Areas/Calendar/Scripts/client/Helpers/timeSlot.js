
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
        var bTS, i, delATS = false, delSlot, pre, post, updATS, updSlot;

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
            delATS = true;
            updSlot = pre;
            delSlot = post;
        } else {
            if (pre) {
                pre.endTime = aTS.endTime;
                delATS = true;
                updSlot = pre;
            } else {
                if (post) {
                    aTS.tsOrig.endTime = post.endTime;
                    delSlot = post;
                }
            }
        }

        return {
            delATS: delATS,
            updATS: updATS,
            delSlot: delSlot,
            updSlot: updSlot
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
                delArr.push(aTS.tsOrig);
                break;
            }
        }

        if (pre && post) {
            pre.endTime = post.endTime;
            delArr.push(aTS.tsOrig);
            delArr.push(post);
        } else {
            if (pre) {
                pre.endTime = aTS.endTime;
                delArr.push(aTS.tsOrig);
            } else {
                if (post) {
                    aTS.tsOrig.endTime = post.endTime;
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
                    aTS.tsOrig.calDbId === tsArr[i].tsOrig.calDbId &&
                    retObj.checkOverlap(aTS, tsArr[i]) !== calHelpers.TimeSlotOverlap.NONE) {
                return true;
            }
        }
        return false;
    };

    retObj.merge = function (tsArr1, tsArr2) {
        var i, j, last, match;

        last = tsArr1.length;
        for (i = 0; i < tsArr2.length; i += 1) {
            match = false;
            for (j = 0; j < last; j += 1) {
                if (tsArr1[j].dbId === tsArr2[i].dbId) {
                    match = true;
                    break;
                }
            }
            if (!match) {
                tsArr1.push(tsArr2[i]);
            }
        }
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

var calHelpers = (function () {
    'use strict';
    var retObj = {};

    retObj.TimeSlotStatus = {
        FREE: 0,
        RESERVED: 1,
        EXCLUDED: 2
    };

    retObj.CalendarDbType = {
        ABSOLUTE: 0,
        RELATIVE: 1
    };

    retObj.CalendarSourceType = {
        DATABASE: 0,
        LAYERED: 1
    };

    retObj.TimeSlotOverlap = {
        NONE: 0,
        LATE_OVERLAP: 1,
        EARLY_OVERLAP: 2,
        OVERRIDE: 3,
        SPLIT_OVERLAP: 4
    };


    return retObj;
}());
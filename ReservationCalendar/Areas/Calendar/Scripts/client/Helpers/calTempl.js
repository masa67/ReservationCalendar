
/*global calHelpers, moment, timeSlotHelpers */
var calTemplHelpers = (function () {
    'use strict';

    function duplicateObject(o) {
        return JSON.parse(JSON.stringify(o));
    }

    function cmpCalByWeight(a, b) {
        if (a.weight > b.weight) {
            return -1;
        }
        if (a.weight < b.weight) {
            return 1;
        }
        return 0;
    }

    return {
        createCombinedCalTempl: function (cals) {
            var cal, dSlot, i, j, k, mSlot, slot, timeSlotsToDelete, tsCmp,
                retObj = {
                    calendarSourceType: calHelpers.CalendarSourceType.LAYERED,
                    timeSlots: [],
                    timeSlotConflicts: []
                };

            cals.sort(cmpCalByWeight);

            for (i = 0; i < cals.length; i += 1) {
                cal = cals[i];
                for (j = 0; j < cal.timeSlots.length; j += 1) {
                    slot = cal.timeSlots[j];
                    timeSlotsToDelete = [];

                    for (k = 0; k < retObj.timeSlots.length; k += 1) {
                        mSlot = retObj.timeSlots[k];

                        tsCmp = timeSlotHelpers.checkOverlap(slot, mSlot);

                        if (tsCmp !== calHelpers.TimeSlotOverlap.NONE) {
                            retObj.timeSlotConflicts.push({
                                aSlot: slot,
                                bSlot: mSlot,
                                timeSlotOverlap: tsCmp
                            });
                        }

                        switch (tsCmp) {
                        case calHelpers.TimeSlotOverlap.NONE:
                            break;
                        case calHelpers.TimeSlotOverlap.LATE_OVERLAP:
                            mSlot.fullDay = false;
                            mSlot.endTime = slot.startTime;
                            break;
                        case calHelpers.TimeSlotOverlap.EARLY_OVERLAP:
                            if (mSlot.fullDay) {
                                mSlot.fullDay = false;
                                mSlot.endTime = moment(mSlot.startTime).add(1, 'days');
                            }
                            if (slot.fullDay) {
                                mSlot.startTime = moment(slot.startTime).add(1, 'days');
                            } else {
                                mSlot.startTime = slot.endTime;
                            }
                            break;
                        case calHelpers.TimeSlotOverlap.OVERRIDE:
                            timeSlotsToDelete.push(mSlot);
                            break;
                        case calHelpers.TimeSlotOverlap.SPLIT_OVERLAP:
                            dSlot = duplicateObject(mSlot);

                            mSlot.endTime = slot.startTime;
                            if (slot.fullDay) {
                                dSlot.startTime = moment(slot.startTime).add(1, 'days');
                            } else {
                                dSlot.startTime = slot.endTime;
                            }
                            retObj.timeSlots.push(dSlot);
                            break;
                        }
                    }

                    for (k = 0; k < timeSlotsToDelete.length; k += 1) {
                        retObj.timeSlots.splice(retObj.timeSlots.indexOf(timeSlotsToDelete[k]), 1);
                    }

                    retObj.timeSlots.push(duplicateObject(slot));
                }
            }

            return retObj;
        }
    };
}());


/*global calHelpers, moment, timeSlotHelpers */
var calTemplHelpers = (function () {
    'use strict';

    function duplicateObject(o) {
        return JSON.parse(JSON.stringify(o));
    }

    function cmpCalByWeight(a, b) {
        if (a.weight > b.weight) {
            return 1;
        }
        if (a.weight < b.weight) {
            return -1;
        }
        return 0;
    }

    return {
        createCombinedCalTempl: function (cals, sel) {
            var cal, dSlot, i, j, k, mSlot, slot, timeSlotsToDelete, tsCmp,
                retObj = {
                    calendarSourceType: calHelpers.CalendarSourceType.LAYERED,
                    timeSlots: [],
                    timeSlotConflicts: []
                };

            // this.sortCalByWeight(cals);

            for (i = 0; i < cals.length; i += 1) {
                if (!sel || sel[cals.length - i - 1]) {
                    cal = cals[cals.length - i - 1];
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
                                mSlot.endTime = slot.startTime;
                                break;
                            case calHelpers.TimeSlotOverlap.EARLY_OVERLAP:
                                mSlot.startTime = slot.endTime;
                                break;
                            case calHelpers.TimeSlotOverlap.OVERRIDE:
                                timeSlotsToDelete.push(mSlot);
                                break;
                            case calHelpers.TimeSlotOverlap.SPLIT_OVERLAP:
                                dSlot = duplicateObject(mSlot);

                                mSlot.endTime = slot.startTime;
                                dSlot.startTime = slot.endTime;
                                retObj.timeSlots.push(dSlot);
                                break;
                            }
                        }

                        for (k = 0; k < timeSlotsToDelete.length; k += 1) {
                            retObj.timeSlots.splice(retObj.timeSlots.indexOf(timeSlotsToDelete[k]), 1);
                        }

                        dSlot = duplicateObject(slot);
                        dSlot.tsOrig = slot;
                        retObj.timeSlots.push(dSlot);
                    }
                }
            }

            return retObj;
        },
        listAllTimeSlots: function (cals, sel) {
            var i, retObj = [];

            for (i = 0; i < cals.length; i += 1) {
                if (!sel || sel[i]) {
                    retObj.push.apply(retObj, cals[i].timeSlots);
                }
            }

            return retObj;
        },
        merge: function (toRB, frRB) {
            var i;

            for (i = 0; i < toRB.calendarLayers.length; i += 1) {
                timeSlotHelpers.merge(toRB.calendarLayers[i].timeSlots, frRB.calendarLayers[i].timeSlots);
            }
        },
        sortCalByWeight: function (cals) {
            cals.sort(cmpCalByWeight);
        }
    };
}());

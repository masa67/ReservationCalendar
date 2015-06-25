
/*global angular */
(function () {
    'use strict';

    angular
        .module('RBookService', [])
        .factory('rBook', ['$resource', '$q', function ($resource, $q) {
            var calLayerChangeCallback,
                retObj;

            retObj = {
                OpStatusType: {
                    OK: 'ok',
                    CONFLICT: 'conflict',
                    FAIL: 'fail'
                },
                getRBook: function (id, startTime, endTime) {
                    var d = $q.defer();

                    $resource(
                        '/api/ReservationBookAbsApi/GetReservationBookAbs/:id?startTime=:startTime&endTime=:endTime'
                    ).get(
                        {
                            id: id,
                            startTime: startTime,
                            endTime: endTime
                        },
                        function (ret) {
                            if (ret.Status) {
                                d.resolve(ret.Data);
                            } else {
                                throw new Error('Reservation Book not found.');
                            }
                        },
                        function () {
                            throw new Error('GetRreservationBookAbs failed.');
                        }
                    );

                    return d.promise;
                },
                notifyOnCalLayerChange: function (f) {
                    calLayerChangeCallback = f;
                },
                saveCalLayer: function (data) {
                    var d = $q.defer();

                    $resource(
                        '/api/CalendarLayerApi/Edit/1'
                    ).save(data, function (ret) {
                        if (ret.Status) {
                            d.resolve({
                                status: retObj.OpStatusType.OK,
                                data: ret.Data
                            });

                            if (calLayerChangeCallback) {
                                calLayerChangeCallback(ret.Data);
                            }
                        } else {
                            if (ret.Message === 'Concurrency conflict') {
                                d.resolve({
                                    status: retObj.OpStatusType.CONFLICT
                                });
                            } else
                                throw new Error('CalendarLayerApi/Edit failed: ' +
                                    ret.Message);
                        }
                    }, function () {
                        throw new Error('CalendarLayerApi/Edit failed.');
                    });

                    return d.promise;
                }
            };

            return retObj;
        }]);
}());

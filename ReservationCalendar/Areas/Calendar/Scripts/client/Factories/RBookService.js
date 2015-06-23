
/*global angular */
(function () {
    'use strict';

    angular
        .module('RBookService', [])
        .factory('rBook', ['$resource', '$q', function ($resource, $q) {

            return {
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
                            d.resolve(ret);
                        },
                        function () {
                            throw new Error('GetRreservationBookAbs failed.');
                            // d.reject(err);
                        }
                    );

                    return d.promise;
                },
                saveCalLayer: function (data) {
                    var d = $q.defer();

                    $resource(
                        '/api/CalendarLayerApi/Edit/1'
                    ).save(data, function (ret) {
                        if (ret.Data) {
                            d.resolve(ret.Data);
                        } else {
                            throw new Error('CalendarLayerApi/Edit failed: ' +
                                ret.Message);
                            // d.reject();
                        }
                    }, function () {
                        throw new Error('CalendarLayerApi/Edit failed.');
                        // d.reject(err);
                    });

                    return d.promise;
                }
            };
        }]);
}());


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
                        function (err) {
                            d.reject(err);
                        }
                    );

                    return d.promise;
                },
                saveCalTempl: function (data) {
                    var d = $q.defer();

                    $resource(
                        '/api/CalendarTemplateApi/Edit/1'
                    ).save(data, function (ret) {
                        d.resolve(ret);
                    }, function (err) {
                        d.reject(err);
                    });

                    return d.promise;
                }
            };
        }]);
}());

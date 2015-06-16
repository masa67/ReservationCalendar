
/*global angular */
(function () {
    'use strict';

    angular
        .module('RBookService', [])
        .factory('rBook', ['$resource', '$q', function ($resource, $q) {

            return {
                getRBook: function (id, startTime, endTime) {
                    return $resource(
                        '/api/ReservationBookAbsApi/GetReservationBookAbs/:id?startTime=:startTime&endTime=:endTime',
                        {
                            id: id,
                            startTime: startTime,
                            endTime: endTime
                        }
                    );
                },
                saveCalTempl: function (data) {
                    var d = $q.defer();

                    $resource(
                        '/api/AbsCalendarTemplateApi/Edit'
                    ).get(undefined /*data*/, function (ret) {
                        d.resolve(ret);
                    }, function (err) {
                        d.reject(err);
                    });

                    return d.promise;
                }
            };
        }]);
}());

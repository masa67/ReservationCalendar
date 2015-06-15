
/*global angular */
(function () {
    'use strict';

    angular
        .module('RBookService', [])
        .factory('rBook', ['$resource', '$q', function ($resource, $q) {

            return {
                getRBook: function (id) {
                    return $resource(
                        '/ReservationBookAbs/Details/:id?data=true',
                        { id: id }
                    );
                },
                saveCalTempl: function (id, data) {
                    var d = $q.defer();

                    $resource(
                        '/AbsCalendarTemplate/Edit/:id',
                        { id: id }
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

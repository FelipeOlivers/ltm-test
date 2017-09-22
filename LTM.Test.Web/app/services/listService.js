'use strict';
app.factory('listService', ['$http', '$q', 'ngAuthSettings', function ($http, $q, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var listServiceFactory = {};

    var _getLista = function () {

        return $http.post(serviceBase + 'api/lista/get', null, { headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'Authorization': 'Bearer ' + localStorage.getItem('currentToken') } })
            .success(function (result) {

            }).error(function (result) {

            });
    };

    listServiceFactory.getLista = _getLista;

    return listServiceFactory;

}]);
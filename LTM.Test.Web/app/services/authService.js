'use strict';
app.factory('authService', ['$http', '$q', 'ngAuthSettings', function ($http, $q, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};

    var _login = function (user, pass) {

        var data = "grant_type=password&username=" + user + "&password=" + pass;
        var token = localStorage.getItem('currentToken');
        var res;

        return $http.post(serviceBase + 'api/security/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } })
            .success(function (result) {
                localStorage.setItem('currentToken', result.access_token);
            }).error(function (result) {
                return result;
            });

    };

    authServiceFactory.login = _login;

    return authServiceFactory;

}]);
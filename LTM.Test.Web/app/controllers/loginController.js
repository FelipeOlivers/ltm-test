'use strict';
app.controller('loginController', ['$scope', '$location', '$http', 'authService', 'ngAuthSettings', function ($scope, $location, $http, authService, ngAuthSettings) {

    $scope.message = "";

    $scope.login = function () {

        authService.login($scope.user, $scope.pass)
            .then(function (response) {
                $location.path('/list');
            }).catch(function (err) {
                $scope.message = err.data.error_description;
            });
        ;

    };

    $scope.testar = function () {

        authService.login($scope.user, $scope.pass)
            .then(function (response) {
                $scope.token = response.data.access_token;
                $scope.expire = response.data.expires_in;
            }).catch(function (err) {
                $scope.message = err.data.error_description;
            });

    };

}]);

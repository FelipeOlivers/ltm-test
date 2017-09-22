'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.logOut = function () {
        $location.path('/login');
    }

    $scope.authentication = authService.authentication;

}]);
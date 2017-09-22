'use strict';
app.controller('listController', ['$scope', '$location', 'listService', function ($scope, $location, listService) {

    $scope.app = "Lista Telefônica";

    listService.getLista()
        .then(function (response) {
            $scope.contatos = response.data;
        }).catch(function (err) {
            $scope.message = err.data.error_description;
        });

}]);

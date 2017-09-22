var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);
app.config(function ($routeProvider) {

    $routeProvider.when("/list", {
        controller: "listController",
        templateUrl: "/app/views/list.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.otherwise({ redirectTo: "/login" });

});

var serviceBase = 'http://localhost:15797/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
});


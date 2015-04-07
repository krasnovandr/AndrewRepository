﻿angular.module('AudioNetworkApp')
.controller('LoginController', function ($scope, $rootScope, $http, $location, authorizationService) {
    //$scope.loginForm = {
    //    emailAddress: '',
    //    password: '',
    //    rememberMe: false,
    //    returnUrl: "",
    //    loginFailure: false
    //};
    $scope.password = '';
    $scope.userName = '';
    $scope.loginError = '';
    $scope.rememberMe = false;
    $scope.providers = [];

    $scope.login = function () {
        var loginData =
        {
            UserName: $scope.userName,
            Password: $scope.password,
            RememberMe: $scope.rememberMe
        };
        authorizationService.login(loginData).success(function (answer) {
            if (answer.Success) {
                $rootScope.logState.LoggedIn = !$rootScope.logState.LoggedIn;
                checkLogin();
                $scope.loginError = '';
                $location.path('/Home');

            } else {
                $scope.loginError = answer.Message;
            }
        });
    };
    $scope.logout = function () {
        authorizationService.logout().success(function () {
            $rootScope.logState.LoggedIn = !$rootScope.logState.LoggedIn;
            $location.path('/Login');
        });
    };

    function checkLogin() {
        authorizationService.checkLogin().success(function (answer) {
            if (answer.LoggedIn) {
                //$rootScope.logState.UserName = answer.UserName;
                //$rootScope.logState.LoggedIn = answer.LoggedIn;
                //$rootScope.logState.Id = answer.Id;
                $rootScope.logState =answer;
            } else {
            }
        });
    }


    checkLogin();
    $scope.getexternalProviders = function () {
        authorizationService.getLoginProviders().success(function (providers) {
            $scope.providers = providers;
        });
    }();

    $scope.externalAuthentification = function (provider) {
        var data = {
            provider: provider.AuthenticationType,
            returnUrl:'http://localhost:38114/signin-vkontakte'
        };
        authorizationService.externalAuthentification(data).success(function (providers) {
        });
    };

});


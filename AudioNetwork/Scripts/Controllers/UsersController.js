angular.module('AudioNetworkApp').
controller('UsersController', function ($scope, $http, userService) {

    $scope.searchModel = {
        FirstName: "",
        LastName: "",
        City: "",
        Country: "",
        Genres: "",
        Atrists: "",
        BirthDate: null
    };

    $scope.users = [];
    $scope.friends = [];
    $scope.songs = [];

    $scope.getUsers = function () {
        userService.getUsers().success(function (users) {
            $scope.users = users;
        });
    };

    $scope.removeFriend = function (friend) {
        userService.removeFriend(friend).success(function (users) {
            $scope.getFriends();
        });
    };

    $scope.addFriend = function (friend) {
        userService.addFriend(friend).success(function (users) {
            $scope.getUsers();
        });
    };

    $scope.getFriends = function () {
        userService.getFriends().success(function (friends) {
            $scope.friends = friends;
        });
    };

    $scope.searchUsers = function () {
        userService.searchUsers($scope.searchModel).success(function (users) {
            $scope.users = users;
        });
    };

    $scope.getFriends();
    $scope.getUsers();
});


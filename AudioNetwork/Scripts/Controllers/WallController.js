angular.module('AudioNetworkApp').
    controller('WallController', function ($scope, wallService, $routeParams, $rootScope, $modal, musicService) {
        $scope.wallItems = [];
        $scope.songIndexes = [];
        $scope.Note = "";
        $scope.wallItemSons = [];
        $scope.songs = [];

        $scope.getWall = function () {
            var userData = {};
            if ($routeParams.id) {
                userData.userId = $routeParams.id;
            } else {
                userData.userId = $rootScope.logState.Id;
            }
            wallService.getWall(userData).success(function (wallItems) {
                $scope.wallItems = wallItems;
            });
        };
        $scope.getWall();


        $scope.addWallItem = function () {
            var wallItem = {
                Note: $scope.Note,
                ItemSongs: $scope.wallItemSons,
                AddByUserId: $rootScope.logState.Id,

            };

            if ($routeParams.id) {
                wallItem.IdUserWall = $routeParams.id;
            } else {
                wallItem.IdUserWall = $rootScope.logState.Id;
            }

            wallService.addWallItem(wallItem).success(function (wallItems) {
                $scope.getWall();
                $scope.Note = "";
                $scope.songIndexes = [];
                $scope.wallItemSons = [];
            });
        };


        $scope.removeWallItem = function (wallItem) {
            wallService.removeWallItem(wallItem).success(function (wallItems) {
                $scope.getWall();
            });
        };

        $scope.open = function (size) {

            var modalInstance = $modal.open({
                templateUrl: 'Music/ViewSongsModal',
                size: size,
                scope: $scope
            });

            $scope.ok = function () {
                modalInstance.close($scope.selected.item);
            };

            $scope.cancel = function () {
                modalInstance.dismiss('cancel');
            };

            $scope.addSong = function (song, index) {
                var songIndex = {
                    song: song,
                    index: index
                };
                $scope.songIndexes.push(songIndex);
                $scope.songs.splice(index, 1);
                $scope.wallItemSons.push(song);
            };
        };

        $scope.removeWallItemSong = function (song, index) {
            for (var i = 0; i < $scope.songIndexes.length; i++) {
                if ($scope.songIndexes[i].song.SongId == song.SongId) {
                    $scope.songs.splice($scope.songIndexes[i].index, 1, song);
                }
            }
            $scope.wallItemSons.splice(index, 1);
        };

        $rootScope.getMyMusic = function () {
            musicService.getMySongs().success(function (songs) {
                $scope.songs = songs;
            });
        }();

    });


//WallController.$inject = ['$scope', 'wallService', '$routeParams', '$rootScope', '$modal', 'musicService'];


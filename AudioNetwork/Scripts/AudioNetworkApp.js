﻿var angularApp = angular.module('AudioNetworkApp', ['ngRoute', 'angularFileUpload', 'dndLists', 'ui.bootstrap', 'infScroll', 'sf.virtualScroll', 'ngSanitize', 'n3-line-chart']);
//angularApp.controller('HomeController', HomeController);
//angularApp.controller('LoginController', LoginController);
//angularApp.controller('RegisterController', RegisterController);
//angularApp.controller('MainController', MainController);
//angularApp.controller('UsersController', UsersController);
//angularApp.controller('DownloadController', DownloadController);
//angularApp.controller('PlayerController', PlayerController);
//angularApp.controller('PlaylistsController', PlaylistsController);
//angularApp.controller('MyController', MyController);
//angularApp.controller('MessagesController', MessagesController);
//angularApp.controller('UserController', UserController);
//angularApp.controller('SongController', SongController);
//angularApp.controller('WallController', WallController);
//angularApp.controller('ModalInstanceCtrl', ModalInstanceCtrl);


angularApp.controller('AppLoadController', function ($scope, FileUploader) {
    //  $scope.uploader = new FileUploader({ url: 'Upload/UploadImage', queueLimit: 1 });

    var uploader = $scope.uploader = new FileUploader({
        url: 'Upload/UploadImage',

        //queueLimit: 1
    });

    // FILTERS

    uploader.filters.push({
        name: 'imageFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
        }
    });
});

angularApp.directive('enterSubmit', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attrs) {
            elem.bind('keydown', function (event) {
                var code = event.keyCode || event.which;
                if (code === 13) {
                    if (!event.shiftKey) {
                        event.preventDefault();
                        scope.$apply(attrs.enterSubmit);
                    }
                }
            });
        }
    };
});

angularApp.directive("passwordVerify", function () {
    return {
        require: "ngModel",
        scope: {
            passwordVerify: '='
        },
        link: function (scope, element, attrs, ctrl) {
            scope.$watch(function () {
                var combined;

                if (scope.passwordVerify || ctrl.$viewValue) {
                    combined = scope.passwordVerify + '_' + ctrl.$viewValue;
                }
                return combined;
            }, function (value) {
                if (value) {
                    ctrl.$parsers.unshift(function (viewValue) {
                        var origin = scope.passwordVerify;
                        if (origin !== viewValue) {
                            ctrl.$setValidity("passwordVerify", false);
                            return undefined;
                        } else {
                            ctrl.$setValidity("passwordVerify", true);
                            return viewValue;
                        }
                    });
                }
            });
        }
    }
});
angularApp.filter('ctime', function () {

    return function (jsonDate) {

        var date = new Date(parseInt(jsonDate.substr(6)));
        return date;
    };

});

angularApp.filter('onlineUsersFilter', [function ($filter) {
    return function (inputArray, searchCriteria, txnType) {
        if (!angular.isDefined(searchCriteria) || searchCriteria == false) {
            return inputArray;
        }
        var data = [];
        angular.forEach(inputArray, function (item) {
            if (item.IsOnline == true) {
                data.push(item);
            }
        });
        return data;
    };
}]);
angularApp.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});

angularApp.directive('scroll', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            scope.$watchCollection(attr.scroll, function (newVal) {
                $timeout(function () {
                    element[0].scrollTop = element[0].scrollHeight;
                });
            });
        }
    }
});
angularApp.directive('ngThumb', ['$window', function ($window) {
    var helper = {
        support: !!($window.FileReader && $window.CanvasRenderingContext2D),
        isFile: function (item) {
            return angular.isObject(item) && item instanceof $window.File;
        },
        isImage: function (file) {
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
        }
    };

    return {
        restrict: 'A',
        template: '<canvas class=Mycanvas/>',
        link: function (scope, element, attributes) {
            if (!helper.support) return;

            var params = scope.$eval(attributes.ngThumb);

            if (!helper.isFile(params.file)) return;
            if (!helper.isImage(params.file)) return;

            var canvas = element.find('canvas');
            var reader = new FileReader();

            reader.onload = onLoadFile;
            reader.readAsDataURL(params.file);

            function onLoadFile(event) {
                var img = new Image();
                img.onload = onLoadImage;
                img.src = event.target.result;
            }

            function onLoadImage() {
                var width = params.width || this.width / this.height * params.height;
                var height = params.height || this.height / this.width * params.width;
                canvas.attr({ width: width, height: height });
                canvas[0].getContext('2d').drawImage(this, 0, 0, width, height);
            }
        }
    };
}]);

var config = function ($routeProvider) {
    $routeProvider.
      when('/Login', {
          templateUrl: 'Account/Login',
          controller: 'LoginController'
      }).when('/Register', {
          templateUrl: 'Account/Register',
          controller: 'RegisterController'
      }).when('/Home', {
          templateUrl: 'Users/Index',
      }).when('/Upload', {
          templateUrl: 'Upload/Upload',
      }).when('/Player', {
          templateUrl: 'Music/Player',
          //controller: 'DownloadController'
      }).when('/Users', {
          templateUrl: 'Users/ViewUsers',
      }).when('/Friends', {
          templateUrl: 'Users/ViewFriends',
      }).when('/Users/:id', {
          templateUrl: function (params) { return 'Users/ViewUser?id=' + params.id; },
          controller: 'UsersController'
      }).when('/Songs/:id', {
          templateUrl: function (params) { return 'Music/ViewSong?id=' + params.id; },
          controller: 'PlayerController'
      }).when('/Register/:id:email', {
          templateUrl: 'Account/Register',
          controller: 'RegisterController'
      }).when('/Conversations/:id', {
          templateUrl: 'Conversation/ViewConversation',
      }).when('/Conversations', {
          templateUrl: 'Conversation/ViewConversations',
      }).when('/EditConversations', {
          templateUrl: 'Conversation/ViewEditConversations',
      }).when('/Playlists', {
          templateUrl: 'Playlist/Playlists',
      }).when('/Music', {
          templateUrl: 'Music',
      }).when('/Statistics', {
          templateUrl: 'Statistics/ViewStatistics',
      }).when('/News', {
          templateUrl: 'Wall/ViewNews',
      }).
       otherwise({
           redirectTo: '/Login'
       });
};

config.$inject = ['$routeProvider'];

angularApp.config(config);
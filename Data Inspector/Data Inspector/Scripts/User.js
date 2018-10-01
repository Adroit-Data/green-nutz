var UserApp = angular.module('UserApp', [])

UserApp.controller('UserController', function ($scope, UserService) {

    getUsers();
    function getUsers() {
        UserService.getUsers()
            .then(function (usrs) {
                $scope.users = usrs;
                console.log($scope.users);
            });
    }

});

UserApp.factory('UserService', ['$http', function ($http) {


    var UserService = {};

    UserService.getUsers = function () {

        return $http.get('/UsersList/GetUsers');

    };

    return UserService;



}]);


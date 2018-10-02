var TableLoadApp = angular.module('TableLoadApp', [])

TableLoadApp.controller('TableLoadController', function ($scope, TableLoadService){

    $scope.sortType = 'DIRowID'; // set the default sort type
    $scope.sortReverse = false;  // set the default sort order
    $scope.searchFish = '';     // set the default search/filter term

    getTableLoadData();
    function getTableLoadData() {
        TableLoadService.getTableLoadData()
            .then(function (tblload) {
                $scope.tableload = tblload;
                console.log($scope.tableload);
            });
    }

});

TableLoadApp.factory('TableLoadService', ['$http', function ($http) {


    var TableLoadService = {};

    TableLoadService.getTableLoadData = function () {

        return $http.get('/MyLoads/GetTableLoadData/8be8750a-431c-440a-a067-b4371364dc31');
        //return $http.get('/UsersList/GetUsers');

    };

    return TableLoadService;



}]);


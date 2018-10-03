var TableLoadApp = angular.module('TableLoadApp', [])

TableLoadApp.controller('TableLoadController', function ($scope, tableid ,TableLoadService) {

    //$scope.init = function (tableid) {
    //    $scope.tableid = tableid;
    // }
    //$scope.tableid = '8BE8750A-431C-440A-A067-B4371364DC31';
    $scope.tableid = tableid;

    getTableLoadData($scope.tableid);

    function getTableLoadData(tableid) {
        TableLoadService.getTableLoadData(tableid)
            .then(function (tblload) {
                $scope.tableload = tblload;
                console.log($scope.tableload);
            });
    }

});


TableLoadApp.factory('TableLoadService', ['$http', function ($http) {

    var TableLoadService = {};

    TableLoadService.getTableLoadData = function (id) {       

        var geturl = '/MyLoads/GetTableLoadData/' + id;

        return $http.get(geturl);

    };

    return TableLoadService;

}]);






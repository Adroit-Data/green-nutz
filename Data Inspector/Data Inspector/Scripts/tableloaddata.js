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
                $scope.tableload.selected = {};
                console.log($scope.tableload);
            });
    }

    // gets the template to ng-include for a table row / item
    $scope.getTemplate = function (r) {
        if (r.DIRowID === $scope.tableload.selected.DIRowID) return 'edit';
         return 'display';
        //return 'display';
    };

    $scope.editData = function (r) {
        $scope.tableload.selected = angular.copy(r);
    };

    $scope.saveData = function (idx) {
        console.log("Saving data");
        $scope.tableload.data[idx] = angular.copy($scope.tableload.selected);
        $scope.reset();
    };

    $scope.reset = function () {
        $scope.tableload.selected = {};
    };

});


TableLoadApp.factory('TableLoadService', ['$http', function ($http) {

    var TableLoadService = {};

    TableLoadService.getTableLoadData = function (id) {       

        var geturl = '/MyLoads/GetTableLoadData/' + id;

        return $http.get(geturl);

    };

    return TableLoadService;

}]);






var TableLoadApp = angular.module('TableLoadApp', [])

TableLoadApp.controller('TableLoadController', function ($scope, tableid, $http ,TableLoadService) {

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
    };

    // changes the template from display to edit and copies the existing values over to the form
    $scope.editData = function (r) {
        $scope.tableload.selected = angular.copy(r);
    };

    //idx is the row number, r is the row data (original data), selected is the updated value data (changed data values)
    $scope.saveData = function (idx, r, selected) {
        console.log("Saving data");
        
        // set the rowID that's being updated
        $scope.rowid = r.DIRowID;

        // for each field/column, save to DB
        var x;
        for (x in r) {


            $scope.value = selected[x];

            // don't try to update the row id or anything else in r that is undefined object type
            if (typeof $scope.value != 'undefined' && x != 'DIRowID' )
            {
                (function (r) {
                    $http({
                        method: 'POST',
                        url: '/MyLoads/update',
                        params: { "TableName": $scope.tableid, "RowId": $scope.rowid, "ColumnName": x, "ColumnNewValue": $scope.value }
                    })
                .then(function (response) {
                    // success
                    console.log("Saved data");
                },
                function (response) { // optional
                    // failed
                });
                })();
            }
        }

        //refresh the data
        getTableLoadData($scope.tableid);
        //Update View
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






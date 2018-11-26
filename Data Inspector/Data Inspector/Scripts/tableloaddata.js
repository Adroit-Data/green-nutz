var TableLoadApp = angular.module('TableLoadApp', ['ui.bootstrap'])

TableLoadApp.controller('TableLoadController', function ($scope, tableid, $http ,TableLoadService) {
    
    $scope.tableid = tableid;

    getTableLoadData($scope.tableid);

    function getTableLoadData(tableid) {
        TableLoadService.getTableLoadData(tableid)
            .then(function (tblload) {
                $scope.tableload = tblload;
                $scope.tableload.selected = {};
                buildheaders();                
                console.log($scope.tableload);

                //Pagination
                //$scope.viewby = 10;
                $scope.totalItems = $scope.tableload.data.length;
                //$scope.currentPage = 1;
                //$scope.itemsPerPage = $scope.viewby;
                //$scope.maxSize = 5; //Number of pager buttons to show

                //$scope.setPage = function (pageNo) {
                //    $scope.currentPage = pageNo;
                //};

                //$scope.pageChanged = function () {
                //    console.log('Page changed to: ' + $scope.currentPage);
                //};

                //$scope.setItemsPerPage = function (num) {
                //    $scope.itemsPerPage = num;
                //    $scope.currentPage = 1; //reset to first page
                //}
                //Pagination END


            });
    }
     
    //Pagination
    $scope.viewby = 50;
    //$scope.totalItems = $scope.tableload.data.length;
    $scope.currentPage = 1;
    $scope.itemsPerPage = $scope.viewby;
    $scope.maxSize = 5; //Number of pager buttons to show

    $scope.setPage = function (pageNo) {
        $scope.currentPage = pageNo;
    };

    $scope.pageChanged = function () {
        console.log('Page changed to: ' + $scope.currentPage);
    };

    $scope.setItemsPerPage = function (num) {
        $scope.itemsPerPage = num;
        $scope.currentPage = 1; //reset to first page
    }
    //Pagination END

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
            var i = 0;
            var length = r.length - 1;

            var x;
            for (x in r) {

                i++;

                $scope.value = selected[x];

                // don't try to update the row id or anything else in r that is undefined object type
                if (typeof $scope.value != 'undefined' && x != 'DIRowID') {
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

                if (i === length) {
                    //Update View
                    getTableLoadData($scope.tableid);                    
                };

            }

            $scope.tableload.data[idx] = angular.copy($scope.tableload.selected);
            $scope.reset();
         
    };

    $scope.reset = function () {
        $scope.tableload.selected = {};
    };
        
    function buildheaders() {
        var x;
        var head = $scope.tableload.data[0];
        var built = [];
        for (x in head) {
            if (x != "DIRowID") {
                built.push(x);
            }            
        }
        $scope.headers = built;
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






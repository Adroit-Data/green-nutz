var TableLoadApp = angular.module('TableLoadApp', [])

TableLoadApp.controller('TableLoadController', function ($scope, tableid, $http, TableLoadService, FindData, UpdateCheck) {

    //$scope.init = function (tableid) {
    //    $scope.tableid = tableid;
    // }
    //$scope.tableid = '8BE8750A-431C-440A-A067-B4371364DC31';
    $scope.tableid = tableid;
    $scope.Column = 'Search By';
    $scope.Value = '';
    $scope.displayFirstXRecords = 25;
    $scope.sortByColumn = "nothing";
    $scope.sortOrder = '';
    $scope.updateCheck = {};

    getTableLoadData($scope.tableid, $scope.displayFirstXRecords, $scope.sortByColumn);

    function getTableLoadData(tableid, numOfRecords, sortBy, sortOrder) {
        $scope.Isloading = true;
        TableLoadService.getTableLoadData(tableid, numOfRecords, sortBy, sortOrder)
            .then(function (tblload) {
                $scope.tableload = tblload;
                $scope.tableload.selected = {};
                buildheaders();
                buildDropDownList();
                $scope.Isloading = false;
                //builddata();
                console.log($scope.tableload);
                //console.log($scope.Rafalrows);

 
            });
        
    };

    function doFieldUpdateCheck(id, Column, RowId) {

        UpdateCheck.doFieldUpdateCheck(id, Column, RowId)
        .then(function (checkResult) {


            if ($scope.updateCheck.length == 0) {
                $scope.updateCheck = checkResult;
            }
            else if ($scope.updateCheck == checkResult) {
                alert('update failed');
                $scope.updateCheck = {};

            }
            else {
                $scope.updateCheck = {};
            }

        });
    };


    function getFindTableDataF(tableid, Column, Value) {
        FindData.getTableLoadDataF(tableid, Column, Value)
            .then (function (tblloada) {
                $scope.Isloading = false;
                if (tblloada.data[0] == '' || tblloada.data[0] == null)
                {
                    alert('There are no results for provided Search Criteria')
                }
                else
                {
                    $scope.Isloading = true;
                    $scope.tableload = tblloada;
                    $scope.tableload.selected = {};
                    buildheaders();
                    buildDropDownList();
                    $scope.Isloading = false;
                }
            });

    };
  

    $scope.findData = function () {
        if ($scope.tableid == '' || $scope.Column == 'Search By' || $scope.Value == '')
        {
            alert('Please provide values for both "Search By" and "Value" field')
            getTableLoadData($scope.tableid, $scope.displayFirstXRecords, $scope.sortByColumn, $scope.sortOrder);;
        }
        else
        {
            $scope.displayFirstXRecords = 25;
            getFindTableDataF($scope.tableid, $scope.Column, $scope.Value)
        }
    };

    // gets the template to ng-include for a table row / item
    $scope.getTemplate = function (r) {
        if (r.DIRowID === $scope.tableload.selected.DIRowID) return 'edit';
         return 'display';
    };
    $scope.updateCheck;
    // changes the template from display to edit and copies the existing values over to the form
    $scope.editData = function (r) {
        $scope.tableload.selected = angular.copy(r);
        $scope.updateCheck = $scope.tableload.selected;
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

               // UpdateCheck($scope.tableid, x, $scope.rowid);
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
                        console.log("not saved data");
                    });
                    })();
                }

                if (i === length) {
                    //Update View
                    getTableLoadData($scope.tableid);                    
                };

               // doFieldUpdateCheck($scope.tableid, x, $scope.rowid);

            }

            $scope.tableload.data[idx] = angular.copy($scope.tableload.selected);
            //console.log("scope :" + $scope.tableload.data[idx]);
            //console.log("scope check :" + $scope.updateCheck);

            $scope.reset();
            getTableLoadData($scope.tableid, $scope.displayFirstXRecords, $scope.sortByColumn, $scope.sortOrder);
    };

    $scope.reset = function () {
        $scope.tableload.selected = {};
    };

    $scope.reload = function() {
        location.reload();
    };

    //SORT DATA
        
    function buildheaders() {
        var x;
        var head = $scope.tableload.data[0];
        var built = [];
        var dropDown = [];
        for (x in head) {
            if (x != "DIRowID") {
                built.push(x);
            }            
        }
        $scope.headers = built;
        dropDown = built;
        //dropDown.push('Search By');
        $scope.dropDownList = dropDown;

    };

    function buildDropDownList() {
        var x;
        var head = $scope.tableload.data[0];
        var dropDown = [];
        dropDown.push('Search By');
        for (x in head) {
            if (x != "DIRowID") {
                dropDown.push(x);
            }
        }
        $scope.dropDownList = dropDown;

    };

    function builddata() {
        var x;
        var singleRow = [];
        var singleRowData = [];
        var allRows = [];
        for (var x = 0 ; x <= 3 ; x++) {

            for (var i = x; i <= x; i++) {
                angular.forEach($scope.tableload.data[i], function (value, key) {
                    if (key != "DIRowID") {
                        this.push(value);
                    }
                }, singleRow);
            };
            allRows.push(singleRow)
            singleRow = [];

        };
        $scope.Rafalrows = allRows;

    };

    //sortation code for font awsome
    $scope.sortCol = false;

    $scope.revSort = false;

    $scope.sortData = function (columnIndex)
    {

        $scope.revSort = ($scope.sortCol == $scope.headers[columnIndex]) ? !$scope.revSort : false;
        
        if ($scope.revSort)
        {
            $scope.sortOrder = "Asc"
        }
        else
        {
            $scope.sortOrder = "Desc"
        }

        $scope.sortCol = $scope.headers[columnIndex];
        $scope.sortByColumn = $scope.headers[columnIndex];
        getTableLoadData($scope.tableid, $scope.displayFirstXRecords, $scope.sortByColumn, $scope.sortOrder);
    }

    //co-works with directive('infinitescroll') to get next lot of data after scroll all the way down
    $scope.Nextpage = function () {

        if ($scope.displayFirstXRecords < $scope.displayFirstXRecords+1) {

            $scope.displayFirstXRecords += 25;
            //$scope.totalpage = $scope.currentpage + 1;
            getTableLoadData($scope.tableid, $scope.displayFirstXRecords, $scope.sortByColumn, $scope.sortOrder)
            
        }
    };


    //event handler to detect if 'Enter' has been pressed in search 'Value' field
    document.getElementById("searchedValue").addEventListener('keydown', checkKeyPress);
    function checkKeyPress(key) {
        if (key.keyCode == "13") { // 13 is keyCode for Enter 
           $scope.findData(); 
        }

    }

});

TableLoadApp.directive('infinitescroll', function () {
    //debugger;
    return {
       

        restrict: 'A',

        link: function (scope, element, attrs) {

            element.bind('scroll', function () {

                if (((element[0].scrollTop) + element[0].offsetHeight) == element[0].scrollHeight) {
                 
                    //element[0].scrollTop -= element[0].scrollTop
                    element[0].scrollTop = element[0].scrollHeight - element[0].offsetHeight;
                    element[0].scrollHeight += element[0].scrollTop;
                   
                    //alert('"scrolled"=' + element[0].scrollTop + ' "offsetHeight="' + element[0].offsetHeight + ' "scrollHeight="' + element[0].scrollHeight)
                    scope.$apply(attrs.infinitescroll);
                    

                }
                //alert('"scrolled"=' + element[0].scrollTop + ' "offsetHeight="' + element[0].offsetHeight + ' "scrollHeight="' + element[0].scrollHeight)
            })   
        }
    }

})


TableLoadApp.factory('TableLoadService', ['$http', function ($http) {

     

    var TableLoadService = {};

    TableLoadService.getTableLoadData = function (id, numOfRecords, sortBy, sortOrder) {

        var geturl = '/MyLoads/GetTableLoadDataScroll/' + id + '?numOfRecords=' + numOfRecords + '&sortBy=' + sortBy + '&sortOrder=' + sortOrder;

        return $http.get(geturl);

    };

    return TableLoadService;

}]);

TableLoadApp.factory('FindData', ['$http', function ($http) {



    var FindData = {};

    FindData.getTableLoadDataF = function (id, Column, Value) {

        var geturl = '/MyLoads/DISearch/' + id + '?Column=' + Column + '&Value=' + Value;

        return $http.get(geturl);

    };

    return FindData;

}]);

TableLoadApp.factory('UpdateCheck', ['$http', function ($http) {



    var UpdateCheck = {};

    UpdateCheck = function (id, Column, RowId) {

        var geturl = '/MyLoads/UpdateCheck/' + id + '?Column=' + Column + '&RowId=' + RowId;

        return $http.get(geturl);

    };
    return UpdateCheck;

}]);








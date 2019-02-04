'use strict';
var ChartsPage = angular.module('ChartsPage', ['datatables']);

ChartsPage.controller('PageChartController', function ($scope, $http, TableFieldsPopulation, TableRecordsCount, TableMinMaxAvgValues, TableDateTypeFieldsData) {


    google.charts.load('current', { 'packages': ['corechart','gauge'] });

    // Set a callback to run when the Google Visualization API is loaded.
    google.charts.setOnLoadCallback(drawchart.data);
    google.charts.setOnLoadCallback(drawStatChart);
    google.charts.setOnLoadCallback(drawDatesPieChart);
    
 
    //$(function () {  
    //    $.ajax({  
    //        type: 'POST',  
    //        dataType: 'json',  
    //        contentType: 'application/json',  
    //        url: 'ColumnChart.aspx/GetChartData',  
    //        data: '{}',  
    //        success: function (response) {  
    //            drawchart(response.d); // calling method  
    //        },  
  
    //        error: function () {  
    //            alert("Error loading data! Please try again.");  
    //        }  
    //    });  
    //})  

    $scope.tableid = tableid;
    $scope.tableload = [];
    $scope.tableTotalNumberOfRecordsValue;
    $scope.statValues = [];
    $scope.dateStatValues = [];

    getTotalNumberOfRecords($scope.tableid);
    getTablePopulationData($scope.tableid);
    getTableMinMaxAvgValues($scope.tableid);
    getTableDateTypeFieldsData($scope.tableid);
    


    function getTablePopulationData(tableid) {
        TableFieldsPopulation.getTablePopulationData(tableid)
            .then(function (tblload) {

                $scope.tableload = tblload;

                console.log('TablePopulationData ', $scope.tableload);
                drawchart($scope.tableload);

            });

    };

    function getTotalNumberOfRecords(tableid) {
        TableRecordsCount.getTotalNumberOfRecords(tableid)
            .then(function (tblRecordCount) {

                var tableTotalNumberOfRecords = tblRecordCount;
                var singleValue;

                angular.forEach(tableTotalNumberOfRecords.data[0], function (value, key) {
                    if (key == "TotalNumberOfRecords") {
                        //this.push(value);
                        singleValue = value;
                    }
                });
                $scope.tableTotalNumberOfRecordsValue = singleValue;
                console.log('TotalNumberOfRecords ', $scope.tableTotalNumberOfRecordsValue);
            });

    };

    function getTableMinMaxAvgValues(tableid) {
        TableMinMaxAvgValues.getTableMinMaxAvgValues(tableid)
        .then(function (tblStatValues) {

            $scope.statValues = tblStatValues;
            console.log('TableMinMaxAvg', $scope.statValues)
        });
    };

    function getTableDateTypeFieldsData(tableid) {
        TableDateTypeFieldsData.getTableDateTypeFieldsData(tableid)
        .then(function (tblDateTypeStats) {

            $scope.dateStatValues = tblDateTypeStats;
            console.log('TableDateTypeFieldsData', $scope.dateStatValues)

        });

    }

    function drawchart(dataValues) {
        // Callback that creates and populates a data table,  
        // instantiates the pie chart, passes in the data and  
        // draws it.  
        var data = new google.visualization.DataTable();
        var numOfRecords = $scope.tableTotalNumberOfRecordsValue;

        data.addColumn('string', 'FieldName');
        data.addColumn('number', 'data');
        data.addColumn({ type: 'string', role: 'tooltip' });
        data.addColumn({ type: 'string', role: 'annotation' });

        for (var i = 0; i < dataValues.data.length; i++) {
            data.addRow([dataValues.data[i].FieldName, dataValues.data[i].PopulationPercentage, 'Populated: \n' + dataValues.data[i].PopulationCount.toString() + '\n out of \n' + numOfRecords + '\n records', +dataValues.data[i].PopulationPercentage + '% ']);
        }

        var view = new google.visualization.DataView(data);
        view.setColumns([0, 1, 1, 2]);

        // Set chart options
        var options = {

            //legend: 'none',
            title: '',
            titleTextStyle: {
                color: '#FF0000',
                fontSize: '20'
            },
            chartArea: {
                width: '70%'},
            vAxis: {
                title: 'Population %',
                titleTextStyle: {
                    color: '#FF0000',
                    fontSize: '20'
                },
                gridlines: { count: 21 },
                minValue: 0,
                maxValue: 100
            },
            hAxis: {
                title: 'Field Name',
                titleTextStyle: {
                    color: '#FF0000',
                    fontSize: '20'
                },

            },
            bar: { groupWidth: "50%" }
               
        };
        // Instantiate and draw our chart, passing in some options  
        var chart = new google.visualization.ColumnChart(document.getElementById('chartdiv'));

        chart.draw(data, options);
    };



    function drawStatChart() {
        if ($scope.statValues.data.length != 0) {

            for (var i = 0; i <= $scope.statValues.data.length; i++) {

                var data = google.visualization.arrayToDataTable([
                  ['Label', 'Value'],
                  ['Min', $scope.statValues.data[i].MinValue],
                  ['Max', $scope.statValues.data[i].MaxValue],
                  ['Avg', $scope.statValues.data[i].AvgValue]
                ]);

                var options = {
                    width: 550, height: 160,
                    min: 0,
                    max: $scope.statValues.data[i].MaxValue,

                    //redFrom: 90, redTo: 100,
                    //yellowFrom: 75, yellowTo: 90,
                    minorTicks: 5
                };
                var block_to_insert;
                var container_block;
                var individual_container;
                var text_to_insert;
                var gauge_chart_title_div;
                var gauge_chart_title_text;
                individual_container = document.createElement('div');
                block_to_insert = document.createElement('div');
                gauge_chart_title_div = document.createElement('div');
                gauge_chart_title_div.id = ('gauge_div');
                gauge_chart_title_div.style = ('width: 100%; margin-top: 10px; margin-bottom: 20px;');
                gauge_chart_title_text = document.createElement('h3');
                gauge_chart_title_text.innerHTML = ('Fields Min, Max & Avg Values ')
                text_to_insert = document.createElement('h3');
                text_to_insert.innerHTML = $scope.statValues.data[i].FieldName;
                block_to_insert.id = 'chart_div' + i.toString();
                individual_container.id = i.toString();

                if ((i % 2) == 0) {
                    individual_container.style = 'width: 515px; height: 240px; padding:20px; float: left; background-color: #e1e3e8;';
                }
                else {
                    individual_container.style = 'width: 515px; height: 240px; padding:20px; float: left; background-color: lightgrey;';
                }

                if (i == 0) {//if there is at least one numeric field Create Chart Title  
                    container_block = document.getElementById('container');
                    gauge_chart_title_div.appendChild(gauge_chart_title_text);
                    container_block.appendChild(gauge_chart_title_div);
                };

                container_block = document.getElementById('container');
                individual_container.appendChild(text_to_insert);
                individual_container.appendChild(block_to_insert);
                container_block.appendChild(individual_container);
                container_block.style = ('border-top: 3px solid lightgrey')




                var chart = new google.visualization.Gauge(document.getElementById('chart_div' + i.toString()));

                chart.draw(data, options);
            };
        } else {
            //doing nothing 
        };

        //setInterval(function () {
        //    data.setValue(0, 1, 40 + Math.round(60 * Math.random()));
        //    chart.draw(data, options);
        //}, 13000);
        //setInterval(function () {
        //    data.setValue(1, 1, 40 + Math.round(60 * Math.random()));
        //    chart.draw(data, options);
        //}, 5000);
        //setInterval(function () {
        //    data.setValue(2, 1, 60 + Math.round(20 * Math.random()));
        //    chart.draw(data, options);
        //}, 26000);
    };

    function drawDatesPieChart() {
        if ($scope.dateStatValues.data.length != 0) {
        var fieldName;
        var counter = 0;
        var counterArray = new Array();

        for (var i = 0; i < $scope.dateStatValues.data.length; i++) {
            if (fieldName != $scope.dateStatValues.data[i].ColumnName) {
                counter = i;
                counterArray.push(counter);
                fieldName = $scope.dateStatValues.data[i].ColumnName;
            }
            else {
                counter = i;
            }
        }
        counterArray.push($scope.dateStatValues.data.length);

        console.log('rafal : ' + counterArray);

        for (var i = 0; i < counterArray.length - 1; i++) {


            var data = new google.visualization.DataTable();


            data.addColumn('string', 'Year');
            data.addColumn('number', 'Count');
            data.addColumn({ type: 'string', role: 'tooltip' });
            //data.addColumn({ type: 'string', role: 'annotation' });

            for (var x = counterArray[i]; x < counterArray[i + 1]; x++) {
                data.addRow([$scope.dateStatValues.data[x].year.toString(), $scope.dateStatValues.data[x].year_count, 'There is ' + $scope.dateStatValues.data[x].year_count + ' records in ' + $scope.dateStatValues.data[x].year]); //, 'Populated: \n' + dataValues.data[i].PopulationCount.toString() + '\n out of \n' + numOfRecords + '\n records', +dataValues.data[i].PopulationPercentage + '% '
            }

            //var view = new google.visualization.DataView(data);
            //view.setColumns([0, 1, 1, 2]);

            // Set chart options
            var options =
                {
                    'title': 'How many records in column \n "' + $scope.dateStatValues.data[counterArray[i]].ColumnName + '" \n are in Particular Year',
                    'titleFontSize': 20,
                    'fontSize': 14,// chart font size
                    'titlePosition': '',
                    'width': 600,
                    'height': 450,
                    //'backgroundColor':'#b0d6f9', 
                    'backgroundColor': {
                        'fill': 'transparent',
                        'opacity': 100 // not sure if that is working - didn't tested
                    },
                    'is3D': true
                };

            var block_to_insert;
            var container_block;
            var individual_container;
            //var text_to_insert;
            var pie_chart_title_div;
            var pie_chart_title_text;
            individual_container = document.createElement('div');
            block_to_insert = document.createElement('div');
            block_to_insert.id = 'chartPie_div' + i.toString();
            pie_chart_title_div = document.createElement('div');
            pie_chart_title_div.id = ('PieChart_div');
            pie_chart_title_div.style = ('width: 100%; padding-top: 10px; margin-top: 10px; margin-bottom: 20px; margin-top: 20px; border-top: 3px solid lightgrey');
            pie_chart_title_text = document.createElement('h3');
            pie_chart_title_text.innerHTML = ('Years Pie Chart ')
            //text_to_insert = document.createElement('h3');
            //text_to_insert.innerHTML = $scope.dateStatValues.data[counterArray[i]].ColumnName;

            individual_container.id = i.toString();
            individual_container.style = 'float: left';

            //if ((i % 2) == 0) {

            //}
            //else {
            //    individual_container.style = 'width: 515px; height: 240px; padding:20px; float: left; background-color: lightgrey;';
            //}

            if (i == 0) {//if there is at least one numeric field Create Chart Title  
                container_block = document.getElementById('containerPieChart');
                pie_chart_title_div.appendChild(pie_chart_title_text);
                container_block.appendChild(pie_chart_title_div);
            };

            container_block = document.getElementById('containerPieChart');
            //individual_container.appendChild(text_to_insert);
            individual_container.appendChild(block_to_insert);
            container_block.appendChild(individual_container);
          //  container_block2.style = ('border-top: 3px solid lightgrey')

            // Instantiate and draw our chart, passing in some options  
            var chart = new google.visualization.PieChart(document.getElementById('chartPie_div' + i.toString()));

            chart.draw(data, options);
        };
        } else {
                //doing nothing 
               }
            };
        
    });



//Service to Get the Fields Population % and Count
ChartsPage.factory('TableFieldsPopulation', ['$http', function ($http) {



        var TableFieldsPopulation = {};

        TableFieldsPopulation.getTablePopulationData= function (id) {

            var geturl = '/MyLoads/GetTablePopulationData/' + id;

            return $http.get(geturl);

        }
        return TableFieldsPopulation;
    

}]);

//Service to Get the Total Number Of Records (based on file line count at this moment)
ChartsPage.factory('TableRecordsCount', ['$http', function ($http) {



    var TableRecordsCount = {};

    TableRecordsCount.getTotalNumberOfRecords = function (id) {

        var geturl = '/MyLoads/GetTableRecordsCount/' + id;

        return $http.get(geturl);

    }
    return TableRecordsCount;


}]);

//Service to Get Min, Max and Avg Values for each field of numeric type
ChartsPage.factory('TableMinMaxAvgValues', ['$http', function ($http) {



    var TableMinMaxAvgValues = {};

    TableMinMaxAvgValues.getTableMinMaxAvgValues = function (id) {

        var geturl = '/MyLoads/GetTableMinMaxAvgValues/' + id;

        return $http.get(geturl);

    }
    return TableMinMaxAvgValues;


}]);

//Service to Get Date Type Values for PieChart
ChartsPage.factory('TableDateTypeFieldsData', ['$http', function ($http) {



    var TableDateTypeFieldsData = {};

    TableDateTypeFieldsData.getTableDateTypeFieldsData = function (id) {

        var geturl = '/MyLoads/GetDateTypeFieldsData/' + id;

        return $http.get(geturl);

    }
    return TableDateTypeFieldsData;


}]);
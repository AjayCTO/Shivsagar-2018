'use strict';
app.controller('ordersController', ['$scope', 'ordersService', function ($scope, ordersService) {

    $scope.orders = [];

    ordersService.getOrders().then(function (results) {

        $scope.orders = results.data;

    }, function (error) {
       
    });

    $scope.order = localStorage.getItem("Customerorders");
    console.log("orders");
        $scope.orders= JSON.parse($scope.order);
    console.log($scope.orders);


    $scope.getorderitem = function (id) {
        debugger;
       
        $.ajax({
            url: serviceBase + '/api/CustomerOrders/GetOrderItem?UserName=' + authData.userName,
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
            }
        });
    }

}]);
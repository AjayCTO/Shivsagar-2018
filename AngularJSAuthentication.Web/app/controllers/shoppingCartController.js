'use strict';
app.controller('shoppingCartController', ['$scope','localStorageService', '$rootScope', function ($scope,localStorageService, $rootScope) {

    var _localCartItems = localStorage.getItem("shoppingCart");
   
    
    function LocalCartFiller() {
        _localCartItems = localStorage.getItem("shoppingCart");

        if (_localCartItems != null && _localCartItems != undefined) {
            _localCartItems = JSON.parse(_localCartItems);
        }
        else {
            _localCartItems = [];
        }   
        //$scope.shoppingCart = _localCartItems;
        $scope.TotalCartItems = _globalTotal;
        CheckScopeBeforeApply();
    }

    $scope.RemoveCartGlobal = function (item) {
        debugger;
        $rootScope.$emit("DeleteFromCart", item);
        setTimeout(function () { $scope.GetCart(); }, 2000);        
    }



    $scope.GetProductImageGlobal = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/ProductImages/" + Path;
        }
        return "../img/no-image.png";
    }
    //Get Product 
   

    //$scope.RemoveCartGlobal = function (item) {

    //    debugger;
    //    //$rootScope.$emit("DeleteFromCart", product);
    //    //LocalCartFiller();
    //    //$scope.CalculateCartGlobal($scope.shoppingCart);
    //    bootbox.confirm("Are you sure you want to delete this item from cart ?", function (result) {
    //        if (result) {

    //            var authData = localStorageService.get('authorizationData');

    //            if (authData != null) {
    //                $.ajax({
    //                    url: serviceBase + 'api/Cart/DeleteFromCart?UserName=' + authData.userName + '&id=' + item.id,
    //                    type: 'DELETE',
    //                    dataType: 'json',
    //                    success: function (data, textStatus, xhr) {
    //                        toastr.success("Success! Cart Updated");
    //                        $scope.CalculateCartGlobal();
    //                        $scope.GetCart();
                           

    //                    },
    //                    error: function (xhr, textStatus, errorThrown) {
    //                        alert("into error");
    //                    }
    //                });
    //            }
    //        }
    //    });

    //}

    $scope.CalculateCartGlobal = function (cart) {
        $rootScope.$emit("CalculateCart", cart);
        $scope.TotalCartItems = _globalTotal;

    }
    $scope.UpdateQuantity = function (type, Index) {
        debugger;
        var _Objcopy = angular.copy($scope.shoppingCart[Index]);
        debugger;
        if (type == 1) {
            if (_Objcopy.Quantity == _Objcopy.ProductQuantity) {
                alert("Product Reached to its maximum limit");
            }
            else {
                _Objcopy.Quantity = _Objcopy.Quantity + 1;
            }

        }
        debugger;
        if (type == 2) {
            if (_Objcopy.Quantity > 1) {

                _Objcopy.Quantity = _Objcopy.Quantity - 1;
            }
        }
        debugger;
        $scope.shoppingCart[Index] = _Objcopy;
        CheckScopeBeforeApply();
        localStorage.setItem("shoppingCart", JSON.stringify($scope.shoppingCart));
        LocalCartFiller();
        $scope.CalculateCartGlobal($scope.shoppingCart);
    }


    function CheckScopeBeforeApply() {
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

    $scope.GetCart = function () {
        var authData = localStorageService.get('authorizationData');
        debugger;
        $.ajax({
            url: serviceBase + 'api/Cart/GetCart?UserName=' + authData.userName,
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                debugger;
                console.log(data);

                $scope.CurrentCartList = data;

                $scope.shoppingCart = $scope.CurrentCartList;

                for (var i = 0; i < $scope.shoppingCart.length; i++) {
                    $scope.shoppingCart[i].Quantity = 1;
                }

                console.log($scope.CurrentCartList);
                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("into error");
            }
        });

    }
    function init() {
        $scope.GetCart();
        LocalCartFiller();
    }


    init();
}]);
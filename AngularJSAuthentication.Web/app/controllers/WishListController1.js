'use strict';
app.controller('WishListController1', ['$scope','$rootScope', '$location','localStorageService','authService', function ($scope,$rootScope, $location,localStorageService, authService) {

    $scope.CurrentWishList = [];
    $scope.CurrentWishList.length;
    $scope.GetProductImage = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/ProductImages/" + Path;
        }
        return "../img/no-image.png";
    }


    $scope.RemoveFromwishList = function (ID) {
        debugger;
        $rootScope.$emit("RemoveFromwishList", ID);
        setTimeout(function () { $scope.GetWishListfromService(); }, 2000);
    }

    $scope.AddToCartGlobal = function (productId, product, ID) {       
        $rootScope.$emit("AddToCart", productId, product, ID);
    }


    $scope.GetWishListfromService = function () {
        authService.GetWishList().then(function (response) {
            $scope.CurrentWishList = response;
        },
         function (err) {
             $scope.message = err.error_description;
         });
    }


    //$scope.GetWishList = function () {
    //    debugger;
    //    $rootScope.$emit("GetWishList");
    //}

    //$scope.GetWishList = function () {
    //    debugger;
    //    var authData = localStorageService.get('authorizationData');
    //    if (authData != null)
    //    { 
    //        $.ajax({
    //            url: serviceBase + 'api/CustomerWishlist/GetWishLists',
    //            data: { UserName: authData.userName },
    //            type: 'GET',
    //            dataType: 'json',
    //            success: function (data, textStatus, xhr) {
    //                debugger;
    //                $scope.CurrentWishList = data;
    //                $scope.$apply();
    //            },
    //            error: function (xhr, textStatus, errorThrown) {
    //                debugger;
    //                alert("into error wishlist");
    //            }
    //        });
    //    }
    //}



    //$scope.RemoveFromwishList = function (ID) {
    //    authService.RemoveFromwishList(ID).then(function (response) {
    //        $scope.GetWishListfromService();
    //        window.location.reload();
    //    },
    //     function (err) {
    //         $scope.message = err.error_description;
    //     });
    //}

    //$scope.RemoveFromwishList = function (ID) {
    //    debugger;
    //    $.ajax({
    //        url: serviceBase + 'api/WishlistDelete/DeleteWishList?id=' + ID,
    //        type: 'POST',
    //        dataType: 'json',
    //        success: function (data) {
    //            debugger;
    //            if (data.success == true) {
    //                toastr.success("Success! Wishlist Updated")
    //                $scope.GetWishList();
    //            }
    //        },
    //        error: function (data) {
    //            alert("into error");
    //        }
    //    });
    //};


    function init() {
       // $scope.GetWishList();
        $scope.GetWishListfromService();
    }

    init();
   
}]);
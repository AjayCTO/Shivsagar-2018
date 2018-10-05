

app.controller("CustomerCtrl", function ($scope, $rootScope, AddToCart, CartToCookieService) {

    $scope.CustomerId = customerId;
    $scope.wishList = [];
    $scope.AddToCartText = "";
    $scope.AllCartItems = [];
    var items = CartToCookieService.getCookieData();
    if (items != undefined) {

        $scope.AllCartItems = items;


        $scope.CartProductsCounter = $scope.AllCartItems.length;
    }

    //$scope.GetWishListItems = function () {
    //    $.ajax({
    //        url: '/api/WishLists/GetWishLists',
    //        type: 'GET',
    //        dataType: 'json',
    //        data: { UserID: $scope.CustomerId },
    //        success: function (data, textStatus, xhr) {
    //            debugger;
    //            $scope.wishList = data;
    //            console.log($scope.wishList);

    //            $scope.$apply();
    //        },
    //        error: function (xhr, textStatus, errorThrown) {
    //            return items = [];
    //        }
    //    });
    //};

    $scope.AddToCart = function (productId, product) {
        debugger;
        alert("call from wish list by me");


        var item = $scope.AllCartItems.filter(function (item) {
            if (item.ProductId === product.ProductId) {
                item.Quantity = item.Quantity + 1;
            }
            return item.ProductId === product.ProductId;
        })[0];

              if (item == undefined) {
            $scope.CartProductsCounter++;
            //$('html, body').animate({
            //    'scrollTop': $(".cart_anchor").position().top
            //});
            //var itemImg = $("#pid" + productId).parent().find('img').eq(0);
            $scope.AllCartItems.push({ ProductId: product.ProductId, Image: product.Image, Quantity: 1, ProductName: product.ProductName, Cost: product.ProductPrice, discount: 0 });
       
     

              }
        CartToCookieService.setCookieData($scope.AllCartItems);
        $rootScope.$emit("CallGetCookieData", {});
        $scope.RemoveFromwishList(product.Id);
    };

    //$scope.AddToCartFromWishList = function () {
    //    alert("into wish list");
    //    $scope.AddToCart(_TempProductID, _tempProductData);
    //}
    //$scope.GetWishListItems();

    $scope.RemoveFromwishList = function (ID) {
        debugger;
        $.ajax({
            url: '/Home/DeleteWishList?id='+ID,
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                if (data.Success == true) {

                    toastr.success("Product Added in Your Cart");
                    window.location.href = '/Home/Products';
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("into error");
            }
        });
    };
});


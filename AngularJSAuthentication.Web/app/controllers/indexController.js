'use strict';
app.controller('indexController', ['$scope', '$rootScope', 'localStorageService', '$location', 'authService', function ($scope, $rootScope, localStorageService, $location, authService) {
   
    $scope.searchcategories = [];
    var _localCategories = localStorage.getItem("Categories");
    if (_localCategories != null && _localCategories != undefined) {
        _localCategories = JSON.parse(_localCategories);

    }
    else {
        _localCategories = [];
    }
    $scope.shoppingCart = [];
    $scope.TotalOfCartItems = 0;

    var _localCartItems = localStorage.getItem("shoppingCart");
    if (_localCartItems != null && _localCartItems != undefined) {
        _localCartItems = JSON.parse(_localCartItems);
       
    }
    else {
        _localCartItems = [];
    }
    //$scope.shoppingCart = _localCartItems;
  
    //CalculateTotal();

    $scope.logOut = function () {
        $scope.shoppingCart = []
        authService.logOut();
        $location.path('/home');
    }

  //  $scope.$emit("CategoryID", CATID);


    $rootScope.$on("GetCategories", function () {
        $scope.GetCategories();
    });

    $rootScope.$on("AddToCart", function (event, productId, product, ID) {
        $scope.AddToCart(productId, product, ID);
    });

    $rootScope.$on("addTowishList", function (event, productId) {
        $scope.addTowishList(productId);
    });

    //$rootScope.$on("GetWishList", function (event) {
    //    $scope.GetWishList();
    //});


    $rootScope.$on("DeleteFromCart", function (event, product) {
        $scope.DeleteFromCart(product);

    });


    $rootScope.$on("RemoveFromwishList", function (event, ID) {
        $scope.RemoveFromwishList(ID);

    });

    $rootScope.$on("CalculateCart", function (event, cart) {
        CalculateTotal(cart);
    });
    
    $scope.GetCategories = function () {
       
        $scope.loadallcat = false;

        $.ajax({
            url: serviceBase + '/api/Categories/GetCategories',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                $scope.searchcategories = data;
                localStorage.setItem("Categories", JSON.stringify(data));

               
                $scope.loadallcat = true;
                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.categories = [];
            }
        });
    };


    $scope.GetCart = function () {
        var authData = localStorageService.get('authorizationData');
       
        if (authData != null)
        {
            $.ajax({
                url: serviceBase + '/api/Cart/GetCart?UserName=' + authData.userName,
                type: 'GET',
                dataType: 'json',
                success: function (data, textStatus, xhr) {
                    debugger;
                    $scope.CurrentCartList = data;
                    $scope.shoppingCart = $scope.CurrentCartList;
                    for (var i = 0; i < $scope.shoppingCart.length; i++) {
                        $scope.shoppingCart[i].Quantity = 1;
                    }
                    console.log("==========================$scope.CurrentCartList==============================");
                    console.log($scope.CurrentCartList);
                    CalculateTotal(data);
                    $scope.$apply();
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("GetCart error");
                }
            });
        }
    }

    $scope.GoToProductsWithCategoryID = function (ID) {

        debugger;
         localStorage.setItem("filterCategoryID", ID);

      //  $rootScope.$emit("CategoryID", ID);
         $location.path('/Product');

         $("#Dummylink").trigger('click');

    }


    $scope.GetProductImageGlobal = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/ProductImages/" + Path;
        }
        return "../img/no-image.png";
    }

   

    $scope.RemoveFromwishList = function (ID) {
        authService.RemoveFromwishList(ID).then(function (response) {
            $scope.GetWishListfromService();            
        },
         function (err) {
             $scope.message = err.error_description;
         });
    }




    $scope.DeleteFromCart = function (Product) {       
        bootbox.confirm("Are you sure you want to delete this item from cart ?", function (result) {
            if (result) {
                for (var i = 0; i < $scope.shoppingCart.length; i++) {
                    if ($scope.shoppingCart[i].ProductId == Product.ProductId) {
                        $scope.shoppingCart.splice($.inArray(Product, $scope.shoppingCart), 1);
                    }
                }

                //localStorage.setItem("shoppingCart", JSON.stringify($scope.shoppingCart));

                var authData = localStorageService.get('authorizationData');

                $.ajax({
                    url: serviceBase + 'api/Cart/DeleteFromCart?UserName=' + authData.userName + '&id=' + Product.id,
                    type: 'DELETE',
                    dataType: 'json',
                    success: function (data, textStatus, xhr) {   
                        toastr.success("Success! Remove from cart");
                        $scope.GetCart();
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        alert("into error");
                    }
                });

     
                CheckScopeBeforeApply();
                CalculateTotal();

                return false;
            }
        });
    };


    function CalculateTotal(cart) {
        debugger;
        $scope.shoppingCart = cart==undefined || cart==null ?$scope.shoppingCart:cart;
        var total = 0;

        for (var i = 0; i < $scope.shoppingCart.length; i++) {
            var product = $scope.shoppingCart[i];
            total += (product.productPrice * product.Quantity);
        }
        $scope.TotalOfCartItems = total;
        _globalTotal = total;
        CheckScopeBeforeApply();
    }
    if (_localCategories.length == 0) {
        $scope.GetCategories();

    }
    else {
        $scope.searchcategories = _localCategories;
    }
    $scope.authentication = authService.authentication;


    function Animate2Item(originalID) {
        $(originalID).animate_from_to('.cart', {
            pixels_per_second: 700,
            initial_css: {
                'background': 'rgb(214, 209, 216,0.5)',
                'border-radius': '100%'
            }
        });

        //$('html, body').animate({
        //    'scrollTop': $(".cart").position().top
        //},1500);
    }


    function CheckScopeBeforeApply() {
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

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
    //    var authData = localStorageService.get('authorizationData');
    //    if (authData != null)
    //    {
    //        debugger;
    //        $.ajax({
    //            //url: serviceBase + 'api/CustomerWishlist/GetWishLists?UserName=' +authData.userName,
    //            url: serviceBase + 'api/CustomerWishlist/GetWishLists',
    //            data: {UserName: authData.userName},
    //            type: 'GET',
    //            dataType: 'json',
    //            success: function (data, textStatus, xhr) {
    //                debugger;
    //                console.log(data);

    //                $scope.CurrentWishList = data;

    //                console.log($scope.CurrentWishList);
    //                $scope.$apply();
    //            },
         
    //        });
    //    }       

    //}

   

    $scope.CheckisInWishList = function (ProductID) {
        debugger;
        for (var i = 0; i < $scope.CurrentWishList.length; i++) {
            if ($scope.CurrentWishList[i].productId == ProductID) {
                return "active";
            }
        }
        return "";
    }


    $scope.CheckisInCart = function (ProductID) {
        debugger;
        for (var i = 0; i < $scope.CurrentCartList.length; i++) {
            if ($scope.CurrentCartList[i].productId == ProductID) {
                return "active";
            }
        }
        return "";
    }

    $scope.addTowishList = function (productId) {
      
        debugger;     
        var authData = localStorageService.get('authorizationData');        
        if (authData == null) {
            //Location.Path("/login");
            $location.path("/login");
        }
        else {
            if ($scope.CheckisInWishList(productId) == "active") {
                debugger;
                for (var i = 0; i < $scope.CurrentWishList.length; i++) {
                    if ($scope.CurrentWishList[i].productId == productId) {
                        toastr.warning("Already added in you wishlist");
                        break;
                    }
                }
            }
            else
            {

                var wishListmodel = { ProductId: productId, CustomerId: -1, UserID: authData.userName };
                $.ajax({
                    url: serviceBase + '/api/CustomerWishlist/PostWishList',
                    type: 'POST',
                    data: wishListmodel,
                    dataType: 'json',
                    success: function (data) {
                        debugger;
                        console.log(data);
                        if (data.success == true) {
                            $scope.iconclass = "angel icon-heart";
                            toastr.success("Product successfully added in wishlist");
                            $("#exampleModal").modal("hide");
                            //$scope.GetWishList();
                            $scope.GetWishListfromService();
                            $scope.$apply();
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                    }
                });
            }
        }
        //else {
        //    debugger;
        //    $("#exampleModal").modal("hide");
        //    $(".modal-backdrop").remove();
        //    debugger;
        //    $location.path('/login');

        //}
        //var cid = customerId;

        //if (cid == null || cid === " " || cid == '') {
        //    localStorage.setItem("WishListProductID", productId);

        //    window.location.href = '/account/login';

        //}
        //else {

        //    if ($scope.CheckisInWishList(productId) == "active") {
        //        for (var i = 0; i < $scope.CurrentWishList.length; i++) {
        //            if ($scope.CurrentWishList[i].productId == productId) {

        //                $scope.RemoveFromwishList($scope.CurrentWishList[i].id);
        //                break;

        //            }

        //        }

        //    }
        //    else {


        //        var wishListmodel = { ProductId: productId, CustomerId: -1, UserID: customerId };
        //        $.ajax({
        //            url: serviceBase + 'api/CustomerWishlist/PostWishList',
        //            type: 'POST',
        //            data: wishListmodel,
        //            dataType: 'json',
        //            success: function (data) {
        //                debugger;
        //                console.log(data);
        //                if (data.success == true) {
        //                    alert("Add Product in Your Wishlist");

        //                    $scope.iconclass = "angel icon-heart";
        //                    $scope.GetWishList("188cd426-f277-4160-b006-13084388d583 ");
        //                    $scope.$apply();
        //                }

        //            },
        //            error: function (xhr, textStatus, errorThrown) {

        //            }
        //        });
        //    }
        //}
    };

    $scope.AddToCart = function (productId, product, ID) {
        debugger;
        var authData = localStorageService.get('authorizationData');        
        if (authData == null) {
            //Location.Path("/login");
            $location.path("/login");
        }
        else {
            if ($scope.CheckisInCart(productId) == "active") {
                toastr.warning("Already added in you Cart.Please update quantity from cart.");              
            }
            else
            {
                debugger;
                if (product[5] !== 0) {
                    var item = $scope.shoppingCart.filter(function (item) {
                        if (item.productId === product[8]) {
                            item.Quantity = item.Quantity + 1;
                        }
                        return item.productId === product[8];
                    })[0];
                    if (item == undefined) {
                        $scope.CartProductsCounter++;
                        $scope.shoppingCart.push({ ProductId: product[8], Image: product[2], Quantity: 1, ProductName: product[3], ProductQuantity: product[5], Cost: product[4], discount: 0, SupplierID: product[11] });

                    }
                    Animate2Item("#" + ID + productId);

                    //localStorage.setItem("shoppingCart", JSON.stringify($scope.shoppingCart));
                    debugger;
                    var cartModel = { ProductId: productId, CustomerId: -1, UserID: authData.userName };
                    $.ajax({

                        url: serviceBase + 'api/Cart/PostCart',
                        type: 'POST',
                        data: cartModel,
                        dataType: 'json',
                        success: function (data) {
                            debugger;
                            if (data.success == true) {
                                toastr.success("Success! Product Added into your cart");
                                $("#exampleModal").modal("hide");
                                $scope.$apply();
                                $scope.GetCart();

                            }
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            alert("into error");
                        }
                    });
                    CheckScopeBeforeApply();
                    CalculateTotal();
                }
                else {
                    toastr.error("You can't Add this Item becasue it is Not Available in Stock");
                }
            }
        }        
    };


    function init() {
        $scope.GetCart();
        $scope.GetWishListfromService();
        //$scope.GetWishList();
    }


    init();

}]);
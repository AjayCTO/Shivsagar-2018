'use strict';
app.controller('homeController', ['$scope', '$rootScope','localStorageService', '$location', function ($scope, $rootScope,localStorageService, $location) {
    $scope.iconclass = "icon-heart"
    $scope.searchcategoriesslider = [];
    $scope.pagedItems = [];
    $scope.CurrentWishList = [];
    $scope.MostSaledItems = [];
    $scope.TopRatedItems = [];
    $scope.ProductDetail = {};
    $scope.childmethod = function () {
        $rootScope.$emit("GetCategories", {});
    }


    $scope.loadmostsale = false
    $scope.loadtop = false
    $scope.loadfeatured = false

    $scope.AddToCartGlobal = function (productID, product, ID) {       
        $rootScope.$emit("AddToCart", productID, product, ID);
    }

    $scope.addTowishList = function (productID) {       
        $rootScope.$emit("addTowishList", productID);
    }

    $scope.GetWishList = function () {
        $rootScope.$emit("GetWishList");
    }
   

    //$rootScope.$on("AddToWishlists", function (productId, customerId)
    //{
    //    alert("in");
    //    $scope.addTowishList(productId, customerId);
    //});
    var _localCategories = localStorage.getItem("Categories");
    if (_localCategories != null && _localCategories != undefined) {
        
        _localCategories = JSON.parse(_localCategories);

    }
    else {
         $scope.childmethod();
        _localCategories = localStorage.getItem("Categories")
        _localCategories = JSON.parse(_localCategories);       
    }


    localStorage.setItem("filterCategoryID", "");


    $scope.searchcategoriesslider = _localCategories;
 

  

    $scope.GetcatBgImage = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/CategoryImage/" + Path;
        }
        return "../img/nocategory.png";
    }

    $scope.SetProduct = function (product) {
        localStorage.setItem("ProductDetail", JSON.stringify(product));
        $location.path('/ProductDetail');
    }

    function productSlider(className) {
        // ------------------------------------------------------- //
        // Products Slider
        // ------------------------------------------------------ //
        $(className).owlCarousel({
            loop: false,
            margin: 10,
            dots: false,
            nav: true,
            smartSpeed: 400,
            responsiveClass: true,
            navText: ['<i class="fa fa-long-arrow-left"></i>', '<i class="fa fa-long-arrow-right"></i>'],
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 2
                },
                1000: {
                    items: 4
                }
            }
        });
    }

    $scope.GetProductImage = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/ProductImages/" + Path;
        }
        return "../img/no-image.png";
    }
    $scope.pagedItems = [];
    $scope.total = 0;

    $scope.GetSupplierImage = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/SupplierImage/" + Path;
        }
        return "../img/no-image.png";
    }
    $scope.GetSuppliers = function () {
        $.ajax({
            url: serviceBase + '/api/Supplier/GetSuppliers',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {


                debugger;
                $scope.Suppliers = data;

                console.log("supplier");
                console.log($scope.Suppliers);

                $scope.$apply();

                $('.brands-slider').owlCarousel({
                    loop: true,
                    margin: 20,
                    dots: true,
                    nav: false,
                    smartSpeed: 800,
                    responsiveClass: true,
                    responsive: {
                        0: {
                            items: 2
                        },
                        600: {
                            items: 3
                        },
                        1000: {
                            items: 3,
                            loop: false
                        }
                    }
                });


            },
            error: function (xhr, textStatus, errorThrown) {


                debugger;
            }
        });
    }

    $scope.GetFeaturedProducts = function (_Type) {
        debugger;
        var _isFeatured = "";
        var _MostSale = "";
        var _topRated = "";
        var _displaylength = 10;
        var _ClassName = "";
        switch (_Type) {
            case 1:
                _isFeatured = "1";
                _displaylength = 100;
                break;
            case 2:
                _MostSale = "1";
                _displaylength = 100;
                break;
            case 3:
                _topRated = "1";
                _displaylength = 3000;
                break;
            default:

        }
        var _model = { displayLength: _displaylength, displayStart: 0, searchText: "", filtertext: "", Categories: "", lowprice: "", highprice: "", isFeatured: _isFeatured, isMostSale: _MostSale, TopRated: _topRated };
        $.ajax({
           
            url: serviceBase + 'api/Product/Post',
            type: 'POST',
            dataType: 'json',
            data: _model,
            success: function (data, textStatus, xhr) {
                debugger;
                $scope.total = data.iTotalDisplayRecords;
                

                switch (_Type) {
                    case 1:
                        $scope.pagedItems = data.aaData;
                        _ClassName = '.products-slider';
                        break;
                    case 2:
                        $scope.MostSaledItems = data.aaData;
                        _ClassName = '.mostsaled-slider';

                        break;
                    case 3:
                        $scope.TopRatedItems = data.aaData;
                        _ClassName = '.topRated-slider';
                        console.log("toprated");
                        console.log($scope.TopRatedItems);
                        break;
                    default:

                }

                $scope.loadmostsale = true
                $scope.loadtop = true
                $scope.loadfeatured = true
                $scope.$apply();
                productSlider(_ClassName);

            },
            error: function (xhr, textStatus, errorThrown) {

           
            }
        });
    }

    function CheckScopeBeforeApply() {
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

    
    $scope.getProductDetail = function (_Product) {
        $scope.ProductDetail = _Product;
        CheckScopeBeforeApply();
        $("#exampleModal").modal("show");
    }


    function init() {
        
        $scope.GetFeaturedProducts(1);
        $scope.GetFeaturedProducts(2);
        $scope.GetFeaturedProducts(3);
        $scope.GetSuppliers();
        $scope.GetWishList();
    }

    //add to wishlist

   
    $scope.RemoveFromwishList = function (ID) {
        debugger
        $.ajax({
            url: serviceBase + 'api/WishlistDelete/DeleteWishList?id=' + ID,
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success == true) {
                    toastr.success("Success! Remove Product in  Wishlist");
                    $scope.GetWishList();
                    $scope.$apply();
                }
            },
            error: function (data) {
               
            }
        });
    };



    $scope.CheckisInWishList = function (ProductID) {
        for (var i = 0; i < $scope.CurrentWishList.length; i++) {
            if ($scope.CurrentWishList[i].productId == ProductID) {
                return "active";
            }

        }

        return "";
    }


    //$scope.addTowishList = function (productId) {
    //    debugger;     
    //    var authData = localStorageService.get('authorizationData');        
    //    if (authData != null) {
    //        var wishListmodel = { ProductId: productId, CustomerId: -1, UserID: authData.userName };
    //        $.ajax({
    //            url: serviceBase + 'api/CustomerWishlist/PostWishList',
    //            type: 'POST',
    //            data: wishListmodel,
    //            dataType: 'json',
    //            success: function (data) {
    //                debugger;
    //                console.log(data);
    //                if (data.success == true) {
    //                    $scope.iconclass = "angel icon-heart";
    //                    $scope.GetWishList();
    //                    $scope.$apply();
    //                }
    //            },
    //            error: function (xhr, textStatus, errorThrown) {
    //            }
    //        });
    //    }
    //    //var cid = customerId;

    //    //if (cid == null || cid === " " || cid == '') {
    //    //    localStorage.setItem("WishListProductID", productId);

    //    //    window.location.href = '/account/login';

    //    //}
    //    //else {

    //    //    if ($scope.CheckisInWishList(productId) == "active") {
    //    //        for (var i = 0; i < $scope.CurrentWishList.length; i++) {
    //    //            if ($scope.CurrentWishList[i].productId == productId) {
                     
    //    //                $scope.RemoveFromwishList($scope.CurrentWishList[i].id);
    //    //                break;
                         
    //    //            }

    //    //        }

    //    //    }
    //    //    else {


    //    //        var wishListmodel = { ProductId: productId, CustomerId: -1, UserID: customerId };
    //    //        $.ajax({
    //    //            url: serviceBase + 'api/CustomerWishlist/PostWishList',
    //    //            type: 'POST',
    //    //            data: wishListmodel,
    //    //            dataType: 'json',
    //    //            success: function (data) {
    //    //                debugger;
    //    //                console.log(data);
    //    //                if (data.success == true) {
    //    //                    alert("Add Product in Your Wishlist");
                     
    //    //                    $scope.iconclass = "angel icon-heart";
    //    //                    $scope.GetWishList("188cd426-f277-4160-b006-13084388d583 ");
    //    //                    $scope.$apply();
    //    //                }
                       
    //    //            },
    //    //            error: function (xhr, textStatus, errorThrown) {
                      
    //    //            }
    //    //        });
    //    //    }
    //    //}
    //};


   
    init();
}]);
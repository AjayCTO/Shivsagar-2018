'use strict';
app.controller('ProductDetailController', ['$scope','$rootScope','localStorageService', '$location', 'authService', function ($scope,$rootScope,localStorageService, $location, authService) {

    $scope.product = [];
    $scope.Images = [];
    $scope.CurrentWishList = [];

    $scope.emailvalidation = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    $scope.IsProductLoading = false;
    var _localProductDetail = localStorage.getItem("ProductDetail");
    if (_localProductDetail != null && _localProductDetail != undefined) {
        _localProductDetail = JSON.parse(_localProductDetail);
    }
    else {
        _localProductDetail = [];
    }
    var slider = false;
    var _AttributesName = [];
    $scope.AttributesName = [];
    $scope.Tempproduct = _localProductDetail;


    function CheckScopeBeforeApply() {
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

    function init() {
        $scope.GetProductDetail($scope.Tempproduct[8]);
        $scope.GetRelatedPrdoucts($scope.Tempproduct[8])
        $scope.GetReviews($scope.Tempproduct[8]);

    }

    $scope.AddToCartGlobal = function (productID, product, ID) {        
        $rootScope.$emit("AddToCart", productID, product, ID);
    }


    $scope.addTowishList = function (productID) {
        $rootScope.$emit("addTowishList", productID);
    }

    //$scope.AddTowishlistGlobal = function (productId, customerId) {
    //    debugger;
    //    $rootScope.$emit("AddToWishlists", productId, customerId);
    //};

    function applyslider() {
        slider = $('.items-slider').owlCarousel({
            loop: true,
            items: 1,
            thumbs: true,
            thumbsPrerendered: true,
            dots: false,
            responsiveClass: false
        });
    }
    $scope.GetProductImage = function (Path) {
        if ($.trim(Path) != "") {
            return _GlobalImagePath + "/ProductImages/" + Path;
        }
        return "../img/no-image.png";
    }
    $scope.GetProductDetail = function (ID) {
        var _model = { ProductId: ID }
        $.ajax({
            url: serviceBase + 'api/Product/GetProductDetail',
            type: 'GET',
            dataType: 'json',
            data: _model,
            success: function (data, textStatus, xhr) {
                $scope.product = data;
                CheckScopeBeforeApply();

                console.log("productid");
                console.log(data);
                $scope.AttributesData = $scope.product.product.attributes;
                _AttributesName = $scope.product.allAttributes;


                for (var i = 0; i < _AttributesName.length; i++) {
                    $scope.AttributesName.push({ Label: _AttributesName[i], Value: "" });
                }

                $scope.IsProductLoading = true;
                CheckScopeBeforeApply();
                $("#descriptiontab").trigger("click");
                $scope.Images = $scope.product._AllProductImages;
                CheckScopeBeforeApply();
                applyslider();

            },
            error: function (xhr, textStatus, errorThrown) {

                alert(xhr.error);
            }
        });
    }
    $('.dec-btn').on("click", function () {
        var siblings = $(this).siblings('input.quantity-no');
        if (parseInt(siblings.val(), 10) >= 1) {
            siblings.val(parseInt(siblings.val(), 10) - 1);
        }
    });


    $('.inc-btn').on("click", function () {
        var siblings = $(this).siblings('input.quantity-no');
        siblings.val(parseInt(siblings.val(), 10) + 1);
    });

    function CheckIfValueAvailable(Column, Value) {
        var _attributesDataArray = $scope.AttributesData;

        for (var i = 0; i < _attributesDataArray.length; i++) {
            if (_attributesDataArray[i].attributeName == Column && _attributesDataArray[i].attributeValue == Value) {
                return i;
            }

        }

        return -1;
    }

    function SetImages(Id) {
        var _array = [];
        debugger;
        for (var i = 0; i < $scope.Images.length; i++) {
            if ($scope.Images[i].productQuantityId == Id) {
                _array.push($scope.Images[i]);
            }

        }

        $scope.Images = _array.length > 0 ? _array : $scope.Images;
        CheckScopeBeforeApply();
        applyslider();
    }
    $scope.GetPrice = function (Value, Label) {
        var _attributesDataArray = $scope.AttributesData;
        var _attributesArray = [];
        var _attributeNameArray = _AttributesName;

        debugger;
        for (var i = 0; i < _attributeNameArray.length; i++) {
            var _ID = "#attribute" + (i + 1);
            _attributesArray.push({ ColumnName: $(_ID).attr("attributename"), Value: $(_ID).val() })

        }

        var _TempArray = [];

        for (var i = 0; i < _attributesArray.length; i++) {
            var _value = CheckIfValueAvailable(_attributesArray[i].ColumnName, _attributesArray[i].Value);
            if (_value != -1) {
                _TempArray.push(_value);
            }

        }

        var _Quantity = -1;

        if (_TempArray.length == _attributesArray.length) {
            $(".quantityFigure1").attr("data-max", _attributesDataArray[_TempArray[_TempArray.length - 1]].quantity);
            $("#Productprice").html(_attributesDataArray[_TempArray[_TempArray.length - 1]].cost);
            SetImages(_attributesDataArray[_TempArray[_TempArray.length - 1]].productQuantityId)

        }




    }
    $scope.GetAttributesData = function (Data) {
        var _array = [];
        for (var i = 0; i < $scope.AttributesData.length; i++) {
            if ($scope.AttributesData[i].attributeName == Data) {
                if ($.trim($scope.AttributesData[i].attributeValue)) {
                    var itemdata = _array.filter(function (item) {

                        return item === $scope.AttributesData[i].attributeValue;
                    })[0];

                    if (itemdata == undefined || itemdata == null) {
                        _array.push($scope.AttributesData[i].attributeValue);

                    }

                }
            }

        }

        return _array;
    }


    $scope.addtocart = function (productId, product) {
        var authData = localStorageService.get('authorizationData');


        if (authData != null) {
            var cartModel = { ProductId: productId, CustomerId: -1, UserID: authData.userName };
            $.ajax({
                url: serviceBase + 'api/Cart/PostCart',
                type: 'POST',
                data: cartModel,
                dataType: 'json',
                success: function (data) {
                    debugger;
                    if (data.success == true) {
                        $scope.iconclass = "angel icon-heart";
                        $scope.$apply();
                        //$scope.GetCart();

                        $scope.RemoveFromwishList(product.Id);
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("into error");
                }
            });

        }
    }

  

    $scope.GetRelatedPrdoucts = function (ProductId) {
        debugger;
        $.ajax({
            type: 'get',
            url: serviceBase + 'api/Shopping/GetRelatedProducts',
            data: { productID: ProductId },
            dataType: "json",
            success: function (resultData) {
                debugger;
                if (resultData.success == true) {

                    $scope.RelatedProducts = resultData.data;
                    $scope.$apply();
                    $(".owl-carousel.owl-theme.GetRelatedPrdoucts").owlCarousel({

                        nav: true, // Show next and prev buttons

                        dots: false,
                        items: $scope.RelatedProducts.length,
                        margin: 15,
                        navText: ['<i class="fa fa-long-arrow-left"></i>', '<i class="fa fa-long-arrow-right"></i>'],
                        responsiveClass: true,
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
                else {
                    alert(resultData.error);
                }
            }
        });
    }

    // review

    $scope.review = { ProductId:$scope.Tempproduct[8], Name: '', Email: '', Review: '', Rating: 0 };
    // review rating 
    $scope.GetSelectedClass = function (rating) {
        
        if ($scope.review.Rating == rating || $scope.review.Rating > rating) {
            return "text-primary";
        }
        return "";
    }
    //Add reviews

    $scope.reviews = [];
    $scope.reviews.length
    $scope.AddReview = function () {
        var reviews = $scope.review;
        $("#AddReview").addClass("disabled");
 
        $.ajax({
         
            type: 'POST',
            url:serviceBase+'/api/CustomerReviews/PostCustomerReview',
            data: reviews,
            dataType: "Json",
            success: function (resultData) {
                debugger;
                if (resultData.success == true) {
                 
                    $scope.review = { ProductId: $scope.Tempproduct[8], Name: '', Email: '', Review: '', Rating: 0 };

                    $("#AddReview").removeClass("disabled");
                    $scope.GetReviews($scope.Tempproduct[8]);
                }
                else {
                    alert(resultData.error);
                }
            }
        });
    };

    $scope.GetReviews = function (ProductId)
    {
        debugger;
        $.ajax({
            type: 'Get',
            url: serviceBase + 'api/CustomerReviews/GetCustomerReviews',
            data: { ProductId: ProductId },
            dataType: "json",
            success: function (resultData) {
                debugger;
                if (resultData.success == true)
                {

                    $scope.reviews = resultData.data;
                    console.log("reviews");
                    console.log($scope.reviews);
                    $scope.$apply();
                }
                else {
                    alert(resultData.error);
                }
             }
        });
    };


    //

    $scope.GetWishList = function () {      
        var authData = localStorageService.get('authorizationData');
        if (authData != null)
        {
            $.ajax({
                url: serviceBase + 'api/CustomerWishlist/GetWishLists?UserName=' + authData.userName,
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    debugger;
                    $scope.CurrentWishList = data;
                    window.location.reload();
                    $scope.$apply();
                },
                error: function (xhr, textStatus, errorThrown) {

                }
            });
        }
    }

    $scope.RemoveFromwishList = function (ID) {
        debugger
        $.ajax({
            url: serviceBase + 'api/WishlistDelete/DeleteWishList?id=' + ID,
            type: 'POST',
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success == true) {
                    alert("remove Product in Your Wishlist");
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
    //    //                    $scope.GetWishList();
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


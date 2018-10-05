app.controller("PaginationCtrl", function ($scope, $rootScope, AddToCart, CartToCookieService) {

    //for attribulte show limit
    $scope.limit = 5;
    $scope.itemsPerPage = 1000;
    $scope.currentPage = 0;
    $scope.total = 0;

    //for attribulte icon
    $scope.iconclass = "+ Show More";
    $scope.alltotalproduct = 0;
    $scope.search = '';
    $scope.value = '';
    $scope.categoriesarraySelect = [];
    $scope.categoriesobj = '';
    $scope.pagedItems = [];
    $scope.CartProductsCounter = 0;
    $scope.CartItem = { ProductId: 0, Image: '', Quantity: 0, ProductName: "" };
    $scope.AllCartItems = [];
    $scope.Attributes = [];
    $scope.AttributesValue = [];
    $scope.categories = [];
    $scope.AllAttributeFilters = [];
    $scope.Suppliers = [];
    $scope.selectedAttribute = "";
    $scope.showloader = true;
    $scope.CurrentWishList = [];
    $scope.CustomerID = _customerID == undefined ? -1 : _customerID;
    var minpriceval = "";
    var maxpriceval = "";
    var searchtext = _GlobalSearchText;
    $scope.search = _GlobalSearchText;
    var _localstorageProductID = localStorage.getItem("WishListProductID");

    $scope.category = {
        "id": 0,
        "categoryName": null,
        "isActive": false,
        "sort": 0,
        "description": null,
        "notes": null,
        "supplier_Id": null,
        "parentCategory": null,
        "categoryImage": null,
        "categories1": null
    };

    $scope.FilterProducts = function () {
        $scope.itemsPerPage = 1000;
        $scope.currentPage = 0;
        $scope.total = 0;
        $scope.pagedItems = [];
        $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);

    };

    //$scope.loadMoreattribute = function (id) {
    //    debugger;
    //    for (var i = 0; i < $scope.AttributesValue.length; i++) {
    //        if ($scope.AttributesValue[i].AttributeId === id) {
    //            var hee = $scope.AttributesValue[i].AttributeId.length;
    //        }
    //    }

    //    $scope.limit = hee.length;
    //}
    //$scope.loadMoreattribute = function (id) {
    //    debugger;
    //    for (var i = 0; i < $scope.AllAttributeFilters.length; i++)
    //        if ($scope.AllAttributeFilters[i].Name === name) {
    //            $scope.limit = $scope.AttributesValue.length;
    //        }
    //}

    $scope.loadMoreattribute = function (name) {
        debugger;
        var attrlength = $("." + name).children('li').length - 1;
        alert($("." + name).children('li').length - 1);
        if ($scope.limit != 5) {
            $scope.limit = 5;
            $scope.iconclass = "+ Show More";

        }
        else {
            $scope.limit = $scope.AttributesValue.length;
            $scope.iconclass = "- Show less";

        }
    }
    function AddCatID(CategoryId) {
        var idx = $scope.categoriesarraySelect.indexOf(CategoryId);
        if (idx > -1) {
            // is currently selected
            $scope.pagedItems = [];
            $scope.currentPage = 0;

            $scope.categoriesarraySelect.splice(idx, 1);
        }
        else {
            // is newly selected
            $scope.categoriesarraySelect.push(CategoryId);
        }



        $scope.categoriesobj = $scope.categoriesarraySelect.join();
    }
    $scope.AddCatArray = function (CategoryId) {
        debugger;

        AddCatID(CategoryId);
        $scope.loadData(0, $scope.itemsPerPage);

    }


    $scope.Clearfilter = function () {
        debugger
        $scope.search = '',
        FilterText = '',
        $scope.categoriesobj = '',
        minpriceval = '',
        maxpriceval = ''
        $scope.categoriesarraySelect = [];
        $scope.AllAttributeFilters = [];
        $scope.pagedItems = [];
        $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);

    }
    $scope.myFunc = function () {
        debugger;
        $scope.pagedItems = [];
        $scope.currentPage = 0;

        $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);
    };

    $rootScope.$on("CallGetCookieData", function () {
        var items = CartToCookieService.getCookieData();
        if (items != undefined) {

            $scope.AllCartItems = items;
            $scope.CartProductsCounter = $scope.AllCartItems.length;
        }
    });



    $scope.loadData = function (offset, limit) {
        var pathname = window.location.pathname; // Returns path only
        var url = window.location.href;
        var _IsFeatured = "0";
        if (pathname != "/Home/products") {
            _IsFeatured = "1";
        }

        var items = CartToCookieService.getCookieData();
        if (items != undefined) {

            $scope.AllCartItems = items;

            console.log("cart items");
            console.log($scope.AllCartItems);

            $scope.CartProductsCounter = $scope.AllCartItems.length;
        }

        var FilterText = "";
        var newcounter = 0;
        for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
            if ($scope.AllAttributeFilters[i].Values.length > 0) {
                if (newcounter == 0) {
                    FilterText = FilterText + "(";
                    newcounter++;
                }
                FilterText = FilterText + " [" + $scope.AllAttributeFilters[i].Name + "]  in (";
                for (var k = 0; k < $scope.AllAttributeFilters[i].Values.length; k++) {
                    FilterText = FilterText + "'" + $scope.AllAttributeFilters[i].Values[k] + "'" + ",";
                }
                FilterText = FilterText.replace(/,\s*$/, "");

                FilterText = FilterText + ") AND ";


            }
        }
        if (newcounter > 0) {
            FilterText = FilterText.substring(0, FilterText.length - 4);
            FilterText = FilterText + ") AND";
        }
        debugger;
        if ($.trim($scope.categoriesobj) != "" || $.trim(minpriceval) != "" || $.trim(maxpriceval) != "" || $.trim(FilterText) != "" || $.trim($scope.search) != "") {
            offset = 0;
            $scope.pagedItems = [];
        }


        $("#loadingmessage").show();
        $.ajax({
            url: '/Models/GetProducts.ashx',
            type: 'GET',
            dataType: 'json',
            data: { displayLength: limit, displayStart: offset, searchText: $scope.search, filtertext: FilterText, Categories: $scope.categoriesobj, lowprice: minpriceval, highprice: maxpriceval, isFeatured: _IsFeatured },
            success: function (data, textStatus, xhr) {

                $scope.total = data.iTotalDisplayRecords;
                $scope.pagedItems = data.aaData;
                //$scope.pagedItems = data.aaData;


                debugger;
                console.log("Data");
                console.log($scope.pagedItems);
                $("#loadingmessage").hide();
                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                return items = [];
            }
        });


        $scope.loadFilters();


    };


    $scope.IsFilterChecked = function (name, Value) {

        debugger;
        for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
            if ($scope.AllAttributeFilters[i].Name === name) {

                if (CheckVarFromArray($scope.AllAttributeFilters[i].Values, Value)) {
                    return true;
                }

            }
        }

        return false;

    }


    function CheckVarFromArray(array, value) {
        for (var i = 0; i < array.length; i++) {
            if (array[i] == value) {
                return true;
            }

        }
        return false;
    }

    $scope.AddAttrToFilter = function (ischecked, name, value) {

        debugger;
        // if (ischecked == 1)
        {
            for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
                if ($scope.AllAttributeFilters[i].Name === name) {
                    if (ischecked == "1" || ischecked == 1) {
                        if ($scope.AllAttributeFilters[i].Values.indexOf(value) === -1) {
                            $scope.AllAttributeFilters[i].Values.push(value);
                        }
                    }
                    else {
                        if (CheckVarFromArray($scope.AllAttributeFilters[i].Values, value) == true) {
                            $scope.AllAttributeFilters[i].Values.splice($scope.AllAttributeFilters[i].Values.indexOf(value), 1);
                        }
                    }
                }
            }
        }

        //else {
        //    for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
        //        if ($scope.AllAttributeFilters[i].Name === name) {
        //            if ($scope.AllAttributeFilters[i].Values.indexOf(value) === 1) {

        //                $scope.AllAttributeFilters[i].Values.splice($scope.AllAttributeFilters[i].Values.indexOf(value), 1);
        //            }

        //        }
        //    }
        //}


        $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);
    }


    $scope.TrimString = function (_String) {

        return $.trim(_String)


    }

    $scope.getValues = function (name) {

        for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
            if ($scope.AllAttributeFilters[i].Name === name) {
                if ($.trim($scope.AllAttributeFilters[i].Values) != "") {


                    return $scope.AllAttributeFilters[i].Values;
                }

            }
        }

        return [];

    }

    $scope.isAlreadyName = function (name) {

        for (var i = 0; i < $scope.AllAttributeFilters.length; i++) {
            if ($scope.AllAttributeFilters[i].Name === name) {


                return true;

            }
        }

        return false;

    }

    $scope.loadSuppliers = function () {
        $.ajax({
            url: '/api/Products/GetSuppliers',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                debugger;
                $scope.Suppliers = data;

                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.Attributes = [];
            }
        });
    };

    $scope.loadFilters = function () {
        $.ajax({
            url: '/api/Products/GetAttributes',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {

                $scope.Attributes = data;
                for (var i = 0; i < $scope.Attributes.length; i++) {
                    if ($scope.isAlreadyName($scope.Attributes[i].AttributeName) != true) {

                        $scope.AllAttributeFilters.push({
                            Name: $scope.Attributes[i].AttributeName,
                            Values: $scope.getValues($scope.Attributes[i].AttributeName)
                        });
                    }
                }


                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.Attributes = [];
            }
        });
        $.ajax({
            url: '/api/Products/GetAttributesValue',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {

                $scope.AttributesValue = data;


                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.Attributes = [];
            }
        });

        $.ajax({

            url: '/api/Products/GetCategories',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                debugger;
                $scope.categories = data;


                console.log("category");
                console.log($scope.categories);

                $scope.showloader = false;

                $scope.$apply();
                setTimeout(function () {


                    new Swiper('.swiper-coverflow', {
                        pagination: '.swiper-pagination',
                        nextButton: '.swiper-button-next',
                        prevButton: '.swiper-button-prev',
                        paginationClickable: true,
                        effect: 'coverflow',
                        centeredSlides: true,
                        slidesPerView: 'auto',
                        loop: true,
                        coverflow: {
                            rotate: 50,
                            stretch: 0,
                            depth: 100,
                            modifier: 1,
                            slideShadows: true
                        }
                    });
                }, 100);

                if ($.trim(_GlobalCatID) != "") {

                    //$scope.categoriesarraySelect.push(_GlobalCatID);
                    //$scope.categoriesobj = $scope.categoriesarraySelect.join();
                    //AddCatID(_GlobalCatID);
                    $("#cat_" + _GlobalCatID).find("input").trigger("click");
                    _GlobalCatID = "";

                }


            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.categories = [];
            }
        });


        function reinitSwiper(swiper) {

        }


        $.ajax({
            url: '/api/Products/GetSupplier',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {


                $scope.supplierlist = data;
                $scope.$apply();


                console.log($scope.supplierlist);
            },
            error: function (xhr, textStatus, errorThrown) {

            }
        });


    };
    $scope.loadMore = function () {
        debugger;
        $scope.currentPage++;
        $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);

    };

    $scope.nextPageDisabledClass = function () {
        return $scope.currentPage === $scope.pageCount() - 1 ? "disabled" : "";
    };

    $scope.pageCount = function () {
        return Math.ceil($scope.total / $scope.itemsPerPage);
    };



    $scope.AddToCart = function (productId, product) {


        debugger;


        if (product.x[5] !== 0) {


            var item = $scope.AllCartItems.filter(function (item) {
                if (item.ProductId === product.x[8]) {
                    item.Quantity = item.Quantity + 1;
                }
                return item.ProductId === product.x[8];
            })[0];
            if (item == undefined) {
                $scope.CartProductsCounter++;
                $('html, body').animate({
                    'scrollTop': $(".cart_anchor").position().top
                });
                var itemImg = $("#pid" + productId).parent().parent().find('img').eq(0);
                //AddToCart.flyToElement($(itemImg), $('.cart_anchor'));
                $scope.AllCartItems.push({ ProductId: product.x[8], Image: product.x[2], Quantity: 1, ProductName: product.x[3], ProductQuantity: product.x[5], Cost: product.x[4], discount: 0,SupplierID:product.x[11] });


            }
            CartToCookieService.setCookieData($scope.AllCartItems);
            debugger;
        }
        else {
            toastr.error("You can't Add this Item becasue it is Not Available in Stock");
        }

    };


    $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);
    $scope.loadSuppliers();

    $scope.RemoveFromwishList = function (ID) {
        debugger
        $.ajax({
            url: '/Home/DeleteWishList?id=' + ID,
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                if (data.Success == true) {

                    $scope.GetWishList($scope.CustomerID);
                    $scope.$apply();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("into error");
            }
        });
    };
    $scope.addTowishList = function (productId, customerId) {
        var cid = customerId;


        if (cid == null || cid === " " || cid == '') {
            localStorage.setItem("WishListProductID", productId);

            window.location.href = '/account/login';

        }
        else {

            if ($scope.CheckisInWishList(productId) == "active") {
                for (var i = 0; i < $scope.CurrentWishList.length; i++) {
                    if ($scope.CurrentWishList[i].ProductId == productId) {
                        $scope.RemoveFromwishList($scope.CurrentWishList[i].Id);
                        break;
                    }

                }

            }
            else {


                var wishListmodel = { ProductId: productId, CustomerId: -1, UserID: customerId };
                $.ajax({
                    url: '/api/WishLists/PostWishList',
                    type: 'POST',
                    data: wishListmodel,
                    dataType: 'json',
                    success: function (data, textStatus, xhr) {
                        $scope.GetWishList($scope.CustomerID);
                        localStorage.setItem("WishListProductID", "");
                        $scope.$apply();
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        alert("into error");
                    }
                });
            }
        }
    };

  

    $scope.GetWishList = function (customerId) {
        debugger;
        $.ajax({
            url: '/api/WishLists/GetWishLists?UserID=' + customerId,
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                $scope.CurrentWishList = data;
                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("into error");
            }
        });

    }


    
    $scope.GetWishList($scope.CustomerID);

    $scope.CheckisInWishList = function (ProductID) {
        for (var i = 0; i < $scope.CurrentWishList.length; i++) {
            if ($scope.CurrentWishList[i].ProductId == ProductID) {
                return "active";
            }

        }

        return "";
    }

    if ($.trim(_localstorageProductID) != "") {
        $scope.addTowishList(_localstorageProductID, $scope.CustomerID);
    }

    $scope.DeleteAllCarttolist = function () {
        bootbox.confirm("Are you sure you want to delete All Items ?", function (result) {
            if (result) {

                $scope.AllCartItems = [];
                CartToCookieService.setCookieData($scope.AllCartItems);
                $scope.CartProductsCounter = $scope.AllCartItems.length;
                $scope.$apply();
            }
        });
    }

    //function in remove in cart 
    $scope.DeleteCarttolist = function (Product) {
        debugger;
        bootbox.confirm("Are you sure you want to delete this Item ?", function (result) {
            if (result) {
                debugger;

                for (var i = 0; i < $scope.AllCartItems.length; i++) {
                    if ($scope.AllCartItems[i].ProductId == Product.ProductId) {
                        $scope.AllCartItems.splice($.inArray(Product, $scope.AllCartItems), 1);
                    }
                }
                CartToCookieService.setCookieData($scope.AllCartItems);
                $scope.CartProductsCounter = $scope.AllCartItems.length;
                $scope.$apply();
            }


        });
    };
    $("._slider").on("change", function () {
        debugger;

        minpriceval = $("#lowpricevalue").val();
        maxpriceval = $("#highpricevalue").val();

        if (minpriceval != "" || maxpriceval != "") {

            $scope.currentPage = 0;
            $scope.pagedItems = [];
            $scope.$apply();

            $scope.loadData($scope.currentPage * $scope.itemsPerPage, $scope.itemsPerPage);
        }

    });



});
app.directive('imageonload', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('load', function () {
                element[0].nextElementSibling.remove();
                element[0].style.display = "";
                var image = new Image();
                image.src = $(element).attr("src");
                image.onload = function () {

                    //var _height = this.height;
                    //var _Width = this.width;

                    //if (_height < _Width) {
                    //    _Width = _height;
                    //}

                    //if (_Width < _height) {
                    //    _height = _Width;
                    //}



                    //$(element).css("height", _height + "px");
                    //$(element).css("width", _Width + "px");
                };

            });
            element.bind('error', function () {
            });
        }
    };
});
$("#cart").on("click", function () {
    debugger;
    $(".shopping-cart").fadeToggle("fast");
    document.getElementById("overlay").style.display = "block";
});

$(".fa-angle-double-right").on("click", function () {
    debugger;
    $(".shopping-cart").fadeToggle("fast");
    document.getElementById("overlay").style.display = "none";

});

$("#overlay").on("click", function () {
    debugger;
    $(".shopping-cart").fadeToggle("fast");
    document.getElementById("overlay").style.display = "none";
});




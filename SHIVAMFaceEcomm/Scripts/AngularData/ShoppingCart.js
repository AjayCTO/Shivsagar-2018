app.controller("ShoppingCartCtrl", function ($scope, AddToCart, CartToCookieService) {
    $scope.emailvalidation = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    $scope.CartItems = [];
    $scope.TotalOfCartItems = 0;
    $scope.years;

    $scope.ccinfo = { type: "fa fa-credit-card" }
    $scope.CustomerInfoList = _CustomerInfo;
    $scope.CustomerAddress = { address1: "", address2: "", city: "", state: "", region: "", type: "", country: "" };

    $scope.CustomerDetails = { firstName: $scope.CustomerInfoList.FirstName, lastName: $scope.CustomerInfoList.LastName, phone: $scope.CustomerInfoList.Phone, email: $scope.CustomerInfoList.Email, cardType: 0, CreditCard: "", CardExpMo: "", CardExpYr: "", userName: "", password: "" };

    $scope.loadyear = function () {
        debugger;
        var year = new Date().getFullYear();
        var range = [];
        range.push(year);

        for (var i = 1; i < 20; i++) {
            range.push(year + i);
        }

        $scope.years = range;
    }


    $scope.loadItemsFromCookie = function () {
        debugger;
        $scope.CartItems = CartToCookieService.getCookieData();
        var total = 0;
        for (var i = 0; i < $scope.CartItems.length; i++) {
            var product = $scope.CartItems[i];
            total += (product.Cost * product.Quantity);
        }
        $scope.TotalOfCartItems = total;


    };

    $scope.updateQuantityOfCartItem = function (Product) {





        CartToCookieService.setCookieData($scope.CartItems);

        var total = 0;

        for (var i = 0; i < $scope.CartItems.length; i++) {
            var product = $scope.CartItems[i];

            if (product.Quantity != undefined && product.Quantity != "") {

                total += (product.Cost * product.Quantity);
            }
        }
        $scope.TotalOfCartItems = total;


    };

    $scope.DeleteCartItem = function (Product) {

        //if (confirm("are you sure you want to delete this item from cart ?") == true) {
        //    for (var i = 0; i < $scope.CartItems.length; i++) {
        //        if ($scope.CartItems[i].ProductId == Product.ProductId) {
        //            $scope.CartItems.splice($.inArray(Product, $scope.CartItems), 1);
        //        }
        //    }
        //    debugger;
        //    CartToCookieService.setCookieData($scope.CartItems);

        //    var total = 0;

        //    for (var i = 0; i < $scope.CartItems.length; i++) {
        //        var product = $scope.CartItems[i];
        //        total += (product.Cost * product.Quantity);
        //    }
        //    $scope.TotalOfCartItems = total;

        //}

        bootbox.confirm("are you sure you want to delete this item from cart ?", function (result) {
            if (result) {
                debugger;
                for (var i = 0; i < $scope.CartItems.length; i++) {
                    if ($scope.CartItems[i].ProductId == Product.ProductId) {
                        $scope.CartItems.splice($.inArray(Product, $scope.CartItems), 1);
                    }
                }
                debugger;
                CartToCookieService.setCookieData($scope.CartItems);

                var total = 0;

                for (var i = 0; i < $scope.CartItems.length; i++) {
                    var product = $scope.CartItems[i];
                    total += (product.Cost * product.Quantity);
                }
                $scope.TotalOfCartItems = total; $scope.$apply();
            }


        });


    };




    $scope.Continue = function () {
        debugger;
        var allDataToSend = { CartItems: $scope.CartItems, CustomerData: $scope.CustomerDetails, CustAddress: $scope.CustomerAddress };
        var saveData = $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            url: "/ShoppingCart/CompletePurchaseAndCreateSessionForUser",
            data: JSON.stringify(allDataToSend),
            dataType: "json",
            success: function (resultData) {
                console.log(resultData);
                if (resultData.Success == true) {
                    alert("Save Complete");
                    window.location.href = "/ShoppingCart/OrderConfirmation";
                }
                else {
                    alert("Error occurred" + resultData.ex)
                }

            }
            , error: function (jqXHR, exception) {
                var msg = '';

                console.log(jqXHR.responseText);
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status == 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status == 500) {
                    msg = 'Internal Server Error [500].';
                } else if (exception === 'parsererror') {
                    msg = 'Requested JSON parse failed.';
                } else if (exception === 'timeout') {
                    msg = 'Time out error.';
                } else if (exception === 'abort') {
                    msg = 'Ajax request aborted.';
                } else {
                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
                }
                alert(msg);
            },
        });
        //saveData.error(function () { alert("Something went wrong"); });

    };
    $scope.loadItemsFromCookie();




});




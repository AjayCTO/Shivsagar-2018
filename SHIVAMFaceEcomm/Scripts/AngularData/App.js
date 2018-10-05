var app = angular.module("MyApp", []);
app.filter('unique', function () {
    // we will return a function which will take in a collection
    // and a keyname
    return function (collection, keyname) {
        // we define our output and keys array;
        var output = [],
            keys = [];

        // we utilize angular's foreach function
        // this takes in our original collection and an iterator function
        angular.forEach(collection, function (item) {
            // we check to see whether our object exists
            var key = item.AttributeValue;
            // if it's not already part of our keys array
            if (keys.indexOf(key) === -1) {
                // add it to our keys array
                keys.push(key);
                // push this item to our final output array
                output.push(item);
            }
        });
        // return our array which should be devoid of
        // any duplicates
        return output;
    };
});
app.factory("AddToCart", function () {


    return {
        flyToElement: function (flyer, flyingTo) {
            var $func = $(this);
            var divider = 3;
            var flyerClone = $(flyer).clone();
            $(flyerClone).css({ position: 'absolute', top: $(flyer).offset().top + "px", left: $(flyer).offset().left + "px", opacity: 1, 'z-index': 1000 });
            $('body').append($(flyerClone));
            var gotoX = $(flyingTo).offset().left + ($(flyingTo).width() / 2) - ($(flyer).width() / divider) / 2;
            var gotoY = $(flyingTo).offset().top + ($(flyingTo).height() / 2) - ($(flyer).height() / divider) / 2;

            $(flyerClone).animate({
                opacity: 0.4,
                left: gotoX,
                top: gotoY,
                width: $(flyer).width() / divider,
                height: $(flyer).height() / divider
            }, 700,
            function () {
                $(flyingTo).fadeOut('fast', function () {
                    $(flyingTo).fadeIn('fast', function () {
                        $(flyerClone).fadeOut('fast', function () {
                            $(flyerClone).remove();
                        });
                    });
                });
            });
        }
    };
});
app.factory("CartToCookieService", function () {

    var CartProducts = [];
    return {
        setCookieData: function (CartProducts) {
            Cookies.remove('Products');
            Cookies.set('Products', JSON.stringify(CartProducts), { expires: 7 });

        },
        getCookieData: function () {
            CartProducts = Cookies.get('Products');
            return CartProducts === undefined ? CartProducts : JSON.parse(CartProducts);
        },
        clearCookieData: function () {
            CartProducts = "";
            Cookies.remove('Products');
        }
    };

});

app.controller("layoutCtrl", function ($scope, AddToCart, CartToCookieService) {
    $scope.searchcategories = [];
    $scope.wishlistCounter = 0;
    $scope.CustomerId = _CustomerIDLayout;
    debugger;
    var _localCategories = localStorage.getItem("Categories");
    if (_localCategories != null && _localCategories != undefined) {
        _localCategories = JSON.parse(_localCategories);

    }
    else {
        _localCategories = [];
    }
    $scope.GetCategories = function () {

        $.ajax({
            url: '/api/Products/GetCategories',
            type: 'GET',
            dataType: 'json',
            success: function (data, textStatus, xhr) {

                $scope.searchcategories = data;

                localStorage.setItem("Categories", JSON.stringify(data));


                $scope.$apply();



            },
            error: function (xhr, textStatus, errorThrown) {
                $scope.categories = [];
            }
        });
    };

    if (_localCategories.length == 0) {
        $scope.GetCategories();

    }
    else {
        $scope.searchcategories = _localCategories;
    }

    $scope.GetWishListCounter = function () {
        $.ajax({
            url: '/api/WishLists/GetWishLists',
            type: 'GET',
            dataType: 'json',
            data: { UserID: $scope.CustomerId },
            success: function (data, textStatus, xhr) {

                $scope.wishlistCounter = data.length;

                $scope.$apply();
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    };


    $scope.GetWishListCounter();

});
//credit card directive
app.directive
 ('creditCardType'
 , function () {

     var directive =
       {
           require: 'ngModel'
       , link: function (scope, elm, attrs, ctrl) {
           ctrl.$parsers.unshift(function (value) {
               scope.ccinfo.type =
                 (/^5[1-5]/.test(value)) ? "fa fa-cc-mastercard"
                 : (/^4/.test(value)) ? "fa fa-cc-visa"
                 : (/^3[47]/.test(value)) ? 'fa fa-cc-amex'
                 : (/^6011|65|64[4-9]|622(1(2[6-9]|[3-9]\d)|[2-8]\d{2}|9([01]\d|2[0-5]))/.test(value)) ? 'fa fa-cc-discover'
                 : undefined
               ctrl.$setValidity('invalid', !!scope.ccinfo.type)
               return value
           })
       }
       }
     return directive
 }
   )


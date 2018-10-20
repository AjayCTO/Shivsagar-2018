'use strict';
app.controller('checkOutController', ['$scope', '$location',  'localStorageService', 'authService', function ($scope, $location,localStorageService, authService) {

    $scope.emailvalidation = /^[a-z]+[a-z0-9._]+@[a-z]+\.[a-z.]{2,5}$/;
    $scope.ccinfo = { type: "fa fa-credit-card" }
    $scope.CurrentTab = 1;
    $scope.shipping = 0;
    $scope.CartTotal = 0;
    $scope.xyz = [];

    $scope.ChangeTab = function (CurrentTab) {
        $scope.CurrentTab = CurrentTab;
        CheckScopeBeforeApply();
    }

    $scope.CustomerDetails = { FirstName:'', LastName: '', Phone:'', Email: '', Street: '', City: '', PinCode: '', State: '', Country: '', UserName: '', Password: '' };

    $scope.PaymentInformation={cardType:'2', Nameoncard:'',ExpiryDate:'', CardNumber:'',Zip:'',CVV:''}

  

    $scope.PlaceOrder = function () {
        debugger;
        var allDataToSend = { OrderCartItem: $scope.shoppingCartNew, CartTotal: $scope.CartTotal, OrderCustomerData: $scope.CustomerDetails, PaymentInfo: $scope.PaymentInformation };

       
        $.ajax({
            url: serviceBase + 'api/CustomerOrders/PostOrder',
            type: 'POST',
            data: allDataToSend,
            dataType: 'json',
            success: function (data) {
                debugger;
                if (data.success == true) {
         
                    $("#exampleModal").modal("show");
                    //toastr.success("Success! Order placed successfully");

                }
            },
            error: function (xhr, textStatus, errorThrown) {
                alert("into error");
            }
        });

    };





    $scope.getcustomerinfo = function () {
        var authData = localStorageService.get('authorizationData');
        debugger;
        if (authData != null) {
            $.ajax({
                 url: serviceBase + 'api/CustomerWishlist/GetCustomerinfo',
                data: { UserName: authData.userName },
                type: 'GET',
                dataType: 'json',
                success: function (data) {
                    debugger;
              
                    if (data.success = true) {
                        $scope.CustomerDetails = {
                            FirstName: data.data[0].firstName,
                            LastName: data.data[0].lastName,
                            Phone:data.data[0].phone,
                            Email: data.data[0].email,
                            UserName: authData.userName,
                            Password:"abc12345",
                        }
    
                        $scope.$apply();
                    }
                   
                },
                error: function (xhr, textStatus, errorThrown) {
                    alert("error");
                }
            });
        }
        
    }

    $scope.getCartInfo = function () {
        $scope.shoppingCartNew = JSON.parse(localStorage.getItem("shoppingCartNew"));
        $scope.CartTotal = localStorage.getItem("GlobalTotal");
        $scope.$apply();     

    }


    function init() {
        $scope.getCartInfo();
        $scope.getcustomerinfo();
    }

    init();
}]);

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

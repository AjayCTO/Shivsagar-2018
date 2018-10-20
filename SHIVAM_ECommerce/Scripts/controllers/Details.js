angular.module('Details', ['toaster']).
    controller('DetailsController', function ($scope, $http, $location, $window, toaster) {

        $scope.ImageList = [];
        init();
        $scope.message = "anuglercheck";
        function init() {
            var _productData1 = JSON.parse(_productData);
            debugger;
            console.log("productdetailsArray");
            console.log(_productData1);
            $scope.ProductDetails = {
                ProductName: _productData1.ProductName,
                ProductCode: _productData1.ProductCode,
                ProductPrice: _productData1.ProductPrice,
                ProductQuantity: "",
                SKU: _productData1.SKU,
                IDSKU: "",
                Description:_productData1.Description,
                Images: _productData1.Images,
                allAttributes: _productData1.allAttributes,
                ManufacturerID: _productData1.ManufacturerID,
                CategoryID: _productData1.CategoryID,
                SupplierID: _productData1.SupplierID,
                UnitOfMeasureID: _productData1.UnitOfMeasureID,
                ProductID: _productData1.ProductID

            };
     
            CheckScopeBeforeApply();

            for (var i = 0; i < $scope.ProductDetails.allAttributes.length; i++) {

                if ($scope.ProductDetails.allAttributes[i].Images.length > 0) {

                    for (var k = 0; k < $scope.ProductDetails.allAttributes[i].Images.length; k++) {
                        $scope.ImageList = $scope.ProductDetails.allAttributes[i].Images;

                    }
              
                }
            }

            for (var i = 0; i < $scope.ImageList.length; i++) {
               
                var _img = '<img src="/ProductImages/' + $scope.ImageList[i].FileName + '" height="40" width="40"/>';
                $("#mainImage").attr("src","/ProductImages/"+ $scope.ImageList[i].FileName +"")
                $(".listImages").append(_img);
                CheckScopeBeforeApply();
            }
            if ($scope.ImageList.length == 0) {
                    var _img = '<img src="/ProductImages/no-image.png" height="40" width="40"/>';
                $("#mainImage").attr("src", "/ProductImages/no-image.png")
                $(".listImages").append(_img);
            }
        }
            


        function CheckScopeBeforeApply() {
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };



    });
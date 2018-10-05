angular.module('SupplierAttribute', ['toaster']).
    controller('SupplierAttributeController', function ($scope, $http, $location, $window, toaster) {

        var id = 0;

        $scope.Title = "Add Product Attribute to Product";


        function CheckScopeBeforeApply() {
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };
        $scope.CurrentUserID = _supplierID;
        $scope.CurrentSupplier = _supplierID == '-1' ? 0 : _supplierID;
        $scope.Suppliers = [];
        $scope.ProductSupplierAttributes = [];
        $scope.ProductAttributes = [];
        $scope.AllSupplierAttributes = function () {

            $scope.CurrentSupplier = $scope.CurrentSupplier != 0 ? $scope.CurrentSupplier : $("#CurrentSupplier").val();
            if ($.trim($scope.CurrentSupplier) != "") {
                $scope.ProductSupplierAttributes = [];
                $http({
                    method: "GET",
                    url: "/ProductAttributes/GetProductAttributes?SupplierID=" + $scope.CurrentSupplier
                }).then(function mySuccess(response) {
                    for (var i = 0; i < response.data.length; i++) {
                        $scope.ProductSupplierAttributes.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                    }

                    for (var i = 0; i < $scope.ProductAttributes.length; i++) {
                        $scope.ProductAttributes[i].Checked = false;
                        if ($scope.IsChecked($scope.ProductAttributes[i].Id)) {
                            $scope.ProductAttributes[i].Checked = true;
                        }
                    }

                    CheckScopeBeforeApply();
                    toaster.pop('success', "Product Attributes", "Product Attributes loaded successfully");

                }, function myError(response) {
                    toaster.pop('error', "Product Attributes", response.statusText);

                });

            }
        };

        if ($scope.CurrentSupplier != 0)
        {
            setTimeout(function () {
                $scope.AllSupplierAttributes();

            },1000);
        }
        $scope.IsChecked = function (Id) {
            for (var i = 0; i < $scope.ProductSupplierAttributes.length; i++) {
                if ($scope.ProductSupplierAttributes[i].Id == Id) {

                    return true;
                }
            }

            return false;
        }
        //Get All Supplier
        $scope.GetSupplier = function () {
            $http({
                method: "GET",
                url: "/Supplier/AllSupplier"
            }).then(function mySuccess(response) {
                for (var i = 0; i < response.data.length; i++) {
                    $scope.Suppliers.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                }

                toaster.pop('success', "Supplier", "Supplier loaded successfully");

            }, function myError(response) {
                toaster.pop('error', "Supplier", response.statusText);

            });

        };


        $scope.AddAttributes = function () {
            var items = $('input[name="chk[]"]:checked');

            var _productAttributeRelation = [];
            $(".custom-switch-input").each(function () {
                if ($(this).is(":checked")) {
                    _productAttributeRelation.push({ SupplierID: $scope.CurrentSupplier, ProductAttributesId: $(this).attr("value") });

                }
            });

            $.ajax({
                url: "/ProductAttributes/SaveSupplierAttributes",
                type: 'POST',
                data: JSON.stringify({ "Model": _productAttributeRelation }),
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {

                    if (result.Success == true) {
                        window.location.reload();
                    }
                    else {
                        toastr.error(result.ex);
                    }
                },
                error: function (err) {



                },
                complete: function () {
                }
            });

        };

        $scope.AllAttributes = function () {
            $http({
                method: "GET",
                url: "/ProductAttributes/GetAllAttributes",
            }).then(function mySuccess(response) {
                for (var i = 0; i < response.data.length; i++) {
                    $scope.ProductAttributes.push({ Id: response.data[i].Id, Name: response.data[i].Name, Descp: response.data[i].Descp, Checked: false })
                }

                console.log($scope.ProductAttributes);

                toaster.pop('success', "Product Attributes", "Product Attributes loaded successfully");

            }, function myError(response) {
                toaster.pop('error', "Product Attributes", response.statusText);

            });

        };




        $scope.GetSupplier();
        $scope.AllAttributes();
    });
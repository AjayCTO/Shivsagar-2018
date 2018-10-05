angular.module('myFormApp', ['toaster']).
    controller('ProductController', function ($scope, $http, $location, $window, toaster) {

        var id = 0;
        $scope.ProductObject = {
            ProductName: "",
            ProductCode: "",
            ProductPrice: "0",
            ProductQuantity: "",
            SKU: "",
            IDSKU: "",
            Description: "",
            Images: [],
            allAttributes: [],
            ManufacturerID: "",
            CategoryID: "",
            SupplierID: "",
            UnitOfMeasureID: ""

        };

        var _productData1 = JSON.parse(_productData);

        $scope.Title = "Add New Product";


        $scope.CurrentSupplier = { Id: 0, Name: "" };
        $scope.Suppliers = [];
        $scope.UnitsOfMeasureList = [];
        $scope.ImageList = [];
        $scope.CurrentCategory = { Id: 0, Name: "" };
        $scope.Categories = [];

        $scope.CurrentManf = { Id: 0, Name: "" };
        $scope.Manfs = [];

        $scope.CurrentProductAttribute = { Id: 0, Name: "" };
        $scope.ProductAttributes = [];
        $scope.ProductAttributesList = [];


        $scope.ResetDefaults = function () {

            if (_productData.ProductID == 0) {
                $scope.ProductObject.SupplierID = "";
                $scope.ProductObject.ManufacturerID = "";
                $scope.ProductObject.CategoryID = "";
                $scope.ProductObject.UnitOfMeasureID = "";
                CheckScopeBeforeApply();

            }
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


              //  toaster.pop('success', "Supplier", "Supplier loaded successfully");

            }, function myError(response) {
                toaster.pop('error', "Supplier", response.statusText);

            });

        };
        $scope.GetUnitsMeasure = function () {
            $http({
                method: "GET",
                url: "/UnitOfMeasures/GetUnitOfMeasure"
            }).then(function mySuccess(response) {
                for (var i = 0; i < response.data.length; i++) {
                    $scope.UnitsOfMeasureList.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                }

                $scope.ResetDefaults();

               // toaster.pop('success', "Units of meausre", "Units of meausre loaded successfully");

            }, function myError(response) {
                $scope.ResetDefaults();
                toaster.pop('error', "Units of meausre", response.statusText);

            });

        };
        $scope.GetCategories = function () {
            $http({
                method: "GET",
                url: "/Category/AllCategories"
            }).then(function mySuccess(response) {
                for (var i = 0; i < response.data.length; i++) {
                    $scope.Categories.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                }


               // toaster.pop('success', "Categories", "Categories loaded successfully");

            }, function myError(response) {
                toaster.pop('error', "Categories", response.statusText);

            });

        };

        $scope.GetManufactures = function () {
            $http({
                method: "GET",
                url: "/Manufacturers/GetManufactures"
            }).then(function mySuccess(response) {
                for (var i = 0; i < response.data.length; i++) {
                    $scope.Manfs.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                }


             //   toaster.pop('success', "Manufacturers", "Manufacturers loaded successfully");

            }, function myError(response) {
                toaster.pop('error', "Manufacturers", response.statusText);

            });

        };
        $scope.AllAttributes = function () {

            if ($.trim($scope.ProductObject.SupplierID) != "") {
                $scope.ProductAttributes = [];

                $http({
                    method: "GET",
                    url: "/ProductAttributes/GetProductAttributes?SupplierID=" + $scope.ProductObject.SupplierID
                }).then(function mySuccess(response) {
                    for (var i = 0; i < response.data.length; i++) {
                        $scope.ProductAttributes.push({ Id: response.data[i].Id, Name: response.data[i].Name })
                    }


                  //  toaster.pop('success', "Product Attributes", "Product Attributes loaded successfully");

                }, function myError(response) {
                    toaster.pop('error', "Product Attributes", response.statusText);

                });

            }
        };

        //$("#AddNew").click(function () {
        //    id++;
        //    var master = $(this).parents("table.attributes");

        //    // Get a new row based on the prototype row
        //    var prot = master.find(".prototype").clone();
        //    prot.attr("class", "")
        //    prot.find(".id").attr("value", id);

        //    master.find("tbody").append(prot);
        //});
        $scope.triggerFileClick = function () {
            $("#files").trigger("click");

        }
        $scope.addNew = function () {
            var _obj = { AttributeID: "", Value: "", Quantity: 0 };
            var _ListData = [];
            for (var i = 0; i < $scope.ProductAttributes.length; i++) {
                _ListData.push({ AttributeID: $scope.ProductAttributes[i].Id, Value: "", Quantity: "" })
            }
            $scope.ProductAttributesList.push({ ColumnsData: _ListData, Quantity: "" });
            CheckScopeBeforeApply();

        }
        $scope.RemoveIt = function ($index) {
            $scope.ProductAttributesList.splice($index, 1);

        };

        $("#files").on('change', function (event) {
            $scope.handleFileSelect(event);
        });



        $scope.RemoveFromImageList = function (ID) {
            for (var i = 0; i < $scope.ImageList.length; i++) {
                if ($scope.ImageList[i].ImageID == ID) {
                    $scope.ImageList.splice(i, 1);
                    break;
                }
            }

        }





        $scope.IsValidProduct = function () {
            if ($.trim($scope.ProductObject.ProductName) == "" || $.trim($scope.ProductObject.SKU) == ""
                || $.trim($scope.ProductObject.UnitOfMeasureID) == "" || $.trim($scope.ProductObject.CategoryID) == "" || $.trim($scope.ProductObject.ManufacturerID) == "") {

                return false;

            }

            return true;

        }


        function randomString(length, chars) {
            var result = '';
            for (var i = length; i > 0; --i) result += chars[Math.floor(Math.random() * chars.length)];
            return result;
        }
        $scope.handleFileSelect = function (evt) {


            var files = evt.target.files;
            FileName = "";
            StreamData = "";
            var _ImgObj = { ImageID: 0, FileName: "", bytestring: "", Size: 0 }
            // Loop through the FileList and render image files as thumbnails.
            for (var i = 0, f; f = files[i]; i++) {

                // Only process image files.
                if (!f.type.match('image.*')) {
                    continue;
                }

                var reader = new FileReader();

                // Closure to capture the file information.
                reader.onload = (function (theFile) {

                    var id = randomString(5, '0123456789');
                    _ImgObj.ImageID = id;
                    return function (e) {
                        // Render thumbnail.
                        FileName = theFile.name;
                        StreamData = e.target.result;
                        _ImgObj.FileName = FileName;
                        _ImgObj.bytestring = e.target.result;
                        _ImgObj.Size = theFile.size;

                    };
                })(f);

                // Read in the image file as a data URL.
                reader.readAsDataURL(f);


            }

            setTimeout(function () {

                if ($.trim(_ImgObj.FileName) !== "") {

                    $scope.ImageList.push(_ImgObj);
                    CheckScopeBeforeApply();
                }

            }, 100);

        }

        function CheckScopeBeforeApply() {
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };

        function removePaddingCharacters(bytes) {
            bytes = bytes.replace(/^data:image\/(png|jpg|jpeg|gif);base64,/, "");

            return bytes;
        }

        $scope.EditProduct = function () {

            $.ajax({
                url: "/Product/Edit",
                type: 'POST',
                data: JSON.stringify({ "Model": $scope.ProductObject }),
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {

                    if (result.Success == true) {
                        window.location.href = "/Product/ProductDetails?productID=" + result.id;
                    }
                    else {
                        toaster.pop('error', "Product save", result.ex);
                    }
                },
                error: function (err) {



                },
                complete: function () {
                }
            });
        }
        $scope.SaveProduct = function () {




            $.ajax({
                url: "/Product/Create",
                type: 'POST',
                data: JSON.stringify({ "Model": $scope.ProductObject }),
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {

                    debugger;
                    if (result.Success == true) {
                        window.location.href = "/Product/ProductDetails?productID=" + result.id;
                    }
                    else {
                        toaster.pop('error', "Product save", result.ex);
                    }
                },
                error: function (err) {



                },
                complete: function () {
                }
            });
        }

        $scope.GetSupplier();
        $scope.GetCategories();
        $scope.GetManufactures();
        $scope.GetUnitsMeasure();


        if (_productData1.ProductID != 0) {
            $scope.ProductObject = {
                ProductName: _productData1.ProductName,
                ProductCode: _productData1.ProductCode,
                ProductPrice: _productData1.ProductPrice,
                ProductQuantity: "",
                SKU: _productData1.SKU,
                IDSKU: _productData1.IDSKU,
                Description: _productData1.Description,
                Images: [],
                allAttributes: [],
                ManufacturerID: _productData1.ManufacturerID.toString(),
                CategoryID: _productData1.CategoryID.toString(),
                SupplierID: _productData1.SupplierID.toString(),
                UnitOfMeasureID: _productData1.UnitOfMeasureID.toString(),
                ProductID: _productData1.ProductID

            };

        }

    });
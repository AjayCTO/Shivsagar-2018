angular.module('myFormApp', ['toaster']).
    controller('ProductController', function ($scope, $http, $location, $window, toaster) {
        $scope.productstatuslist = JSON.parse(_productstatuslist);

        $scope.productstatuslist[0].Name;

        var id = 0;

        function init() {
            var _productData1 = JSON.parse(_productData);
            $scope.ProductObject = {
                ProductName: _productData1.ProductName,
                ProductCode: _productData1.ProductCode,
                ProductPrice: _productData1.ProductPrice,
                ProductQuantity: "",
                SKU: _productData1.SKU,
                IDSKU: "",
                Description: "",
                Images: _productData1.Images,
                allAttributes: _productData1.allAttributes,
                ManufacturerID: _productData1.ManufacturerID,
                CategoryID: _productData1.CategoryID,
                SupplierID: _productData1.SupplierID,
                UnitOfMeasureID: _productData1.UnitOfMeasureID,
                ProductID: _productData1.ProductID

            };
            CheckScopeBeforeApply();



            $scope.AllAttributes();
            for (var i = 0; i < $scope.ProductObject.allAttributes.length; i++) {
                if ($scope.ProductObject.allAttributes[i].Images.length > 0) {
                    for (var k = 0; k < $scope.ProductObject.allAttributes[i].Images.length; k++) {
                        var _base64String = base64ArrayBuffer($scope.ProductObject.allAttributes[i].Images[k].bytestring);
                        $scope.ProductObject.allAttributes[i].Images[k].bytestring = _base64String;

                    }
                }
                $scope.ProductAttributesList.push($scope.ProductObject.allAttributes[i]);
            }

            $scope.ProductObject.allAttributes = [];

            CheckScopeBeforeApply();
        }

        $scope.TempIndex = -1;

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

        function base64ArrayBuffer(arrayBuffer) {
            var base64 = ''
            var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

            var bytes = new Uint8Array(arrayBuffer)
            var byteLength = bytes.byteLength
            var byteRemainder = byteLength % 3
            var mainLength = byteLength - byteRemainder

            var a, b, c, d
            var chunk

            // Main loop deals with bytes in chunks of 3
            for (var i = 0; i < mainLength; i = i + 3) {
                // Combine the three bytes into a single integer
                chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

                // Use bitmasks to extract 6-bit segments from the triplet
                a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
                b = (chunk & 258048) >> 12 // 258048   = (2^6 - 1) << 12
                c = (chunk & 4032) >> 6 // 4032     = (2^6 - 1) << 6
                d = chunk & 63               // 63       = 2^6 - 1

                // Convert the raw binary segments to the appropriate ASCII encoding
                base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
            }

            // Deal with the remaining bytes and padding
            if (byteRemainder == 1) {
                chunk = bytes[mainLength]

                a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

                // Set the 4 least significant bits to zero
                b = (chunk & 3) << 4 // 3   = 2^2 - 1

                base64 += encodings[a] + encodings[b] + '=='
            } else if (byteRemainder == 2) {
                chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

                a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
                b = (chunk & 1008) >> 4 // 1008  = (2^6 - 1) << 4

                // Set the 2 least significant bits to zero
                c = (chunk & 15) << 2 // 15    = 2^4 - 1

                base64 += encodings[a] + encodings[b] + encodings[c] + '='
            }

            return "data:image/png;base64," + base64
        }
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


                    toaster.pop('success', "Product Attributes", "Product Attributes loaded successfully");

                }, function myError(response) {
                    toaster.pop('error', "Product Attributes", response.statusText);

                });

            }
        };



        $scope.CheckColumn = function (ID) {
            for (var i = 0; i < $scope.ProductAttributes.length; i++) {
                if ($scope.ProductAttributes[i].Id == ID) {
                    return true;

                }
            }

            return false;
        }


        setTimeout(function () {
            $(".statusselect").each(function () {
                $(this).val($(this).attr("selectedvalue"));
                $(this).trigger("change");
            });
        }, 1000)
        $scope.triggerFileClick = function () {
            $("#files").trigger("click");

        }
        $scope.addNew = function () {
            var _obj = { AttributeID: "", Value: "", Quantity: 0 };
            var _ListData = [];
            for (var i = 0; i < $scope.ProductAttributes.length; i++) {
                _ListData.push({ AttributeID: $scope.ProductAttributes[i].Id, Value: "", Quantity: "" })
            }
            $scope.ProductAttributesList.push({ ColumnsData: _ListData, IsFeatured: false, lowQuantityThreshold: null, highQuantityThreshold: null, StatusId: null, Quantity: "", price: "", weight: "", Images: [], Id: 0 });
            CheckScopeBeforeApply();
        }

        $scope.UploadImages = function (_Index) {
            $scope.TempIndex = _Index;

            CheckScopeBeforeApply();
            $("#myModal").modal("show");
        }
        $scope.RemoveIt = function ($index) {

            var _ID = $scope.ProductAttributesList[$index].Id;
            if (_ID != 0) {

                $.ajax({
                    url: "/Product/DeleteProductDetail",
                    type: 'POST',
                    data: JSON.stringify({ "ID": _ID }),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {

                        if (result.Success == true) {
                            toaster.pop('success', "Product Details", "deleted success fully");
                            $scope.ProductAttributesList.splice($index, 1);
                            CheckScopeBeforeApply();

                        }
                        else {
                            bootbox.alert(result.ex);
                            //toastr.error(result.ex);
                        }
                    },
                    error: function (err) {



                    },
                    complete: function () {
                    }
                });
            }
            else {
                $scope.ProductAttributesList.splice($index, 1);
                CheckScopeBeforeApply();
            }


        };

        $("#files").on('change', function (event) {
            for (var i = 0; i < event.currentTarget.files.length; i++) {
                $scope.handleFileSelect(event.currentTarget.files[i]);

            }
        });



        $scope.RemoveFromImageList = function (ID) {
            var _Index = -1;
            for (var i = 0; i < $scope.ProductAttributesList[$scope.TempIndex].Images.length; i++) {
                if ($scope.ProductAttributesList[$scope.TempIndex].Images[i].ImageID == ID) {
                    _Index = i;
                    break;
                }
            }

            var _ID = $scope.ProductAttributesList[$scope.TempIndex].Images[_Index].ID;
            if (_ID != 0) {

                $.ajax({
                    url: "/Product/DeleteProductImage",
                    type: 'POST',
                    data: JSON.stringify({ "ID": _ID }),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (result) {

                        if (result.Success == true) {
                            toaster.pop('success', "Product Images", "deleted success fully");
                            $scope.ProductAttributesList[$scope.TempIndex].Images.splice(_Index, 1);

                            CheckScopeBeforeApply();

                        }
                        else {
                            bootbox.alert(result.ex);
                            //toastr.error(result.ex);
                        }
                    },
                    error: function (err) {



                    },
                    complete: function () {
                    }
                });
            }
            else {
                $scope.ProductAttributesList[$scope.TempIndex].Images.splice(_Index, 1);
            }
          

        }

        $(document).on("keydown", ".number", function (event) {


            if (event.shiftKey == true) {
                event.preventDefault();
            }

            if ((event.keyCode >= 48 && event.keyCode <= 57) ||
                (event.keyCode >= 96 && event.keyCode <= 105) ||
                event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 ||
                event.keyCode == 39 || event.keyCode == 46 || event.keyCode == 190) {

            } else {
                event.preventDefault();
            }

            if ($(this).val().indexOf('.') !== -1 && event.keyCode == 190)
                event.preventDefault();
            //if a decimal has been added, disable the "."-button

        });

        $scope.CheckValidity = function () {
            for (var i = 0; i < $scope.ProductAttributesList.length; i++) {
                if ($.trim($scope.ProductAttributesList[i].highQuantityThreshold) == "" || $.trim($scope.ProductAttributesList[i].lowQuantityThreshold) == "" || $.trim($scope.ProductAttributesList[i].StatusId) == "" || $.trim($scope.ProductAttributesList[i].Quantity) == "" || $.trim($scope.ProductAttributesList[i].price) == "") {
                    return false;
                }
                for (var k = 0; k < $scope.ProductAttributesList[i].ColumnsData.length; k++) {
                    if ($.trim($scope.ProductAttributesList[i].ColumnsData[k].Value) == "") {
                        return false;
                    }
                }
            }
            return true;
        }

        $scope.$watchCollection('ProductAttributesList', function (newVal, oldVal) {
            $scope.IsValidProduct();
        }, true);


        $scope.IsValidProduct = function () {

            if ($scope.ProductAttributesList.length == 0 || $scope.CheckValidity() == false) {
                console.log("false");
                return false;

            }

            return true;

        }


        function randomString(length, chars) {
            var result = '';
            for (var i = length; i > 0; --i) result += chars[Math.floor(Math.random() * chars.length)];
            return result;
        }
        $scope.handleFileSelect = function (f) {

            FileName = "";
            StreamData = "";
            var _ImgObj = { ImageID: 0, FileName: "", bytestring: "", Size: 0, ID: 0 }




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



            setTimeout(function () {

                if ($.trim(_ImgObj.FileName) !== "") {

                    $scope.ProductAttributesList[$scope.TempIndex].Images.push(_ImgObj);
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
        $scope.SaveProductDetails = function () {


            var _toAddAttributes = angular.copy($scope.ProductAttributesList);



            for (var i = 0; i < _toAddAttributes.length; i++) {

                var _columnsData = [];
                for (var j = 0; j < _toAddAttributes[i].ColumnsData.length; j++) {
                    if ($.trim(_toAddAttributes[i].ColumnsData[j].Value) != "") {

                        _columnsData.push({ AttributeID: _toAddAttributes[i].ColumnsData[j].AttributeID, Value: _toAddAttributes[i].ColumnsData[j].Value });
                    }

                }

                var _toSendImages = angular.copy(_toAddAttributes[i].Images);
                var _Myimages = [];
                for (var k = 0; k < _toSendImages.length; k++) {

                    if (_toSendImages[k].bytestring != null && _toSendImages[k].bytestring != undefined) {
                        _toSendImages[k].bytestring = removePaddingCharacters(_toSendImages[k].bytestring);
                        _Myimages.push({ ID: _toSendImages[k].ID, ImageID: _toSendImages[k].ImageID, FileName: _toSendImages[k].FileName, Size: _toSendImages[k].Size, bytestring: _toSendImages[k].bytestring });
                    }

                }
                $scope.ProductObject.allAttributes.push({ ColumnsData: _columnsData, IsFeatured: _toAddAttributes[i].IsFeatured, lowQuantityThreshold: _toAddAttributes[i].lowQuantityThreshold, highQuantityThreshold: _toAddAttributes[i].highQuantityThreshold, StatusId: _toAddAttributes[i].StatusId, Quantity: _toAddAttributes[i].Quantity, price: _toAddAttributes[i].price, weight: _toAddAttributes[i].weight, Images: _Myimages, Id: _toAddAttributes[i].Id });
            }



            console.log($scope.ProductObject);

            CheckScopeBeforeApply();

            $.ajax({
                url: "/Product/ProductDetails",
                type: 'POST',
                data: JSON.stringify({ "Model": $scope.ProductObject }),
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {

                    if (result.Success == true) {
                        window.location.href = "/Product/GetAllProducts";
                    }
                    else {
                        bootbox.alert(result.ex);
                        //toastr.error(result.ex);
                    }
                },
                error: function (err) {



                },
                complete: function () {
                }
            });
        }

        init();
    });
angular.module('AssignProductImages', ['toaster']).
    controller('AssignProductImagesController', function ($scope, $http, $location, $window, toaster) {
        $scope.Images = [];
        $scope.Paths = [];
        $scope.SearchString = "";

        /*Pagination Functions */
        $scope.itemsPerPage = 12;
        $scope.currentPage = 1;
        $scope.items = [];
        $scope.TotalItems = 0;
        $scope.AlreadySelectedImages = JSON.parse(_AlreadySelectedData);
        $scope.IsSaving = false;
        //for (var i = 0; i < 50; i++) {
        //    $scope.items.push({
        //        id: i, name: "name " + i, description: "description " + i
        //    });
        //}

        $scope.range = function () {
            var rangeSize = $scope.itemsPerPage;
            var ret = [];
            var start;

            start = $scope.currentPage;
            if (start > $scope.pageCount() - rangeSize - 1) {
                start = $scope.pageCount() - rangeSize + 1;
            }

            for (var i = start; i < start + (rangeSize - 1) ; i++) {
                if (i > -1) {
                    ret.push(i);

                }
            }
            return ret;
        };

        $scope.prevPage = function () {
            if ($scope.currentPage > 0) {
                $scope.currentPage--;
                CheckScopeBeforeApply();
                GetImages();
            }
        };

        $scope.prevPageDisabled = function () {
            return $scope.currentPage === 1 ? "disabled" : "";
        };

        $scope.pageCount = function () {
            return Math.ceil($scope.TotalItems / $scope.itemsPerPage);
        };


        $scope.DisableCursor = function (path) {
            for (var i = 0; i < $scope.AlreadySelectedImages.length; i++) {
                if ($.trim($scope.AlreadySelectedImages[i]) == $.trim(path)) {
                    return "NonePointer";
                }
            }

            return "";
        }

        $scope.nextPage = function () {
            if ($scope.currentPage < $scope.pageCount()) {
                $scope.currentPage++;
                CheckScopeBeforeApply();
                GetImages();

            }
        };

        $scope.nextPageDisabled = function () {
            return $scope.currentPage === $scope.pageCount() ? "disabled" : "";
        };

        $scope.setPage = function (n) {
            $scope.currentPage = n + 1;
            CheckScopeBeforeApply();
            GetImages();
        };

        /*Pagination Functions end */

        function init() {
            GetImages();
        }
        function CheckScopeBeforeApply() {
            if (!$scope.$$phase) {
                $scope.$apply();
            }
        };
        function GetImages() {
            var _model = { ProductID: _ProductID, pageSize: $scope.itemsPerPage, page: $scope.currentPage, SearchString: $scope.SearchString };
            $.ajax({
                url: "/AllImages/AllImageDataSelected",
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ "Model": _model }),
                contentType: "application/json; charset=utf-8",
                error: function (err) {
                    console.log(err);
                    bootbox.alert('There is a problem with the service!');
                },
                success: function (data) {
                    try {
                        debugger;
                        if (data.Success==true) {
                            $scope.Images = data.data;

                            console.log($scope.Images);
                            $scope.TotalItems = data.TotalCount;
                            CheckScopeBeforeApply();
                        }
                        else {
                            bootbox.alert("An issue occurred while performning action " + data.ex);
                        }

                    } catch (_ex) {
                        bootbox.alert("An issue occurred while performning action " + _ex);
                    }
                }
            });

        }


        $scope.SaveImages = function () {
            var _model = { ProductID: _ProductID, Path: $scope.Paths };
            $scope.IsSaving = true;
            $.ajax({
                url: "/AllImages/SaveImages",
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ "Data": _model }),
                contentType: "application/json; charset=utf-8",
                error: function () {
                    $scope.Paths = [];
                    $scope.IsSaving = false;
                    bootbox.alert('There is a problem with the service!');
                },
                success: function (data) {
                    try {
                        $scope.Paths = [];
                        $scope.IsSaving = false;
                        if (data.Success === true) {
                            bootbox.alert("Saved Successfully");
                            GetImages();
                            window.location.reload();
                            CheckScopeBeforeApply();
                        }

                    } catch (_ex) {
                        $scope.Paths = [];
                        $scope.IsSaving = false;
                        bootbox.alert(_ex);
                    }
                }
            });
        }


        $scope.IsSelected = function (path) {
            for (var i = 0; i < $scope.Paths.length; i++) {
                if ($.trim($scope.Paths[i]) == $.trim(path)) {
                    return "Selected";
                }
            }

            return "";
        }

        $scope.AddToArray = function (Path) {
            var _TempImageData = angular.copy($scope.Paths);
            var _CheckVar = false;
            for (var i = 0; i < $scope.Paths.length; i++) {
                if ($scope.Paths[i] == Path) {
                    _TempImageData.splice(i, 1);
                    _CheckVar = true;
                    break;
                }
            }
            if (!_CheckVar) { $scope.Paths.push(Path) } else { $scope.Paths = _TempImageData };
            CheckScopeBeforeApply();

        }


        init();
    });

angular.module('AssignProductImages').filter('offset', function () {
    return function (input, start) {
        start = parseInt(start, 10);
        return input.slice(start);
    };
});
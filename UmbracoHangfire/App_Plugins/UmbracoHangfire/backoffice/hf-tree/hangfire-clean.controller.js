(function () {
    "use strict";

    function HFCleanController($scope, $routeParams, $http, notificationsService) {
        var vm = this;
        vm.loaded = true;
        vm.headerName = "Clean Orphaned Jobs";
        vm.clean = function () {
            $http({
                method: "POST",
                url: "/umbraco/backoffice/api/HangfireApi/CleanOrphaned"
            }).then(function (response) {
                if (typeof response.data.Success !== "undefined" && typeof response.data.Message !== "undefined") {
                    if (response.data.Success === true)
                        notificationsService.success("Success", response.data.Message);
                    else
                        notificationsService.warning(response.data.Message);
                }
            }, function (response) {
                if (typeof response.data.ExceptionMessage !== "undefined")
                    notificationsService.error(response.data.ExceptionMessage);
            });
        };
    }
    angular.module("umbraco").controller("hangfireclean.controller", HFCleanController);
})();
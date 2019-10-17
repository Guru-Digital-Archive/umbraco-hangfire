(function () {
    "use strict";

    function HFJobController($scope, $routeParams, $http, notificationsService) {
        var vm = this;
        vm.loaded = false;
        vm.headerName = "Hangfire Job";
        vm.CronModList = ["Minutes", "Hours", "Days", "Months"];
        vm.job = null;
        $http.get("/umbraco/backoffice/api/HangfireApi/GetSettings?id=" + $routeParams.id).then(function (response) {
            if (response.data != null) {
                if (typeof response.data.ExceptionMessage !== "undefined") {
                    notificationsService.error(response.data.ExceptionMessage);
                    return;
                }
                vm.job = response.data;
                //Ref: https://github.com/shawnchin/jquery-cron/
                $('#cron-selector').cron({
                    initial: vm.job.Cron,
                    customValues: {
                        "Every 5 Minutes": "*/5 * * * *",
                        "Every 10 Minutes": "*/10 * * * *",
                        "Every 15 Minutes": "*/15 * * * *",
                        "Every 30 Minutes": "*/30 * * * *"
                    },
                    onChange: function () {
                        vm.job.Cron = $(this).cron("value");
                    }
                });
            }
            vm.loaded = true;

        }, function (response) {
            if (typeof response.data.ExceptionMessage !== "undefined")
                notificationsService.error(response.data.ExceptionMessage);
        });

        vm.save = function () {
            vm.submitDisabled = true;
            $http({
                method: "POST",
                url: "/umbraco/backoffice/api/HangfireApi/Save",
                data: vm.job
            }).then(function (response) {
                if (notificationsService.current.length > 0) {
                    notificationsService.remove(0); // Make sure only 1 notification displays at a time
                }
                if (typeof response.data.ExceptionMessage !== "undefined") {
                    notificationsService.error(response.data.ExceptionMessage);
                    return;
                }
                vm.errors = [];
                if (typeof response.data.Success !== "undefined" && response.data.Success === true && typeof response.data.Message !== "undefined") {
                    notificationsService.success("Success", response.data.Message);
                    vm.job = response.data;

                } else if (typeof response.data.errors) {
                    vm.errors = response.data.errors;
                }
                vm.submitDisabled = false;

            }, function (response) {
                if (typeof response.data.ExceptionMessage !== "undefined")
                    notificationsService.error(response.data.ExceptionMessage);
            });
        }

        vm.execute = function () {
            $http({
                method: "POST",
                url: "/umbraco/backoffice/api/HangfireApi/ExecuteNow",
                data: vm.job
            }).then(function (response) {
                if (typeof response.data.ExceptionMessage !== "undefined") {
                    notificationsService.error(response.data.ExceptionMessage);
                    return;
                }
                if (typeof response.data.Success !== "undefined" && response.data.Success === true && typeof response.data.Message !== "undefined") {
                    notificationsService.success(response.data.Message);
                }
                vm.job.LastExecuted = response.data.LastExecuted;
                vm.job.NextExecution = response.data.NextExecution;
            }, function (response) {
                if (typeof response.data.ExceptionMessage !== "undefined")
                    notificationsService.error(response.data.ExceptionMessage);
            });
        }
    }
    angular.module("umbraco").controller("hangfirejob.controller", HFJobController);
})();


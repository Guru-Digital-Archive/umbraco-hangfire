(function () {
    "use strict";

    function HFJobListController($scope, $routeParams, $http, notificationsService) {

        var vm = this;

        vm.loaded = false;
        vm.headerName = "Jobs";
        vm.nojobs = true;
        vm.jobs = false;
        vm.jobdata = null;
        $http.get("/umbraco/backoffice/api/HangfireApi/GetAllJobs")
            .then(function (response) {
                if (response.data != null) {
                    vm.nojobs = false;
                    vm.jobs = true;
                    vm.jobdata = response.data;
                }
                vm.loaded = true;
        });
    }
    angular.module("umbraco").controller("hangfirejoblist.controller", HFJobListController);
})();
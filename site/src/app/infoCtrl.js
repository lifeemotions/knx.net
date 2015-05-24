(function () {
    var InfoCtrl = function ($rootScope, $scope, $http, appSettings) {
      $scope.data = {
        title: null,
        projectUrl: null,
        githubUrl: null,
        nugetUrl: null,
        ownerUrl: null,
        ownerLogoEnabled: null,
        ownerName: null
      };

      function processInfo(data) {
        if (!data || !data.info)
          return;

        var info = data.info;

        $scope.data.title = info.title;
        $scope.data.projectUrl = info.projectUrl;
        $scope.data.githubUrl = info.githubUrl;
        $scope.data.nugetUrl = info.nugetUrl;
        $scope.data.ownerUrl = info.ownerUrl;
        $scope.data.ownerLogoEnabled = info.ownerLogoEnabled;
        $scope.data.ownerName = info.ownerName;
      }

      function init() {
        $http.get(appSettings.infoFile)
          .then(function(res){
            if (!res || !res.data)
              return;

              processInfo(res.data);
          });
      };

      init();
    };

    InfoCtrl.$inject = ['$rootScope', '$scope', '$http', 'appSettings'];

    angular.module('app').controller('InfoCtrl', InfoCtrl);
}());

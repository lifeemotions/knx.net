(function () {
    var ReleaseCtrl = function ($rootScope, $scope, $http, appSettings) {
      $scope.data = {
        description: null,
        projectUrl: null,
        releaseNotes: null,
        id: null,
        title: null,
        version: null
      };

      function processRelease(data) {
        if (!data || !data.package || !data.package.metadata)
          return;

        var info = data.package.metadata;

        $scope.data.description = info.description;
        $scope.data.projectUrl = info.projectUrl;
        $scope.data.releaseNotes = info.releaseNotes;
        $scope.data.id = info.id;
        $scope.data.title = info.title;
        $scope.data.version = info.version;
      }

      function init() {
        $http.get(appSettings.releaseFile)
          .then(function(res){
            if (!res || !res.data)
              return;

              processRelease(res.data);
          });
      };

      init();
    };

    ReleaseCtrl.$inject = ['$rootScope', '$scope', '$http', 'appSettings'];

    angular.module('app').controller('ReleaseCtrl', ReleaseCtrl);
}());

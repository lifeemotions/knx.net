(function () {
    var MainCtrl = function ($rootScope, $scope) {
      $scope.data = {
      };
    };

    MainCtrl.$inject = ['$rootScope', '$scope'];

    angular.module('app').controller('MainCtrl', MainCtrl);
}());

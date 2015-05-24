(function () {

  var app = angular.module('app', ['ngMaterial']);

  app.config(function($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('light-green')
      .accentPalette('grey')
      .warnPalette('red')
      .backgroundPalette('grey');
  });

}());

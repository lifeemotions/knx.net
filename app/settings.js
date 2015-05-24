(function () {
    var app = angular.module('app');

    app.value('appSettings', {
      releaseFile: 'ref/release.json',
      documentationFile: 'ref/documentation.json',
      infoFile: 'ref/info.json'
    });
}());

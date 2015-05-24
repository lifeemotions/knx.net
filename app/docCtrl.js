(function () {
    var DocumentationCtrl = function ($rootScope, $scope, $mdDialog, $http, appSettings) {

      $scope.data = {
        classes: [],
        selectedClass: null,
        dialog: $mdDialog
      };

      function processDocumentation(data) {
        if (!data || !data.doc)
          return;
        if (!data.doc.members || !data.doc.members.member)
          return;

        var previousMemberName = '';
        var members = data.doc.members.member;
        for (var i = 0, len = members.length; i < len; i++) {
          var currMember = members[i];
          var currMemberName = currMember.$.name.trim();

          var indexOfParentheses = currMemberName.indexOf('(');
          if (indexOfParentheses >= 0)
            currMemberName = currMemberName.substring(0, indexOfParentheses);
          var lastIndexOfDot = currMemberName.lastIndexOf('.');
          currMemberClassName = currMemberName.substring(2, lastIndexOfDot);

          if (previousMemberName === currMemberClassName) {
            // class member
            var currClass = getClass(currMemberClassName);
            if (currClass == null)
              continue;

            insertMemberIntoClass(currClass, currMember);
          }
          else {
            // class
            currMemberClassName = currMemberName.substring(2);
            previousMemberName = currMemberClassName;
            $scope.data.classes.push({
              name: currMemberClassName,
              summary: currMember.summary.trim(),
              constructors: [],
              fields: [],
              delegates: [],
              properties: [],
              functions: []
            });
          }

        }
      }

      function insertMemberIntoClass(currClass, currMember) {
        var currMemberName = currMember.$.name.trim().substring(2);
        var currMemberType = currMember.$.name.trim()[0];

        if (currMemberType === 'M') {
          if (currMemberName.indexOf('#ctor') >= 0) {
            var indexOfCtor = currMemberName.indexOf('.#ctor');
            var currMemberNameEnd = currMemberName.substring(indexOfCtor + 6);
            currMemberName = currMemberName.substring(0, indexOfCtor);
            currMemberName += currMemberNameEnd;
            currMemberType = 'C';
          }
        }

        if (currMemberType != 'C')
          currMemberName = currMemberName.replace(currClass.name + '.', '');
        else {
          var lastIndexOfDot = currClass.name.lastIndexOf('.');
          currMemberName = currMemberName.substring(lastIndexOfDot + 1);
        }

        currMemberName = currMemberName.replace(",", ", ");

        var entity =
          {
            name: currMemberName,
            summary: currMember.summary.trim(),
            parameters: processParams(currMember.param),
            //exception: processException(currMember.exceptions),
            //returns: processReturns(currMember.returns)
          };

        switch (currMemberType) {
          case 'C':
            currClass.constructors.push(entity);
            break;
          case 'F':
            currClass.fields.push(entity);
            break;
          case 'T':
            currClass.delegates.push(entity);
            break;
          case 'P':
            currClass.properties.push(entity);
            break;
          case 'M':
            currClass.functions.push(entity);
            break;
        }
      }

      function processParams(params) {
        var output = [];

        if (!params)
          return output;

        for (var i = 0, len = params.length; i < len; i++) {
          var param = params[i];

          output.push({
            name: param.$.name,
            description: param._
          });
        }

        return output;
      }

      function getClass(name) {
        for (var i = 0, len = $scope.data.classes.length; i < len; i++) {
          var currClass = $scope.data.classes[i];
          if (currClass.name === name)
            return currClass;
        }
        return null;
      }

      function init() {
        $http.get(appSettings.documentationFile)
          .then(function(res){
            if (!res || !res.data)
              return;

            processDocumentation(res.data);
          });
      };

      init();

      $scope.openDetails = function(member, evt) {
        $mdDialog.show({
          targetEvent: evt,
          locals: {
            class: $scope.data.selectedClass,
            member: member,
            dialog: $mdDialog,
          },
          controller: angular.noop,
          controllerAs: 'ctrl',
          bindToController: true,
          templateUrl: 'memberdialog.html',
        });
      };
    };

    DocumentationCtrl.$inject = ['$rootScope', '$scope', '$mdDialog', '$http', 'appSettings'];

    angular.module('app').controller('DocumentationCtrl', DocumentationCtrl);
}());

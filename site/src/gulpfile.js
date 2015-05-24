var gulp = require('gulp');
var shell = require('gulp-shell');
var unzip = require('gulp-unzip');
var connect = require('gulp-connect');
var runSequence = require('run-sequence');
var rename = require('gulp-rename');
var clean = require('gulp-clean');
var download = require("gulp-download");
var marked = require('gulp-marked');
var replace = require('gulp-replace');
var xml2json = require('gulp-xml2json');
var file = require('gulp-file');

var info = require('./info.json').info;

gulp.task('download-package', shell.task([
    'nuget install ' + info.nugetProjectTitle + ' -OutputDirectory ../build'
  ])
);

gulp.task('extract-package', ['download-package'], function() {
  return gulp.src('../build/**/*.nupkg')
    .pipe(unzip())
    .pipe(gulp.dest('../build/extract'))
});

gulp.task('copy-files-nupkg', ['copy-files-nupkg-xml',
                               'copy-files-nupkg-nuspec']);

gulp.task('copy-files-nupkg-xml', function() {
  return gulp.src('../build/extract/lib/**/*.xml')
    .pipe(xml2json({
      explicitArray: false,
    }))
    .pipe(rename({
      dirname: '',
      basename: 'documentation',
      extname: '.json'
    }))
    .pipe(gulp.dest('ref/'));
});

gulp.task('copy-files-nupkg-nuspec', function() {
  return gulp.src('../build/extract/*.nuspec')
    .pipe(rename({
      extname: '.xml'
    }))
    .pipe(xml2json({
      explicitArray: false,
    }))
    .pipe(rename({
      dirname: '',
      basename: 'release',
      extname: '.json'
    }))
    .pipe(gulp.dest('ref/'));
});

gulp.task('download-readme-github', function() {
  var url = 'https://raw.githubusercontent.com/' + info.githubAccount;
  url += '/' + info.githubProject + '/master/' + info.githubReadmeFile;
  return download(url)
    .pipe(gulp.dest("../build/"));
});

gulp.task('convert-readme-github', ['download-readme-github'], function() {
  return gulp.src('../build/' + info.githubReadmeFile)
    .pipe(rename({
      basename: 'readme'
    }))
    .pipe(marked({
      highlight: function (code) {
        return require('highlight.js')
          .highlight('cs', code)
          .value;
      },
      langPrefix:'hljs '
    }))
    .pipe(gulp.dest('../output/'));
});

gulp.task('download-license-github', function() {
  var url = 'https://raw.githubusercontent.com/' + info.githubAccount;
  url += '/' + info.githubProject + '/master/' + info.githubLicenseFile;
  return download(url)
    .pipe(rename({
      dirname: '',
      basename: 'license',
        extname: '.txt'
    }))
    .pipe(gulp.dest("ref/"));
});

gulp.task('convert-license-github', ['download-license-github'], function() {
  return gulp.src('ref/license.txt')
    .pipe(rename({
      extname: '.html'
    }))
    .pipe(replace(/\r\n/g, '<br />'))
    .pipe(replace(/\n/g, '<br />'))
    .pipe(gulp.dest("ref/"));
});

gulp.task('copy-hightlightjs-style', function() {
  return gulp.src('./node_modules/highlight.js/styles/solarized_dark.css')
    .pipe(rename({
      dirname: '',
      basename: 'highlight'
    }))
    .pipe(gulp.dest('ref/'));
});

gulp.task('copy-resources-github', [
    'copy-hightlightjs-style',
    'convert-readme-github',
    'convert-license-github'
   ], function() {
  return gulp.src('../output/readme.html')
    .pipe(rename({dirname: ''}))
    .pipe(gulp.dest('ref/'));
});

gulp.task('download-owner-logo',  function() {
  if (!info.ownerLogoGravatarId || info.ownerLogoGravatarId.length <= 0)
    return;

  var url = 'http://www.gravatar.com/avatar/' + info.ownerLogoGravatarId + '?s=320';

  return download(url)
    .pipe(rename({
      dirname: '',
      basename: 'logo',
      extname: '.png'
    }))
    .pipe(gulp.dest("ref/"));
});

gulp.task('update-ref',  function() {
  runSequence('download-owner-logo',
              'extract-package',
              'copy-files-nupkg',
              'copy-resources-github');
});

gulp.task('process-info-file', function() {
  var projectInfo = { info: {} };

  //todo: returns
  //todo: exceptions

  var githubUrl = 'http://github.com/' + info.githubAccount + '/' + info.githubProject;
  var nugetUrl = 'https://www.nuget.org/packages/' + info.nugetProjectTitle;
  var ownerLogoEnabled = info.ownerLogoGravatarId && info.ownerLogoGravatarId.length > 0;

  projectInfo.info.title = info.title;
  projectInfo.info.projectUrl = info.projectUrl;
  projectInfo.info.githubUrl = githubUrl;
  projectInfo.info.nugetUrl = nugetUrl;
  projectInfo.info.ownerUrl = info.ownerUrl;
  projectInfo.info.ownerLogoEnabled = ownerLogoEnabled;
  projectInfo.info.ownerName = info.ownerName;

  return file('info.json', JSON.stringify(projectInfo), { src: true })
    .pipe(gulp.dest('ref'));
});

gulp.task('build', function() {
  runSequence('clean',
              'copy-files');
});

gulp.task('clean', function() {
  return gulp.src(['../build/', '../output/'], {read: false})
    .pipe(clean({force: true}));
});

gulp.task('copy-files', ['copy-files-root',
                         'copy-files-app',
                         'copy-files-ref',
                         'copy-files-assets']);

gulp.task('copy-files-root', function() {
  return gulp.src('*.html')
    .pipe(gulp.dest('../output/'));
});

gulp.task('copy-files-app', function() {
  return gulp.src('app/**/*')
    .pipe(gulp.dest('../output/app'));
});

gulp.task('copy-files-ref', ['process-info-file'], function() {
  return gulp.src('ref/**/*')
    .pipe(gulp.dest('../output/ref'));
});

gulp.task('copy-files-assets', function() {
  return gulp.src('assets/**/*')
    .pipe(gulp.dest('../output/assets'));
});

gulp.task('connect', function() {
  connect.server({
    root: '../output/',
    port: 8000,
    livereload: true
  });
});

gulp.task('watch', function () {
  gulp.watch(['*.html', 'assets/**/*', 'app/**/*', 'ref/**/*'], ['copy-files']);
});

gulp.task('default', ['build', 'connect', 'watch']);

'use strict';

var ts = require('gulp-typescript');
var gulp = require('gulp');
var clean = require('gulp-clean');

var destPath = './wwwroot/lib/npmlibs';

// Delete the dist directory
gulp.task('clean', function () {
    return gulp.src(destPath).pipe(clean());
});

gulp.task("scriptsNStyle", function () {
    gulp.src(['core-js/client/**', 'systemjs/dist/system.src.js', 'reflect-metadata/**', 'rxjs/**', 'zone.js/dist/**', '@angular/**', 'jquery/dist/jquery.*js', 'bootstrap/dist/js/bootstrap.*js', '@angular2-material/**/*'], {
        cwd: "node_modules/**"
    }).pipe(gulp.dest("./wwwroot/lib/npmlibs"));
});

var tsProject = ts.createProject('Scripts/tsconfig.json');
gulp.task('ts', function (done) {
    //var tsResult = tsProject.src()
    var tsResult = gulp.src(["Scripts/*.ts"]).pipe(ts(tsProject), undefined, ts.reporter.fullReporter());
    return tsResult.js.pipe(gulp.dest('./wwwroot/app'));
});

gulp.task('watch', ['watch.ts']);

gulp.task('watch.ts', ['ts'], function () {
    return gulp.watch('Scripts/*.ts', ['ts']);
});

gulp.task('default', ['scriptsNStyle', 'watch']);

/* react */

var browserify = require('browserify');
var babelify = require('babelify');
var source = require('vinyl-source-stream');

gulp.task('buildReact', function () {
    return browserify({ entries: './Scripts/test.jsx', extensions: ['.jsx'], debug: true }).transform('babelify', { presets: ['es2015', 'react'] }).bundle().pipe(source('bundle.js')).pipe(gulp.dest('./wwwroot/app/react/dist'));
});

//gulp.task('watch.react', ['buildReact'], function () {
//    gulp.watch('Scripts/*.jsx', ['buildReact']);
//});

//gulp.task('default', ['watch']);


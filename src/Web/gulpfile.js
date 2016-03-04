/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    react = require('gulp-react'),
    source = require("vinyl-source-stream"),
    streams = require("memory-streams"),
    combinedStream = require("combined-stream"),
    os = require("os"),
    browserify = require('browserify');

var paths = {
    webroot: "./wwwroot/",
    src: {
        jsx: "./wwwroot/js/*.jsx",
        scripts: "./wwwroot/js/*.js"
    },
    dest: {
        bundles: "./wwwroot/js/dist",
        bundlesFilter: "!./wwwroot/js/dist/**/*.js",
        serverBundle: "serverBundle.js",
        clientBundle: "clientBundle.js",
        jsx: "./wwwroot/js"
    }
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";


gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("react", function () {
    return gulp.src(paths.src.jsx)
      .pipe(react())
      .pipe(gulp.dest(paths.dest.jsx));
});

var createServerBundle = function (browserify, configPath) {
    var utils = {
        parseConfig : function(config) {
            if (config) {
                if (config.expose) {
                    var components = {};
                    //1. parse the configuration
                    config.expose.forEach(function (component) {
                        var path, name;

                        if (typeof component === 'string') {
                            path = component;
                        }
                        else {
                            path = component.path;
                            if (component.name) {
                                name = component.name;
                            }
                        }
                        if (name === undefined) {
                            var splitted = path.split('/');
                            name = splitted[splitted.length - 1];
                        }
                        components[name] = path;
                    });
                    return components;
                }
            }
        },
        exposeReact: function(exposedVariables, requires) {
            requires.push({ file: "react" });
            exposedVariables.append('var React = require("react");' + os.EOL);
        }
    };

    if (configPath === undefined) {
        configPath = './reactServerConfig.json';
    }
    var config = require(configPath);

    var serverComponents = utils.parseConfig(config);
    if (serverComponents) {
        var exposedVariables = combinedStream.create();
        var requires = [];
        exposedVariables.append(';' + os.EOL);
        utils.exposeReact(exposedVariables, requires);

        for (var name in serverComponents) {
            var path = serverComponents[name];
            requires.push({ file: path, expose: name });
            exposedVariables.append('var ' + name + ' = require("' + name + '");');
        }
        browserify.require(requires);
        var bundleStream = combinedStream.create();
        bundleStream.append(browserify.bundle());
        bundleStream.append(exposedVariables);

        return bundleStream;
    }
};

var gulpServerBundle = function () {
    var bundle = createServerBundle(browserify(
      {
          extensions: ['.jsx', '.js']
      }
    ));
    return bundle
      .pipe(source(paths.dest.serverBundle))
      .pipe(gulp.dest(paths.dest.bundles));
};

gulp.task("server-build", function () {
    return gulpServerBundle();
});

var gulpClientBundle = function () {
    var b = browserify(paths.src.app,
      {
          extensions: ['.jsx', '.js']
      });
    var bundle = createServerBundle(b);
    return bundle
      .pipe(source(paths.dest.clientBundle))
      .pipe(gulp.dest(paths.dest.bundles));
};

gulp.task("client-build", function () {
    return gulpClientBundle();
});

gulp.task('watch', function () {
    /// watch *.jsx files for changes and compile to *.js
    gulp.watch(paths.src.jsx, [ "react" ]);
    /// watch for *.js file changes and bundle
    gulp.watch([ paths.src.scripts, paths.dest.bundlesFilter ], [ "client-build", "server-build" ]);
});

gulp.task("default", [ "watch" ]);
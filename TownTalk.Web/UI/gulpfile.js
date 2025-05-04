const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const sourcemaps = require('gulp-sourcemaps');
const webpack = require('webpack-stream');
const webpackConfig = require('./webpack.config.js');
const postcss = require('gulp-postcss');
const tailwindcss = require('tailwindcss');
const autoprefixer = require('autoprefixer');
const concat = require("gulp-concat");

// const fs = require("fs");
// const copy = require("recursive-copy");

// const copyTinyMce = () => copy("./node_modules/tinymce/", "../wwwroot/js/tinymce")
//     .then(result => console.log(`TinyMCE Copy: Successfully copied ${result.length} files`))
//     .catch(console.error);


// Compile SCSS to CSS
gulp.task('sass', function () {
    const plugins = [
        tailwindcss('./tailwind.config.js'),
        autoprefixer({}),
    ];

    return gulp.src('./src/scss/*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(postcss(plugins))
        .pipe(concat({ path: "main.css" }))
        .pipe(gulp.dest('../wwwroot/css'));
});

// Compile TypeScript to JavaScript using Webpack
gulp.task('typescript', () => {

    // try {
    //     let tinyMceFiles = fs.readdirSync("../wwwroot/js/tinymce");

    //     if (!tinyMceFiles || !tinyMceFiles.length) {
    //         console.log("TinyMce Copy: starting ...");
    //         copyTinyMce();
    //     }
    // }
    // catch (err) {
    //     if (err.message.includes("ENOENT: no such file or directory")) {
    //         console.log("TinyMce Copy: starting ...");
    //         copyTinyMce();
    //     }
    //     else {
    //         console.error(err);
    //     }
    // }

    return gulp.src('./src/ts/main.ts')
        .pipe(webpack(webpackConfig))
        .pipe(gulp.dest('../wwwroot/js'));
});

// Watch for file changes
gulp.task('watch', function () {
    gulp.watch('src/scss/**/*.scss', gulp.series('sass'));
    gulp.watch(['src/ts/**/*.ts', '../**/*.cshtml'], gulp.series('typescript'));
});

//build and watch
gulp.task('dev', gulp.series('sass', 'typescript', 'watch'));

// Default task
gulp.task('default', gulp.series('sass', 'typescript'));
/// <binding BeforeBuild='cssmin, sass, uglify' ProjectOpened='watch:scss, watch:css, watch:js' />

module.exports = function (grunt) {
    'use strict';
    const sass = require('node-sass');

    //require('load-grunt-tasks')(grunt);

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        cssmin: {
            target: {
                files: [{
                    expand: true,
                    cwd: "wwwroot/css/",
                    src: ["*.css", "!*.min.css"],
                    dest: "wwwroot/css/",
                    ext: ".min.css"
                }]
            }
        },
        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: true //, Create source map
                //outputStyle: 'compressed' // Minify output
            },
            dist: {
                files: [
                    {
                        expand: true, // Recursive
                        cwd: "wwwroot/Scss", // The startup directory
                        src: ["**/*.scss"], // Source files
                        dest: "wwwroot/css", // Destination
                        ext: ".css" // File extension
                    }
                ]
            }
        },

        //uglify: {
        //    options: {
        //        mangle: false
        //    },
        //    all_js: {
        //        files: [
        //            {
        //                expand: true,
        //                cwd: "wwwroot/js/",
        //                src: ["*.js", "!*.min.js"],
        //                dest: "wwwroot/js/",
        //                ext: ".min.js"
        //            }
        //        ]
        //    },
        //},
        watch: {
            scss: {
                files: ['wwwroot/Scss/*.scss'],
                tasks: ['sass'],
            },
            css: {
                files: ['wwwroot/css/*.css'],
                tasks: ['cssmin'],
            }
            //,
            //js: {
            //    files: ['wwwroot/js/*.js'],
            //    tasks: ['uglify'],
            //}
        }
    });

    // Load the plugin
    grunt.loadNpmTasks('grunt-sass');
    //grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-watch');

    // Default task(s).
    grunt.registerTask('default', ['sass', /*'uglify',*/ 'cssmin', 'watch']);
};
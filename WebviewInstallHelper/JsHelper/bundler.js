const esbuild = require('esbuild');


esbuild.build({
    entryPoints: ['./main.js'],
    bundle: true,
    platform: 'node',
    outfile: 'dist/bundle.js',
    minify: true,
    external: []
}).then(function () { console.log("build success") }).catch(function (err) { throw new Error("get a error" + " " + err.toString()) })
var yargs = require('yargs');
var fs = require('fs');
var path = require('path');
var argv = yargs
    .option('string', {
        alias: 's',
        requiresArg: false,
        type: 'string',
        description: 'string content of the __NUXT__ function body',
    })
    .option('file', {
        alias: 'f',
        requiresArg: false,
        type: 'string',
        description: 'text file for reading as a fucntion body, if set to this, it will overwrite the string parameter',
    })
    .help()
    .argv;
window = {};

var functionBody = argv.string;

if (argv.file && argv.file.length && !fs.existsSync(argv.file)) {
    throw new Error(`cannot read file, ${argv.file} is not exists`);
}
functionBody = fs.readFileSync(argv.file, "utf-8");


try {
    eval(functionBody);
}
catch (e) {
    throw new Error(`cannot evalurate string, reason: ${e.message}`);
}

if (!window.__NUXT__) {
    throw new Error(`cannot find __NUXT__ variable in script`);
}

var allDownloads = [];
if (window.__NUXT__.fetch) {
    var webviewKey = Object.keys(window.__NUXT__.fetch).find(k => k.toLowerCase().indexOf("webview2") >= 0);

    if (webviewKey) {
        var webviewInfo = window.__NUXT__.fetch[webviewKey];


        if (webviewInfo.downloads && Array.isArray(webviewInfo.downloads) && webviewInfo.downloads.length > 0) {
            for (let d of webviewInfo.downloads) {
                for (let b of d.builds) {
                    if (b.architecture.toLowerCase().indexOf("arm64") >= 0) {
                        allDownloads.push({ Arch: 3, Url: b.url, Version: d.version })
                    }
                    else if (b.architecture.toLowerCase().indexOf("x64") >= 0) {
                        allDownloads.push({ Arch: 2, Url: b.url, Version: d.version })
                    }
                    else if (b.architecture.toLowerCase().indexOf("x86") >= 0) {
                        allDownloads.push({ Arch: 1, Url: b.url, Version: d.version })
                    }
                }
            }
        }
    }
}



if (allDownloads.length) {
    console.log({ IsSuccess: true, All: allDownloads });
} else {
    console.log({ IsSuccess: false });
}
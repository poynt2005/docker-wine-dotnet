var path = require("path");
var fs = require("fs");
var cp = require("child_process");

if (!fs.existsSync("node_modules")) {
    cp.execSync("npm i");
}
cp.execSync("npm run bundle");

var currentPath = process.cwd();
process.chdir("dist");
console.log("Create a configuration file building a blob that can be injected into the single executable application (see Generating single executable preparation blobs for details):");

fs.writeFileSync("sea-config.json", JSON.stringify({
    main: "bundle.js",
    output: "sea-prep.blob"
}), "utf-8");
console.log("Generate the blob to be injected:");
cp.execSync("node --experimental-sea-config sea-config.json");
console.log("Create a copy of the node executable and name it according to your needs:");
fs.copyFileSync(process.execPath, "jsHelper");
console.log("Inject the blob into the copied binary by running postject with the following options:");
cp.execSync("npx postject jsHelper NODE_SEA_BLOB sea-prep.blob --sentinel-fuse NODE_SEA_FUSE_fce680ab2cc467b6e072b8b5df1996b2");
process.chdir(currentPath);



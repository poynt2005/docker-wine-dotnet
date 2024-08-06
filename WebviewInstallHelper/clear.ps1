& dotnet clean
if (Test-Path .\bin) {
    Remove-Item -Recurse -Force -Path .\bin
}
if (Test-Path .\obj) {
    Remove-Item -Recurse -Force -Path .\obj
}
if (Test-Path .\JsHelper\dist) {
    Remove-Item -Recurse -Force -Path .\JsHelper\dist
}
if (Test-Path .\JsHelper\node_modules) {
    Remove-Item -Recurse -Force -Path .\JsHelper\node_modules
}
Remove-Item -Force -Path .\*.exe

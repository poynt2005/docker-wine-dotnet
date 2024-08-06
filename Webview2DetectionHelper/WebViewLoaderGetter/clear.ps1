& dotnet clean
if (Test-Path .\bin) {
    Remove-Item -Recurse -Force -Path .\bin
}
if (Test-Path .\obj) {
    Remove-Item -Recurse -Force -Path .\obj
}
Remove-Item -Force -Path .\*.exe

$webview2Ver = "Evergreen"
$outputPath = ".\webview2_evergreen.exe"

Write-Host "Currently downloading the webview installer..."
& .\WebviewInstallHelper.exe -v "Download" --webview-version $webview2Ver --webview-outpath $outputPath

Write-Host "Currently Install the webview installer"
& .\WebviewInstallHelper.exe -v "Install" --install-path $outputPath
#!/usr/bin/bash

Webview2Ver="Evergreen"
OutputPath="./webview2_evergreen.exe"

echo "Currently downloading the webview installer..."
./WebviewInstallHelper -v "Download" --webview-version $Webview2Ver --webview-outpath $OutputPath

echo "Currently Install the webview installer by WineTools"
./WineTools -v "InstallApp" --install-command "./WebviewInstallHelper -v \"Install\" --install-path $OutputPath"

# ------------------------- winetools-builder -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS winetools-builder
WORKDIR /progress
COPY WineTools /progress/WineTools
RUN cd ./WineTools \
    && dotnet \
        publish \
        ./WineTools.csproj \
        -c Release \
        -r linux-x64 \
        --self-contained \
        -p:PublishSingleFile=true \
        -p:DebugSymbols=false \
        -p:DebugType=None \
    && mv ./bin/Release/net8.0/linux-x64/publish /progress/WineToolsPublish

# ------------------------- jshelper-builder -------------------------
FROM node:22.5.1-bookworm AS jshelper-builder
WORKDIR /progress
COPY ./WebviewInstallHelper/JsHelper ./JsHelper
WORKDIR ./JsHelper
RUN npm i && node build.js && cp ./dist/jsHelper /progress/jsHelper


# ------------------------- webview2loader-getter -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS webview2loader-getter
WORKDIR /progress
COPY ./Webview2DetectionHelper/WebViewLoaderGetter /progress/WebViewLoaderGetter
WORKDIR ./WebViewLoaderGetter
RUN chmod +x build.sh \
    && ./build.sh \
    && cp ./bin/Release/net8.0/win-x64/publish/runtimes/win-x64/native/WebView2Loader.dll /progress/webview2loader.dll \
    && cp ./bin/Release/net8.0/win-x86/publish/runtimes/win-x86/native/WebView2Loader.dll /progress/webview2loader_i386.dll

# ------------------------- webview2detectionhelper-builder -------------------------
FROM ubuntu:noble AS webview2detectionhelper-builder
RUN apt update
RUN apt install -y mingw-w64
WORKDIR /progress
COPY ./Webview2DetectionHelper/helper.c ./helper.c
RUN x86_64-w64-mingw32-gcc ./helper.c -o helper.exe -lole32 \
    && i686-w64-mingw32-gcc ./helper.c -o helper_i386.exe -lole32 


# ------------------------- webviewinstallhelper-builder -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS webviewinstallhelper-builder
WORKDIR /progress
COPY WebviewInstallHelper /progress/WebviewInstallHelper
WORKDIR ./WebviewInstallHelper
RUN dotnet \
        publish \
        ./WebviewInstallHelper.csproj \
        -c Release \
        -r linux-x64 \
        --self-contained \
        -p:PublishSingleFile=true \
        -p:DebugSymbols=false \
        -p:DebugType=None \
    && mv ./bin/Release/net8.0/linux-x64/publish /progress/WebviewInstallHelperPublish


# ------------------------- main stage -------------------------
FROM poynt2005/docker-wine-dotnet:dotnet48-wine96-devel AS dotnet-base
RUN apt update
RUN apt install imagemagick xdotool -y
WORKDIR /debug
WORKDIR /progress
COPY changewin.instructions.json taskuninstallerscreenshot.instructions.json ./
COPY --from=winetools-builder /progress/WineToolsPublish/WineTools ./WineTools
COPY --from=webviewinstallhelper-builder /progress/WebviewInstallHelperPublish/WebviewInstallHelper ./WebviewInstallHelper
COPY --from=jshelper-builder /progress/jsHelper ./jsHelper
COPY --from=webview2loader-getter /progress/webview2loader.dll ./webview2loader.dll
COPY --from=webview2loader-getter /progress/webview2loader_i386.dll ./webview2loader_i386.dll
COPY --from=webview2detectionhelper-builder /progress/helper.exe ./Webview2DetectionHelper.exe 
COPY --from=webview2detectionhelper-builder /progress/helper_i386.exe ./Webview2DetectionHelper_i386.exe 


RUN ./WebviewInstallHelper -v "Download" --webview-version "Evergreen" --webview-outpath "webview2_evergreen.exe"
RUN ./WineTools -v "InstallApp" --install-command "winecfg" --instruction-file "changewin.instructions.json"

RUN ./WineTools -v "InstallApp" --install-command "./WebviewInstallHelper -v \"Install\" --install-path \"webview2_evergreen.exe\"" \
    && wine Webview2DetectionHelper_i386.exe webview2loader_i386.dll && cat 9c7d6346-ca8d-4340-b514-0339e498a37d.txt \
    && ./WineTools -v "InstallApp" --install-command "wine uninstaller" --instruction-file "taskuninstallerscreenshot.instructions.json"
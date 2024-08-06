& dotnet publish `
    .\WebViewLoaderGetter.csproj `
    -c Release `
    -r win-x86 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:DebugSymbols=false `
    -p:DebugType=None
& dotnet publish `
    .\WebViewLoaderGetter.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:DebugSymbols=false `
    -p:DebugType=None
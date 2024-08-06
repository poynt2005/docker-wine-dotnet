using System.Runtime.InteropServices;
using CommandLine;
using CommandLine.Text;
using WebviewInstallerHelper.Args;
using WebviewInstallerHelper.Lib;

var isAmd64 = false;
var wineprefixPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".wine");
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    isAmd64 = Environment.Is64BitOperatingSystem;
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    var wineArchEnv = Environment.GetEnvironmentVariable("WINEARCH");

    if (!String.IsNullOrEmpty(wineArchEnv))
    {
        if (wineArchEnv.Trim() == "win64")
        {
            isAmd64 = true;
        }
    }

    var winePrefixPathEnv = Environment.GetEnvironmentVariable("WINEPREFIX");
    if (!String.IsNullOrEmpty(winePrefixPathEnv))
    {
        wineprefixPath = winePrefixPathEnv.Trim();
    }
}
else
{
    throw new Exception("current os platform is not supported");
}

var parsedResult = Parser.Default.ParseArguments<HelperOptions>(args);

parsedResult.WithParsed<HelperOptions>(option =>
{
    Task.Run(async () =>
    {
        if (String.IsNullOrEmpty(option.Verb))
        {
            Console.WriteLine("You must specific at least one verb Download or Install");

            var helpText = HelpText.AutoBuild(parsedResult,
                                          h => HelpText.DefaultParsingErrorsHandler(parsedResult, h),
                                          e => e);
            Console.WriteLine(helpText);

            Environment.Exit(-1);
        }

        if (option.Verb == "Download")
        {
            if (String.IsNullOrEmpty(option.WebviewVersion) || (option.WebviewVersion.ToLower() != "EvergreenBootstrap".ToLower() && option.WebviewVersion.ToLower() != "Evergreen".ToLower()))
            {
                Console.WriteLine("webview-version argument must be EvergreenBootstrap or Evergreen");
                Environment.Exit(-1);
            }

            WebviewTargetVersion version = WebviewTargetVersion.Fixed;
            if (option.WebviewVersion.ToLower() == "EvergreenBootstrap".ToLower())
            {
                version = WebviewTargetVersion.EvergreenBootstrap;
            }
            else if (option.WebviewVersion.ToLower() == "Evergreen".ToLower())
            {
                version = WebviewTargetVersion.Evergreen;
            }

            if (String.IsNullOrEmpty(option.WebviewOutPath))
            {
                Console.WriteLine("You must specific a download path");
                Environment.Exit(-1);
            }

            Fetch f = new(version, isAmd64 ? WebviewInstallerHelper.Lib.Architecture.Amd64 : WebviewInstallerHelper.Lib.Architecture.X86);
            await Utils.UrlDownloadToFile(f.DownloadUrl, option.WebviewOutPath);

            Console.WriteLine($"Installer File Downloaded to {option.WebviewOutPath}");
        }
        else if (option.Verb == "Install")
        {
            if (String.IsNullOrEmpty(option.InstallPath))
            {
                Console.WriteLine("You must specific a installer executable path");
                Environment.Exit(-1);
            }

            Install ins = new(option.InstallPath);
            ins.IsAmd64 = isAmd64;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

                ins.WinePrefixFolder = wineprefixPath;
            }
            await ins.StartMainFlow();

            Console.WriteLine($"Webview2 installation completed");
        }
    }).Wait();
}).WithNotParsed<HelperOptions>(option =>
{
    var helpText = HelpText.AutoBuild(parsedResult,
                                          h => HelpText.DefaultParsingErrorsHandler(parsedResult, h),
                                          e => e);
    Console.WriteLine(helpText);
});




using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace WebviewInstallerHelper.Args;

public class HelperOptions
{
    [Option('v', "verb", Required = false, HelpText = "Specific Helper Main Verb, must be Download or Install")]
    public string Verb { get; set; } = String.Empty;

    [Option("webview-version", Required = false, HelpText = "Specific webview download version for Download verb, must be Evergreen or EvergreenBootstrap")]
    public string WebviewVersion { get; set; } = String.Empty;

    [Option("webview-outpath", Required = false, HelpText = "Specific webview download path for Download verb")]
    public string WebviewOutPath { get; set; } = String.Empty;

    [Option("install-path", Required = false, HelpText = "Specific webview installer path for Install verb")]
    public string InstallPath { get; set; } = String.Empty;
}
namespace WebviewInstallerHelper.Lib;


using System.Diagnostics;
using System.Runtime.InteropServices;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft;
using Newtonsoft.Json;

enum WebviewTargetVersion
{
    Evergreen = 0,
    EvergreenBootstrap,
    Fixed
}

enum Architecture
{
    Any = 0,
    X86,
    Amd64,
    Arm64
}

struct JsHelperUrlCollectionsInfo
{
    public Architecture Arch;
    public string Url;
    public string Version;
}

struct JsHelperInfo
{
    public bool IsSuccess;
    public JsHelperUrlCollectionsInfo[] All;
}



class Fetch
{
    private static readonly Dictionary<string, string> FixedUrls = new()
    {
        { "EvergreenBootstrap_Any", "https://go.microsoft.com/fwlink/p/?LinkId=2124703" },
        { "Evergreen_X86", "https://go.microsoft.com/fwlink/?linkid=2099617" },
        { "Evergreen_Amd64", "https://go.microsoft.com/fwlink/?linkid=2124701" },
        { "Evergreen_Arm64", "https://go.microsoft.com/fwlink/?linkid=2099616" },
    };


    public Architecture Architecture { get; private set; }
    public WebviewTargetVersion Version { get; private set; }
    public string? VersionString { get; private set; } = null;
    public string DownloadUrl { get; private set; } = String.Empty;

    public Fetch(WebviewTargetVersion ver, Architecture arch)
    {
        Architecture = arch;
        Version = ver;


        if (Version == WebviewTargetVersion.EvergreenBootstrap)
        {
            Architecture = Architecture.Any;
        }
        else
        {
            if (Architecture == Architecture.Any)
            {
                throw new Exception("cannot bind non bootstrap installer to \"ANY\" type of platform");
            }
        }

        if (ver != WebviewTargetVersion.Fixed)
        {
            DownloadUrl = FixedUrls[$"{Enum.GetName(typeof(WebviewTargetVersion), ver)}_{Enum.GetName(typeof(Architecture), arch)}"];
        }
    }

    public Fetch(WebviewTargetVersion ver) : this(ver, Architecture.Amd64)
    {

    }

    public static async Task<Fetch[]> FromCurrentSite(Architecture arch)
    {
        var resultHtml = String.Empty;

        try
        {
            using HttpClient client = new();
            using HttpRequestMessage req = new(HttpMethod.Get, "https://developer.microsoft.com/en-us/microsoft-edge/webview2");
            req.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");

            using var res = await client.SendAsync(req);
            res.EnsureSuccessStatusCode();

            resultHtml = await res.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"cannot get html string from webview page, reason: {ex.Message}");
        }


        if (String.IsNullOrEmpty(resultHtml))
        {
            throw new Exception("webview2 page returned an empty html string");
        }

        using var context = BrowsingContext.New();
        using var doc = await context.OpenAsync(req => req.Content(resultHtml));

        var nuxtScriptNode = doc.QuerySelectorAll("script").FirstOrDefault(s => s.Text().Trim().StartsWith("window.__NUXT__"));
        if (nuxtScriptNode == default(IElement))
        {
            throw new Exception("cannot select __NUXT__ node from current html");
        }

        var functionBodyTextFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Guid.NewGuid().ToString()}_functionbody.txt");
        File.WriteAllText(functionBodyTextFile, nuxtScriptNode.Text());


        var jsHelperExecutable = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jsHelper");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var jsHelperExecutableForWindows = jsHelperExecutable + ".exe";
            File.Copy(jsHelperExecutable, jsHelperExecutableForWindows, true);
            jsHelperExecutable = jsHelperExecutableForWindows;
        }

        ProcessStartInfo si = new()
        {
            FileName = jsHelperExecutable,
            UseShellExecute = false,
            Arguments = $" --file {functionBodyTextFile}",
            RedirectStandardOutput = true
        };

        using Process proc = new() { StartInfo = si };
        proc.Start();
        var resultFromJsHelper = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();
        var info = JsonConvert.DeserializeObject<JsHelperInfo>(resultFromJsHelper);
        if (!info.IsSuccess)
        {
            throw new Exception("js context returned a invalid result, data loss");
        }

        List<Fetch> toReturn = [];
        foreach (var b in info.All)
        {
            if (b.Arch == arch)
            {
                Fetch f = new(WebviewTargetVersion.Fixed)
                {
                    Architecture = arch,
                    VersionString = b.Version,
                    DownloadUrl = b.Url
                };
                toReturn.Add(f);
            }
        }

        return [.. toReturn];
    }
}
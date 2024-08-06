using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WebviewInstallerHelper.Lib;


static class Utils
{
    private static readonly string WebviewCheckResultFileUuid = "9c7d6346-ca8d-4340-b514-0339e498a37d";
    public static async Task UrlDownloadToFile(string url, string resultPath)
    {
        using HttpClient client = new();
        using HttpRequestMessage request = new(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var fileStream = File.Open(resultPath, FileMode.CreateNew, FileAccess.Write);
        using var httpByteStream = await response.Content.ReadAsStreamAsync();
        await httpByteStream.CopyToAsync(fileStream);
    }

    public static string GetLinuxCurrentUserName()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new Exception("this method can only use on linux platform");
        }

        ProcessStartInfo si = new()
        {
            FileName = "whoami",
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using var proc = new Process() { StartInfo = si };
        proc.Start();

        var outputStr = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        return outputStr.Trim();
    }

    public static bool CheckWebview2IsInstalledInMachine(bool isAmd64)
    {
        ProcessStartInfo pi = new() { UseShellExecute = false };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            pi.FileName = isAmd64 ? "wine64" : "wine";
            pi.Arguments = $" {(isAmd64 ? "Webview2DetectionHelper.exe" : "Webview2DetectionHelper_i386.exe")} {(isAmd64 ? "webview2loader.dll" : "webview2loader_i386.dll")}";
        }
        else
        {
            pi.FileName = $"{(isAmd64 ? "Webview2DetectionHelper.exe" : "Webview2DetectionHelper_i386.exe")}";
            pi.Arguments = $" {(isAmd64 ? "webview2loader.dll" : "webview2loader_i386.dll")}";
        }
        using Process proc = new() { StartInfo = pi };
        proc.Start();
        proc.WaitForExit();
        var txtContains = Encoding.Unicode.GetString(File.ReadAllBytes($"{WebviewCheckResultFileUuid}.txt"));
        return txtContains != "Not Installed";
    }
}
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WebviewInstallerHelper.Lib;

class Install
{
    public bool IsAmd64 { get; set; } = false;
    public string WinePrefixFolder { get; set; } = String.Empty;
    private static bool IsUsingWine = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    private string InstallPath { get; }

    public Install(string installerPath)
    {
        if (!File.Exists(installerPath))
        {
            throw new Exception($"cannot stat {installerPath}, installer not exists");
        }

        InstallPath = Path.GetFullPath(installerPath);
    }

    private void InstallThread()
    {
        Task.Run(() =>
        {
            ProcessStartInfo pi = new()
            {
                FileName = IsUsingWine ? (IsAmd64 ? "wine64" : "wine") : InstallPath,
                Arguments = IsUsingWine ? $" {InstallPath}" : String.Empty,
                UseShellExecute = false,
            };

            using var proc = new Process() { StartInfo = pi };
            proc.Start();
            proc.WaitForExit();
        });
    }

    private async Task InstallThreadAwaiter()
    {
        if (Utils.CheckWebview2IsInstalledInMachine(IsAmd64))
        {
            return;
        }

        bool isCompleted = false;
        for (int i = 0; i < 300; ++i)
        {
            if (Utils.CheckWebview2IsInstalledInMachine(IsAmd64))
            {
                isCompleted = true;
                break;
            }
            await Task.Delay(10000);
        }

        if (!isCompleted)
        {
            throw new Exception("cannot install webview2 due to a installation timeout");
        }
    }

    public async Task StartMainFlow()
    {
        InstallThread();
        await InstallThreadAwaiter();
    }

}
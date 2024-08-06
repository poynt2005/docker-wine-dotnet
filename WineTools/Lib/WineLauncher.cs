using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace WineTools.Lib
{
    class WineLauncher
    {
        private XvfbChecker XvfbChecker { get; } = XvfbChecker.NewXvfbChecker();

        private static bool CheckIsWineInited()
        {
            var winePrefixPath = Environment.GetEnvironmentVariable("WINEPREFIX");

            if (String.IsNullOrEmpty(winePrefixPath))
            {
                Console.WriteLine("[WineTools][Info ] Environment variable WINEPREFIX is not set, use /root/.wine instead");
                winePrefixPath = "/root/.wine";
            }
            return Directory.Exists(winePrefixPath) && Directory.Exists(Path.Combine(winePrefixPath, "drive_c"));
        }

        public WineLauncher()
        {
            XvfbChecker.Checking = true;
        }

        private void ProcOutputHandler(Process proc, string? data)
        {
            Regex searchPattern = new(@"configuration (.+)? has been updated");
            if (!String.IsNullOrEmpty(data))
            {
                Console.WriteLine($"[WineTools][Info ] Winecfg output: {data}");
                if (searchPattern.IsMatch(data))
                {
                    proc.SendSigTerm();
                }
            }
        }

        public async Task Launch()
        {
            if (CheckIsWineInited())
            {
                Console.WriteLine("[WineTools][Info ] wine prefix has been inited, no need to init again");
                return;
            }

            Console.WriteLine("[WineTools][Info ] Waiting for Xvfb running...");
            XvfbChecker.WaitForXvfbRun().Wait();

            Console.WriteLine("[WineTools][Info ] Try to launch winecfg");

            ProcessStartInfo startInfo = new()
            {
                FileName = "/usr/bin/winecfg",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using Process proc = new()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            Console.WriteLine($"[WineTools][Info ] Winecfg process has started");
            proc.Start();

            proc.OutputDataReceived += new((sender, e) =>
            {
                ProcOutputHandler(proc, e.Data);
            });

            proc.ErrorDataReceived += new((sender, e) =>
            {
                ProcOutputHandler(proc, e.Data);
            });

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            try
            {
                await proc.WaitForExitAsync();
                proc.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WineTools][Info ] Winecfg wait error: {ex.Message}");
            }
        }
    }
}
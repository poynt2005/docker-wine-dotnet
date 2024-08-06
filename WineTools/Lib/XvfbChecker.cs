using System.Diagnostics;

namespace WineTools.Lib
{
    class XvfbChecker
    {
        private static readonly int TRY_TIMES = 20;
        private static XvfbChecker? Instance = null;

        private bool __Checking { get; set; } = false;
        private bool PausePooling { get; set; } = false;
        private bool NeedToShowXvfbPid { get; set; } = false;

        public int XvfbPid { get; private set; } = -1;

        public bool Checking
        {
            get
            {
                return __Checking;
            }
            set
            {
                if (!__Checking && value)
                {
                    __Checking = true;
                    StartCheckPooling();
                }

                __Checking = value;
            }
        }

        private void CheckRunning()
        {
            var processes = Process.GetProcesses();
            bool found = false;
            foreach (var proc in processes)
            {
                if (proc.ProcessName.ToLower().Contains("xvfb"))
                {
                    if (NeedToShowXvfbPid)
                    {
                        Console.WriteLine($"[WineTools][Info ] Detected xvfb process: {proc.ProcessName}");
                    }

                    XvfbPid = proc.Id;
                    found = true;

                    break;
                }
            }

            if (!found)
            {
                XvfbPid = -1;
            }
        }

        private async Task TryStartXvfbProcess()
        {
            for (int i = 0; i < TRY_TIMES; ++i)
            {
                _ = Task.Run(() => Utils.CSystem("entrypoint"));
                await Task.Delay(2000);

                CheckRunning();

                if (XvfbPid > 0)
                {
                    return;
                }
                await Task.Delay(2000);
            }

            Console.Error.WriteLine($"[WineTools][Info ] Cannot run xvfb process, reached try times: {TRY_TIMES}");
            throw new Exception("cannot run xvfb process");
        }

        private void StartCheckPooling()
        {
            NeedToShowXvfbPid = true;
            Task.Run(async () =>
            {
                Console.WriteLine("[WineTools][Info ] Start to check whether xvfb process is running");
                while (__Checking)
                {
                    if (PausePooling)
                    {
                        continue;
                    }

                    CheckRunning();

                    if (XvfbPid > 0)
                    {
                        if (NeedToShowXvfbPid)
                        {
                            Console.WriteLine($"[WineTools][Info ] Found xvfb process with pid: {XvfbPid}");
                            NeedToShowXvfbPid = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[WineTools][Info ] Xvfb process is not running");
                        Console.WriteLine($"[WineTools][Info ] Tryna to start a Xvfb process...");
                        PausePooling = true;

                        await TryStartXvfbProcess();
                        NeedToShowXvfbPid = true;
                        PausePooling = false;
                    }

                    await Task.Delay(1000);
                }
            });
        }

        private XvfbChecker()
        {
            Checking = true;
        }

        public async Task WaitForXvfbRun()
        {
            while (true)
            {
                if (XvfbPid > 0)
                {
                    return;
                }
                await Task.Delay(100);
            }
        }

        public static XvfbChecker NewXvfbChecker()
        {
            Instance ??= new();
            return Instance;
        }
    }
}
using System.Collections.Generic;

namespace WineTools.Lib
{
    class WinetricksInstall
    {
        private XvfbChecker XvfbChecker { get; } = XvfbChecker.NewXvfbChecker();
        private List<string> Verbs { get; } = [];

        public WinetricksInstall()
        {
            XvfbChecker.Checking = true;
        }

        public void AddVerbs(string verb)
        {
            Verbs.Add(verb);
        }

        public void Install()
        {
            Console.WriteLine("[WineTools][Info ] Waiting for Xvfb running...");
            XvfbChecker.WaitForXvfbRun().Wait();

            foreach (var verb in Verbs)
            {
                Console.WriteLine($"[WineTools][Info ] Try to execute winetricks verb {verb}");
                Utils.CSystem($"timeout 10m winetricks --unattended --force {verb}");
            }
        }
    }
}
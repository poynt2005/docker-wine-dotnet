using System.IO;

namespace WineTools.Lib
{
    class AppInstaller
    {
        private XvfbChecker XvfbChecker { get; } = XvfbChecker.NewXvfbChecker();
        private XdoToolInstall? XdoToolInstaller { get; set; } = null;

        public AppInstaller()
        {
            XvfbChecker.Checking = true;
        }

        public void InstallBySimpleCommand(string command)
        {
            Console.WriteLine("[WineTools][Info ] Waiting for Xvfb running...");
            XvfbChecker.WaitForXvfbRun().Wait();

            Utils.CSystem(command);
        }

        public void StartUpCommand(string command)
        {
            Task.Run(() =>
            {
                Utils.CSystem(command);
            });
        }

        public void InstallByXdoToolInstaller(string instructionSetFilePath)
        {
            if (!File.Exists(instructionSetFilePath))
            {
                Console.Error.WriteLine($"[WineTools][Error] Instructions json file path: {instructionSetFilePath} is not exists");
            }

            XdoToolInstaller = XdoToolInstall.UnMarshalFromFile(instructionSetFilePath);

            XdoToolInstaller.Install();
        }
    }

}
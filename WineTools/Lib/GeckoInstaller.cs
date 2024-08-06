namespace WineTools.Lib
{
    class GeckoInstaller
    {
        private XdoToolInstall XdoToolInstaller { get; } = new();

        private WineArch WineArch { get; } = Utils.GetWineArch();

        public GeckoInstaller()
        {
            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Install gecko...",
                Command = "echo",
                SleepTime = 5
            });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click Install button",
                Command = "xdotool mousemove 448 102 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss1.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click Input text",
                Command = "xdotool mousemove 208 243 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss2.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Send location to input",
                Command = """xdotool type "Z:\usr\share\wine" """,
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss3.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click Open button",
                Command = "xdotool mousemove 383 239 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss4.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Select wine directory",
                Command = "xdotool mousemove 48 95 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss5.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click Open button",
                Command = "xdotool mousemove 383 239 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss6.png && mv *.png /share",
            //     SleepTime = 5
            // });


            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Select different msi by wine arch",
                Command = WineArch == WineArch.x86 ? "xdotool mousemove 108 96 click 1" : "xdotool mousemove 120 112 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss7.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click Open button",
                Command = "xdotool mousemove 383 239 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss8.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Prevent error dialog",
                Command = "xdotool mousemove 497 406 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss9.png && mv *.png /share",
            //     SleepTime = 5
            // });

            XdoToolInstaller.AddInstructionToSequence(new()
            {
                Description = "Click OK button",
                Command = "xdotool mousemove 293 430 click 1",
                SleepTime = 5
            });

            // XdoToolInstaller.AddInstructionToSequence(new()
            // {
            //     Description = "Capture screenshot",
            //     Command = "import -window root ss10.png && mv *.png /share",
            //     SleepTime = 5
            // });
        }

        public void Install()
        {
            Console.WriteLine("[WineTools][Info ] Instructions is set, try to install Wine-Gecko");
            Console.WriteLine($"[WineTools][Info ] Launching wine uninstaller...");

            Task.Run(() =>
            {
                if (Utils.GetWineArch() == WineArch.x86)
                {
                    Utils.CSystem("wine uninstaller");
                }
                else
                {
                    Utils.CSystem("wine64 uninstaller");
                }
            });

            XdoToolInstaller.Install();
        }
    }
}
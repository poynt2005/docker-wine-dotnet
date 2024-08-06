using System;
using CommandLine;
using WineTools.Args;
using WineTools.Lib;

String[] verbs = [
    "LaunchWineConfig",
    "InstallWineGecko",
    "InstallApp",
    "WinetrikcsInstall"
];

Parser.Default.ParseArguments<WineToolsOptions>(args)
    .WithParsed<WineToolsOptions>(option =>
    {
        if (!option.ListVerb && String.IsNullOrEmpty(option.Verb))
        {
            Console.Error.WriteLine("[WineTools][Error] No command to execute");
            throw new Exception("no command to execute");
        }

        if (option.ListVerb)
        {
            Console.WriteLine("""
                List of supported verbs:
                    1. LaunchWineConfig: Launch Wine prefix by using "winecfg" command
                    2. InstallWineGecko: Install Wine-Gecko to apply Internet Explorer features
                    3. InstallApp: Install your own app, you must provide at least one simple install command line or xdotool instruction set file
                    4. WinetrikcsInstall: Install windows services by winetricks verbs, verbs must be separated by comma sign

                PS: You must install xvfb, xdotool and imagemagick to perform this installation
            """);

            return;
        }

        if (!verbs.Contains(option.Verb))
        {
            Console.Error.WriteLine($"[WineTools][Error] Verb {option.Verb} is not supported in WineTools");
            throw new Exception("verb not supported");
        }

        switch (option.Verb)
        {
            case "LaunchWineConfig":
                {
                    Console.WriteLine("[WineTools][Info ] Verb 'LaunchWineConfig' will help you launcher a windows environment by WINEPREFIX path set in the environment variable");
                    WineLauncher launcher = new();
                    launcher.Launch().Wait();

                    Console.WriteLine("[WineTools][Info ] Wine prefix has been initialized");
                    return;
                }
            case "InstallWineGecko":
                {
                    Console.WriteLine("[WineTools][Info ] Verb 'InstallWineGecko' will help you install the Wine-Gecko software by installer provided in docker container 'scottyhardy/docker-wine'");
                    GeckoInstaller installer = new();
                    installer.Install();

                    Console.WriteLine("[WineTools][Info ] Wine-Gecko has been installed");
                    return;
                }
            case "InstallApp":
                {
                    Console.WriteLine("[WineTools][Info ] Verb 'InstallApp' will help you install app by simple command or xdotool instruction file, it will apply simple command and than apply xdotool instruction file");
                    if (String.IsNullOrEmpty(option.InstallCommand) && String.IsNullOrEmpty(option.InstructionFilePath))
                    {
                        Console.Error.WriteLine($"[WineTools][Error] You CANNOT install a app without provide a command line or instruction file");
                        throw new Exception("app installation lack of instructions");
                    }

                    if (!String.IsNullOrEmpty(option.InstructionFilePath) && String.IsNullOrEmpty(option.InstallCommand))
                    {
                        Console.Error.WriteLine($"[WineTools][Error] You CANNOT install a app by XdoToolInstaller without provide a startup command to it");
                        throw new Exception("app xdotool installation lack of start command");
                    }

                    AppInstaller installer = new();

                    if (!String.IsNullOrEmpty(option.InstructionFilePath))
                    {
                        installer.StartUpCommand(option.InstallCommand);
                        installer.InstallByXdoToolInstaller(option.InstructionFilePath);
                    }
                    else
                    {
                        installer.InstallBySimpleCommand(option.InstallCommand);
                    }

                    Console.WriteLine("[WineTools][Info ] App has been installed");
                    return;
                }
            case "WinetrikcsInstall":
                {
                    Console.WriteLine("[WineTools][Info ] Verb 'WinetrikcsInstall' will help you install windows services by winetricks");

                    if (String.IsNullOrEmpty(option.WineVerb))
                    {
                        Console.Error.WriteLine($"[WineTools][Error] You did not provide winetricks verbs");
                        throw new Exception("winetricks lack of install verbs");
                    }

                    WinetricksInstall installer = new();

                    foreach (var verb in option.WineVerb.Split(','))
                    {
                        installer.AddVerbs(verb.Trim());
                    }
                    installer.Install();
                    return;
                }
        }

    });
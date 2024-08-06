using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace WineTools.Args
{
    public class WineToolsOptions
    {
        [Option('v', "verb", Required = false, HelpText = "Execute which WineTools' verbs")]
        public string Verb { get; set; } = String.Empty;

        [Option('l', "list-verb", Required = false, HelpText = "List all supported verbs")]
        public bool ListVerb { get; set; } = false;

        [Option("instruction-file", Required = false, HelpText = "Instruction file to install App by XdoToolInstaller")]
        public string InstructionFilePath { get; set; } = String.Empty;

        [Option("install-command", Required = false, HelpText = "Command to install App by simple command line install")]
        public string InstallCommand { get; set; } = String.Empty;

        [Option("wine-verb", Required = false, HelpText = "Winetricks verbs to install, separated by comma sign")]
        public string WineVerb { get; set; } = String.Empty;
    }

}
using Newtonsoft.Json;
using System.IO;

namespace WineTools.Lib
{
    public class Instruction
    {
        public string Description { get; set; } = "Install Instruction Unknown";
        public string Command { get; set; } = "echo \"Install Instruction Unknown\"";

        public int SleepTime { get; set; } = 2;
    }


    class XdoToolInstall
    {
        private List<Instruction> InstructionSequences { get; set; } = [];
        private XvfbChecker XvfbChecker { get; } = XvfbChecker.NewXvfbChecker();

        public XdoToolInstall()
        {
            XvfbChecker.Checking = true;
        }

        public void AddInstructionToSequence(Instruction instruction)
        {
            InstructionSequences.Add(instruction);
        }

        public static XdoToolInstall UnMarshalFromFile(string instructionJsonPath)
        {
            if (!File.Exists(instructionJsonPath))
            {
                Console.Error.WriteLine($"[WineTools][Error] Unable to find json instruction file: {instructionJsonPath}");
                throw new Exception("target instruction file not found");
            }

            var contents = File.ReadAllText(instructionJsonPath);

            var instructionSequences = JsonConvert.DeserializeObject<List<Instruction>>(contents);

            if (instructionSequences == null)
            {
                Console.Error.WriteLine($"[WineTools][Error] Unable to unmarshal json instruction file: {instructionJsonPath}, maybe it hass a wrong format");
                throw new Exception("cannot unmarshal instruction file");
            }

            return new()
            {
                InstructionSequences = instructionSequences
            };
        }

        public void Install()
        {
            Console.WriteLine("[WineTools][Info ] Waiting for Xvfb running...");
            XvfbChecker.WaitForXvfbRun().Wait();

            Task.Run(async () =>
            {
                foreach (var instruction in InstructionSequences)
                {
                    Console.WriteLine($"[WineTools][Info ] XdoTool Installer executing description: {instruction.Description}, and wait this instruction for {instruction.SleepTime} second(s)");
                    Utils.CSystem(instruction.Command);
                    await Task.Delay(instruction.SleepTime * 1000);
                }
            }).Wait();
        }

    }

}
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WineTools.Lib
{
    enum WineArch
    {
        x86 = 0,
        amd64
    }

    static class Utils
    {
        [DllImport("libc.so.6", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int system(IntPtr command);

        [DllImport("libc.so.6", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int kill(int pid, int sig);

        public static int CSystem(string command)
        {
            var commandStrPtr = Marshal.StringToHGlobalAnsi(command);
            int exitCode = system(commandStrPtr);
            Marshal.FreeHGlobal(commandStrPtr);
            return exitCode;
        }

        public static void SendSigTerm(this Process proc)
        {
            if (kill(proc.Id, 15) != 0)
            {
                throw new Exception($"cannot send SIGTERM to pid {proc.Id}");
            }
        }

        public static WineArch GetWineArch()
        {
            var wineArchEnv = Environment.GetEnvironmentVariable("WINEARCH");

            if (String.IsNullOrEmpty(wineArchEnv))
            {
                Console.WriteLine("[WineTools][Info ] Cannot get wine architecure from environment variable, use x86 instead");
                return WineArch.x86;
            }

            if (wineArchEnv.ToLower() == "win64")
            {
                return WineArch.amd64;
            }

            return WineArch.x86;
        }
    }
}
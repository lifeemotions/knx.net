using System.Diagnostics;

namespace KNXLibTests.Support.Eibd
{
    internal class DaemonManager
    {
        private const string EibdExecutable = @"eibd";
        private const string EibdExecutableRoutingParameters = @"-c -D -R -S -u ip:";
        private const string EibdExecutableTunnelingParameters = @"-c -D -T -S -u ip:";

        private static Process EibdProcess { get; set; }

        public static bool IsEibdAvailable()
        {
            return Os.Tools.ExistsOnPath(EibdExecutable);
        }

        public static bool StartRouting()
        {
            var filename = Os.Tools.GetFullPath(EibdExecutable);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            try
            {
                Stop();
                EibdProcess = Process.Start(filename, EibdExecutableRoutingParameters);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool StartTunneling()
        {
            var filename = Os.Tools.GetFullPath(EibdExecutable);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            try
            {
                Stop();
                EibdProcess = Process.Start(filename, EibdExecutableTunnelingParameters);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool Stop()
        {
            try
            {
                if (EibdProcess != null)
                    EibdProcess.Kill();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KNXLibTests.Support.Eibd
{
    internal class VBusMonitorManager
    {
        private const string VBusMonitorExecutable = @"vbusmonitor1";
        private const string VBusMonitorParameters = @"local:/tmp/eib";

        private static Process VBusMonitorProcess { get; set; }

        public static bool IsVBusMonitorAvailable()
        {
            return Os.Tools.ExistsOnPath(VBusMonitorExecutable);
        }

        public static bool Start()
        {
            var filename = Os.Tools.GetFullPath(VBusMonitorExecutable);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            try
            {
                Stop();
                VBusMonitorProcess = new Process();
                VBusMonitorProcess.StartInfo.UseShellExecute = false;
                VBusMonitorProcess.StartInfo.RedirectStandardOutput = true;
                VBusMonitorProcess.StartInfo.FileName = filename;
                VBusMonitorProcess.StartInfo.Arguments = VBusMonitorParameters;
                VBusMonitorProcess.OutputDataReceived += VBusMonitorOnDataReceived;
                VBusMonitorProcess.Start();
                VBusMonitorProcess.BeginOutputReadLine();
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
                if (VBusMonitorProcess != null)
                    VBusMonitorProcess.Kill();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private const string Regex1 = @"to ([0-9/]*) hops";
        private const string Regex2 = @"\) ([0-9\s]*)$";

        private static void VBusMonitorOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Receives this:
            // LPDU: BC 00 01 08 06 F1 00 81 3C :L_Data low from 0.0.1 to 1/0/6 hops: 07 T_DATA_XXX_REQ A_GroupValue_Write (small) 01

            if (!e.Data.Contains("A_GroupValue_Write"))
                return;

            string address = string.Empty;
            string value = string.Empty;

            Regex regex = new Regex(Regex1);
            Match match = regex.Match(e.Data);

            if (match.Success)
                address = match.Groups[1].Value;

            regex = new Regex(Regex2);
            match = regex.Match(e.Data);

            if (match.Success)
                value = match.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(value))
                return;

            //TODO: check how it works if value received is more than 1 byte

            if (GroupWrite.IsGroupWriteAvailable())
                GroupWrite.Send(address, value);
        }
    }
}

using System.Diagnostics;

namespace KNXLibTests.Support.Eibd
{
    internal class SendAction
    {
        private const string GroupWriteExecutable = @"groupswrite";
        private const string GroupWriteParameters = @"local:/tmp/eib {0} {1}";

        public static bool IsGroupWriteAvailable()
        {
            return Os.Tools.ExistsOnPath(GroupWriteExecutable);
        }

        public bool Send(string groupAddress, string value)
        {
            var filename = Os.Tools.GetFullPath(GroupWriteExecutable);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            try
            {
                Process.Start(filename, string.Format(GroupWriteParameters, groupAddress, value));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

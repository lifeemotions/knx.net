using System.Diagnostics;

namespace KNXLibTests.Support.Eibd
{
    internal class GroupWrite
    {
        // groupswrite only allows 1 byte APDU, for bigger APDU, groupwrite must be used
        private const string GroupWriteExecutable = @"groupwrite";
        private const string GroupWriteParameters = @"local:/tmp/eib {0} {1}";

        public static bool IsGroupWriteAvailable()
        {
            return Os.Tools.ExistsOnPath(GroupWriteExecutable);
        }

        public static bool Send(string groupAddress, string value)
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

using System.Diagnostics;

namespace KNXLibTests.Support.Eibd
{
    internal class GroupResponse
    {
        // groupsresponse only allows 1 byte APDU, for bigger APDU, groupresponse must be used
        private const string GroupResponseExecutable = @"groupresponse";
        private const string GroupResponseParameters = @"local:/tmp/eib {0} {1}";

        public static bool IsGroupResponseAvailable()
        {
            return Os.Tools.ExistsOnPath(GroupResponseExecutable);
        }

        public static bool Send(string groupAddress, string value)
        {
            var filename = Os.Tools.GetFullPath(GroupResponseExecutable);
            if (string.IsNullOrWhiteSpace(filename))
                return false;

            try
            {
                Process.Start(filename, string.Format(GroupResponseParameters, groupAddress, value));
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

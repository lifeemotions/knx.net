using System;
using System.IO;

namespace KNXLibTests.Support.Os
{
    public class Tools
    {
        public static bool IsUnix
        {
            get
            {
                int p = (int) Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static char PathSeparator
        {
            get
            {
                return IsUnix ? ':' : ';';
            }
        }

        public static bool ExistsOnPath(string fileName)
        {
            if (GetFullPath(fileName) != null)
                return true;
            return false;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

    }
}

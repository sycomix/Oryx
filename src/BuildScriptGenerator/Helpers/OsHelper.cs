using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Oryx.BuildScriptGenerator.Helpers
{
    internal static class OsHelper
    {
        private const string DebianVersionFilePath = "/etc/debian_version";
        private const string DebianStretchMajorVersion = "9";

        /// <summary>
        /// Determines if the current OS is Debian Stretch.
        /// </summary>
        /// <returns>true if running on Stretch, false otherwise</returns>
        public static bool IsDebianStretch()
        {
            if (!File.Exists(DebianVersionFilePath))
            {
                return false;
            }

            try
            {
                var debianVersion = File.ReadAllText(DebianVersionFilePath);
                return debianVersion.Trim().StartsWith(DebianStretchMajorVersion);
            }
            catch
            {
            }

            return false;
        }
    }
}

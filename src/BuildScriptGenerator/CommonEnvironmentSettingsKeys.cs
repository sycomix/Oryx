// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

namespace Microsoft.Oryx.BuildScriptGenerator
{
    public static class CommonEnvironmentSettingsKeys
    {
        // Note: The following two constants exist so that we do not break
        // existing users who might still be using them
        public const string PreBuildScriptPath = "PRE_BUILD_SCRIPT_PATH";
        public const string PostBuildScriptPath = "POST_BUILD_SCRIPT_PATH";

        /// <summary>
        /// Represents an line script or a path to a file
        /// </summary>
        public const string PreBuildCommand = "PRE_BUILD_COMMAND";

        /// <summary>
        /// Represents an line script or a path to a file
        /// </summary>
        public const string PostBuildCommand = "POST_BUILD_COMMAND";
    }
}

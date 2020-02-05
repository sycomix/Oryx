// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

namespace Microsoft.Oryx.BuildScriptGenerator.DotNetCore
{
    public static class DotNetCoreEnvironmentSettingsKeys
    {
        public const string Project = "PROJECT";

        /// <summary>
        /// Represents the 'Configuration' switch of a build, for example: dotnet build --configuration Release
        /// </summary>
        public const string MSBuildConfiguration = "MSBUILD_CONFIGURATION";

        public const string DotNetCoreDefaultVersion = "ORYX_DOTNETCORE_DEFAULT_VERSION";

        public const string DotNetCoreSupportedVersions = "DOTNETCORE_SUPPORTED_VERSIONS";
    }
}

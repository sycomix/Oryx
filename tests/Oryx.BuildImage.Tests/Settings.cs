// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Microsoft.Oryx.BuildImage.Tests
{
    internal static class Settings
    {
        public const string BuildImageName = "mcr.microsoft.com/oryx/build:20191015.2";
        public const string SlimBuildImageName = "mcr.microsoft.com/oryx/build:slim-20191015.2";
        public const string ProdBuildImageName = "mcr.microsoft.com/oryx/build:20191015.2";
        public const string ProdSlimBuildImageName = "mcr.microsoft.com/oryx/build:slim-20191015.2";

        public const string OryxVersion = "0.2.";

        public const string Python27Version = "2.7.16";

        public const string MySqlDbImageName = "mysql/mysql-server:5.7";
        public const string PostgresDbImageName = "postgres";

        public static readonly OSPlatform LinuxOS = OSPlatform.Create("LINUX");
    }
}
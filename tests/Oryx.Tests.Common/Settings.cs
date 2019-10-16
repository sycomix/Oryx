// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

namespace Microsoft.Oryx.Tests.Common
{
    public class Settings
    {
        public const string BuildImageName = "mcr.microsoft.com/oryx/build:20191015.2";
        public const string SlimBuildImageName = "mcr.microsoft.com/oryx/build:slim-20191015.2";
        public const string PackImageName  = "mcr.microsoft.com/oryx/pack:20191015.2";
        
        public const string RemoveTestContainersEnvironmentVariableName = "ORYX_REMOVE_TEST_CONTAINERS";

        public const string MySqlDbImageName = "mysql/mysql-server:5.7";
        public const string PostgresDbImageName = "postgres:alpine";
    }
}
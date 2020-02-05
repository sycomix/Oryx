// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.IO;
using Microsoft.Oryx.Tests.Common;
using Xunit;

namespace Microsoft.Oryx.BuildScriptGenerator.Tests
{
    public class VersionProviderHelperTest : IClassFixture<TestTempDirTestFixture>
    {
        private readonly string _tempDirRoot;

        public VersionProviderHelperTest(TestTempDirTestFixture tempDirFixture)
        {
            _tempDirRoot = tempDirFixture.RootDirPath;
        }

        [Fact]
        public void GetVersionsFromDirectory_IgnoresMalformedVersionStrings()
        {
            // Arrange
            var expectedVersion = "1.0.0";
            CreateSubDirectory(expectedVersion);
            CreateSubDirectory("2.0b"); // Invalid SemVer string
            var versionProviderHelper = new VersionProviderHelper();

            // Act
            var versions = versionProviderHelper.GetVersionsFromDirectory(_tempDirRoot);

            // Assert
            Assert.Single(versions, expectedVersion);
        }

        private void CreateSubDirectory(string name)
        {
            Directory.CreateDirectory(Path.Combine(_tempDirRoot, name));
        }
    }
}

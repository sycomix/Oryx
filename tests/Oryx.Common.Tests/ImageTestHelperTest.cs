// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using Microsoft.Oryx.Tests.Common;
using Xunit;

namespace Microsoft.Oryx.Common.Tests
{
    public class ImageTestHelperTest
    {
        [Theory]
        [InlineData("", "", ImageTestHelper.DefaultRepoPrefix + "/oryx/node:10.10")]
        [InlineData("", "20191008.1", ImageTestHelper.DefaultRepoPrefix + "/oryx/node:10.10-20191008.1")]
        [InlineData("mcr.microsoft.com", "", "mcr.microsoft.com/oryx/node:10.10")]
        [InlineData("mcr.microsoft.com", "20191008.1", "mcr.microsoft.com/oryx/node:10.10-20191008.1")]
        public void GetRuntimeImage(string repoPrefix, string tagSuffix, string expectedImageName)
        {
            // Arrange
            var platformName = "node";
            var platformVersion = "10.10";
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetRuntimeImage(platformName, platformVersion);

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }

        [Theory]
        [InlineData("", "", "oryxtests/build:latest")]
        [InlineData("", "20191008.1", "oryxtests/build:latest")]
        [InlineData("mcr.microsoft.com", "", "oryxtests/build:latest")]
        [InlineData("mcr.microsoft.com", "20191008.1", "oryxtests/build:latest")]
        public void GetBuildImage_ByDefaultReturnsImage_WithUnelevatedPrivileges(
            string repoPrefix,
            string tagSuffix,
            string expectedImageName)
        {
            // Arrange
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetBuildImage();

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }

        [Theory]
        [InlineData("", "", ImageTestHelper.DefaultRepoPrefix + "/oryx/build:latest")]
        [InlineData("", "20191008.1", ImageTestHelper.DefaultRepoPrefix + "/oryx/build:20191008.1")]
        [InlineData("mcr.microsoft.com", "", "mcr.microsoft.com/oryx/build:latest")]
        [InlineData("mcr.microsoft.com", "20191008.1", "mcr.microsoft.com/oryx/build:20191008.1")]
        public void GetBuildImage_ReturnsImageName_WithElevatedPrivileges(
            string repoPrefix,
            string tagSuffix,
            string expectedImageName)
        {
            // Arrange
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetBuildImage(withElevatedPrivileges: true);

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }

        [Theory]
        [InlineData("", "", "oryxtests/build:slim")]
        [InlineData("", "20191008.1", "oryxtests/build:slim")]
        [InlineData("mcr.microsoft.com", "", "oryxtests/build:slim")]
        [InlineData("mcr.microsoft.com", "20191008.1", "oryxtests/build:slim")]
        public void GetSlimBuildImage_ByDefaultReturnsImage_WithUnelevatedPrivileges(
            string repoPrefix,
            string tagSuffix,
            string expectedImageName)
        {
            // Arrange
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetSlimBuildImage();

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }

        [Theory]
        [InlineData("", "", ImageTestHelper.DefaultRepoPrefix + "/oryx/build:slim")]
        [InlineData("", "20191008.1", ImageTestHelper.DefaultRepoPrefix + "/oryx/build:slim-20191008.1")]
        [InlineData("mcr.microsoft.com", "", "mcr.microsoft.com/oryx/build:slim")]
        [InlineData("mcr.microsoft.com", "20191008.1", "mcr.microsoft.com/oryx/build:slim-20191008.1")]
        public void GetSlimBuildImage_ReturnsImageName_WithElevatedPrivileges(string repoPrefix, string tagSuffix, string expectedImageName)
        {
            // Arrange
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetSlimBuildImage(withElevatedPrivileges: true);

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }

        [Theory]
        [InlineData("", "", ImageTestHelper.DefaultRepoPrefix + "/oryx/pack:latest")]
        [InlineData("", "20191008.1", ImageTestHelper.DefaultRepoPrefix + "/oryx/pack:20191008.1")]
        [InlineData("mcr.microsoft.com", "", "mcr.microsoft.com/oryx/pack:latest")]
        [InlineData("mcr.microsoft.com", "20191008.1", "mcr.microsoft.com/oryx/pack:20191008.1")]
        public void GetPackImage(string repoPrefix, string tagSuffix, string expectedImageName)
        {
            // Arrange
            var imageHelper = new ImageTestHelper(repoPrefix, tagSuffix);

            // Act
            var runtimeImage = imageHelper.GetPackImage();

            // Assert
            Assert.Equal(expectedImageName, runtimeImage);
        }
    }
}

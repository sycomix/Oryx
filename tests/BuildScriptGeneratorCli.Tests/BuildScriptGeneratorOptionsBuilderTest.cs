// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.IO;
using Microsoft.Oryx.BuildScriptGenerator.Exceptions;
using Microsoft.Oryx.Tests.Common;
using Xunit;

namespace Microsoft.Oryx.BuildScriptGeneratorCli.Tests
{
    public class BuildScriptGeneratorOptionsBuilderTest : IClassFixture<TestTempDirTestFixture>
    {
        private readonly TestTempDirTestFixture _testRootDir;

        public BuildScriptGeneratorOptionsBuilderTest(TestTempDirTestFixture testFixture)
        {
            _testRootDir = testFixture;
        }

        [Fact]
        public void ResolvesToCurrentDirectoryAbsolutePath_WhenDotNotationIsUsed()
        {
            // Arrange
            var currentDir = Directory.GetCurrentDirectory();

            // Act
            var options = new BuildScriptGeneratorOptionsBuilder()
                .WithSourceDir(".")
                .WithDestinationDir(".")
                .WithIntermediateDir(".")
                .Build();

            // Assert
            Assert.Equal(currentDir, options.SourceDir);
            Assert.Equal(currentDir, options.DestinationDir);
            Assert.Equal(currentDir, options.IntermediateDir);
        }

        [Theory]
        [InlineData("dir1")]
        [InlineData("dir1", "dir2")]
        public void ResolvesToAbsolutePath_WhenRelativePathIsGiven(params string[] paths)
        {
            // Arrange
            var providedPath = Path.Combine(paths);
            var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), providedPath);

            // Act
            var options = new BuildScriptGeneratorOptionsBuilder()
                .WithSourceDir(providedPath)
                .WithDestinationDir(providedPath)
                .WithIntermediateDir(providedPath)
                .Build();

            // Assert
            Assert.Equal(absolutePath, options.SourceDir);
            Assert.Equal(absolutePath, options.DestinationDir);
            Assert.Equal(absolutePath, options.IntermediateDir);
        }

        [Fact]
        public void ResolvesToAbsolutePath_WhenAbsolutePathIsGiven()
        {
            // Arrange
            var absolutePath = Path.GetTempPath();

            // Act
            var options = new BuildScriptGeneratorOptionsBuilder()
                .WithSourceDir(absolutePath)
                .WithDestinationDir(absolutePath)
                .WithIntermediateDir(absolutePath)
                .Build();

            // Assert
            Assert.Equal(absolutePath, options.SourceDir);
            Assert.Equal(absolutePath, options.DestinationDir);
            Assert.Equal(absolutePath, options.IntermediateDir);
        }

        [Fact]
        public void ResolvesToAbsolutePath_WhenDoubleDotNotationIsUsed_RelativeToCurrentDir()
        {
            // Arrange
            var currentDir = Directory.GetCurrentDirectory();
            var expected = new DirectoryInfo(currentDir).Parent.FullName;

            // Act
            var options = new BuildScriptGeneratorOptionsBuilder()
                .WithSourceDir("..")
                .WithDestinationDir("..")
                .WithIntermediateDir("..")
                .Build();

            // Assert
            Assert.Equal(expected, options.SourceDir);
            Assert.Equal(expected, options.DestinationDir);
            Assert.Equal(expected, options.IntermediateDir);
        }

        [Fact]
        public void ResolvesToAbsolutePath_WhenDoubleDotNotationIsUsed()
        {
            // Arrange
            var dir1 = _testRootDir.CreateChildDir();
            var dir2 = Directory.CreateDirectory(Path.Combine(dir1, "subDir1")).FullName;
            var expected = Directory.CreateDirectory(Path.Combine(dir1, "subDir2")).FullName;
            var relativePath = Path.Combine(dir2, "..", "subDir2");

            // Act
            var options = new BuildScriptGeneratorOptionsBuilder()
                .WithSourceDir(relativePath)
                .WithDestinationDir(relativePath)
                .WithIntermediateDir(relativePath)
                .Build();

            // Assert
            Assert.Equal(expected, options.SourceDir);
            Assert.Equal(expected, options.DestinationDir);
            Assert.Equal(expected, options.IntermediateDir);
        }

        [Theory]
        [InlineData("=")]
        [InlineData("==")]
        [InlineData("=true")]
        public void ProcessProperties_Throws_WhenKeyIsNotPresent(string property)
        {
            // Arrange
            var properties = new[] { property };

            // Act & Assert
            var exception = Assert.Throws<InvalidUsageException>(
                () => BuildScriptGeneratorOptionsBuilder.ProcessProperties(properties));

            Assert.Equal($"Property key cannot start with '=' for property '{property}'.", exception.Message);
        }

        [Theory]
        [InlineData("a=\"b c\"", "a", "b c")]
        [InlineData("a=\"b \"", "a", "b ")]
        [InlineData("a=\" b\"", "a", " b")]
        [InlineData("a=\" b \"", "a", " b ")]
        [InlineData("\"a b\"=d", "a b", "d")]
        [InlineData("\"a \"=d", "a ", "d")]
        [InlineData("\" a\"=d", " a", "d")]
        public void ProcessProperties_ReturnsProperty_TrimmingTheQuotes(
            string property,
            string key,
            string value)
        {
            // Arrange
            var properties = new[] { property };

            // Act
            var actual = BuildScriptGeneratorOptionsBuilder.ProcessProperties(properties);

            // Assert
            Assert.Collection(
                actual,
                (kvp) => { Assert.Equal(key, kvp.Key); Assert.Equal(value, kvp.Value); });
        }
    }
}

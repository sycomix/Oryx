// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using JetBrains.Annotations;
using Microsoft.Oryx.Common;
using Microsoft.Oryx.Tests.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Oryx.BuildImage.Tests.Node
{
    /// <summary>
    /// Tests that the Node platform can build and package the VS Code dependencies.
    /// </summary>
    public class NodeVsCodeDependencies : NodeJSSampleAppsTestBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public NodeVsCodeDependencies(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<object[]> SomeVSCodeDependencies => new object[][]
        {
            new object[] { "applicationinsights", "1.0.8", null },
            new object[] { "iconv-lite", "0.5.0", null },
            new object[] { "keytar", "4.11.0", null, new[] { "libsecret-1-dev" } },
            new object[] { "native-keymap", "2.0.0", null, new[] { "libx11-dev", "libxkbfile-dev" } },

            new object[] { "@microsoft/applicationinsights-web", "2.1.1", null },
            new object[] { "graceful-fs", "4.1.11", null },
            new object[] { "http-proxy-agent", "2.1.0", null },
            new object[] { "https-proxy-agent", "2.2.1", null },
            new object[] { "jschardet", "1.6.0", "28152dd8db5904dc2cf9aa12ef4f8783f713e79a" },
            new object[] { "native-is-elevated", "0.3.0", null },
            new object[] { "native-watchdog", "1.0.0", null },
            new object[] { "node-pty", "0.9.0-beta19", null },
            new object[] { "nsfw", "1.2.5", null },
            new object[] { "onigasm-umd", "2.2.2", null },
            new object[] { "semver-umd", "5.5.3", null },
            new object[] { "spdlog", "0.9.0", null },
            new object[] { "sudo-prompt", "9.0.0", null },
            new object[] { "v8-inspect-profiler", "0.0.20", null },
            new object[] { "vscode-chokidar", "2.1.7", null },
            new object[] { "vscode-minimist", "1.2.1", null },
            new object[] { "vscode-proxy-agent", "0.4.0", null },
            new object[] { "vscode-ripgrep", "1.5.6", null },
            new object[] { "vscode-sqlite3", "4.0.8", null },
            new object[] { "vscode-textmate", "4.2.2", null },
            new object[] { "xterm", "3.15.0-beta99", null },
            new object[] { "xterm-addon-search", "0.2.0-beta3", null },
            new object[] { "xterm-addon-web-links", "0.1.0-beta10", null },
            new object[] { "yauzl", "2.9.2", null },
            new object[] { "yazl", "2.4.3", null },
        };

        private readonly string[] IgnoredTarEntries = new[] { "package/.npmignore", "package", "package/yarn.lock" };

        [Theory]
        [MemberData(nameof(SomeVSCodeDependencies))]
        public void CanBuildNpmPackages(
            string pkgName,
            string pkgVersion,
            string altCommitId,
            string[] requiredOsPackages = null)
        {
            const string tarListCmd = "tar -tvf";
            const string npmTarPath = "/tmp/npm-pkg.tgz";
            const string tarListMarker = "---TAR---";

            // Arrange
            var pkgSrcDir = "/tmp/pkg/src";
            var pkgBuildOutputDir = "/tmp/pkg/out";
            var oryxPackOutput = $"{pkgBuildOutputDir}/{pkgName}-{pkgVersion}.tgz";

            (string gitRepoUrl, string commitId) = GetGitInfoFromNpmRegistry(pkgName, pkgVersion);
            Assert.False(string.IsNullOrEmpty(gitRepoUrl), "Could not find a repository address for package");
            Assert.False(string.IsNullOrEmpty(commitId),   "Could not find a commit ID for package");

            var osReqsParam = string.Empty;
            if (requiredOsPackages != null)
            {
                osReqsParam = $"--os-requirements {string.Join(',', requiredOsPackages)}";
            }

            var script = new ShellScriptBuilder()
            // Fetch source code
                .AddCommand($"mkdir -p {pkgSrcDir} && git clone {gitRepoUrl} {pkgSrcDir}")
                .AddCommand($"cd {pkgSrcDir} && git checkout {commitId}")
            // Build & package
                .AddBuildCommand($"{pkgSrcDir} --package -o {pkgBuildOutputDir} {osReqsParam}") // Should create a file <name>-<version>.tgz
                .AddFileExistsCheck(oryxPackOutput)
            // Compute diff between tar contents
                // Download public NPM build for comparison
                    .AddCommand($"export NpmTarUrl=$(npm view {pkgName}@{pkgVersion} dist.tarball)")
                    .AddCommand($"wget -O {npmTarPath} $NpmTarUrl")
                // Print tar content lists
                    .AddCommand("echo " + tarListMarker)
                    .AddCommand($"{tarListCmd} {oryxPackOutput}")
                    .AddCommand("echo " + tarListMarker)
                    .AddCommand($"{tarListCmd} {npmTarPath}")
                .ToString();

            // Act
            // Not using Settings.BuildImageName on purpose - so that apt-get can run as root
            var result = _dockerCli.Run("oryxdevmcr.azurecr.io/public/oryx/build:latest", "/bin/bash", new[] { "-c", script });

            // Assert contained file names
            var tarLists = result.StdOut.Split(tarListMarker);

            var (oryxTarList, oryxTarSize) = ParseTarList(tarLists[1]);
            var (npmTarList,  npmTarSize)  = ParseTarList(tarLists[2]);
            Assert.Equal(npmTarList, oryxTarList);

            // Assert tar file sizes
            var tarSizeDiff = Math.Abs(npmTarSize - oryxTarSize);
            Assert.True(tarSizeDiff < npmTarSize * 0.1, // Accepting differences of less than 10% of the official artifact size
                $"Size difference is too big. Oryx build: {oryxTarSize}, NPM build: {npmTarSize}");
        }

        private (IEnumerable<string>, int) ParseTarList(string rawTarList)
        {
            var fileEntries = rawTarList.Trim().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Where(line => !line.StartsWith('d')) // Filter out directories
                .Select(line => line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries))
                .Select(cols => (Size: int.Parse(cols[2]), Name: cols.Last())) // Keep only the size and the name
                .Where(entry => !IgnoredTarEntries.Contains(entry.Name))
                .OrderBy(entry => entry.Name);

            return (fileEntries.Select(entry => entry.Name), fileEntries.Sum(entry => entry.Size));
        }

        [CanBeNull]
        private (string, string) GetGitInfoFromNpmRegistry(string name, string version)
        {
            var packageJson = JsonConvert.DeserializeObject<dynamic>(
                _httpClient.GetStringAsync($"http://registry.npmjs.org/{name}/{version}").Result);
            var gitRepoNode = packageJson?.repository?.url;
            var gitHeadNode = packageJson?.gitHead as Newtonsoft.Json.Linq.JValue;
            return (gitRepoNode?.ToString().Replace("+https", ""), gitHeadNode?.ToString());
        }
    }
}

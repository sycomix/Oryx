using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Oryx.Common.Utilities;
using Xunit;
using Xunit.Abstractions;
using System.Diagnostics;
using Oryx.Tests.Common;

namespace Oryx.Integration.Tests.LocalDockerTests
{
    enum BuildpackPhase { DETECTING, ANALYZING, BUILDING, EXPORTING }

    class BuildpackStateMachine
    {
        private const string PhaseHeaderPrefix = "===> ";

        BuildpackPhase _currentPhase;

        public BuildpackPhase Phase { get => _currentPhase; }

        /// <summary>
        /// Updates the current state based on a given output line from pack.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>true if the state changed, false otherwise</returns>
        public bool UpdatePhaseFromPackOutputLine(string line)
        {
            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith(PhaseHeaderPrefix)) // State has changed!
            {
                string phaseTitle = line.Substring(PhaseHeaderPrefix.Length);
                switch (phaseTitle)
                {
                    case "DETECTING":
                        _currentPhase = BuildpackPhase.DETECTING;
                        return true;
                    case "ANALYZING":
                        _currentPhase = BuildpackPhase.ANALYZING;
                        return true;
                    case "BUILDING":
                        _currentPhase = BuildpackPhase.BUILDING;
                        return true;
                    case "EXPORTING":
                        _currentPhase = BuildpackPhase.EXPORTING;
                        return true;
                    default:
                        throw new Exception("Unexpected phase title");
                }
            }
            return false;
        }
    }

    public class Benchmarks
    {
        private readonly ITestOutputHelper _output;
        private readonly string _hostSamplesDir;
        private readonly DockerCli _dockerCli;
        private readonly HttpClient _httpClient;

        public Benchmarks(ITestOutputHelper output)
        {
            _output = output;
            _hostSamplesDir = Path.Combine(Directory.GetCurrentDirectory(), "SampleApps");
            _dockerCli = new DockerCli();
            _httpClient = new HttpClient();
        }

        [Theory]
        [InlineData("nodejs", "websocket")]
        public async Task CompareBuildTimes(string platform, string sampleApp)
        {
            // Arrange
            var hostDir = Path.Combine(_hostSamplesDir, platform, sampleApp);

            // Build & measure on Buildpacks
            var pack = new BuildpackStateMachine();
            var buildStopwatch = new Stopwatch();
            
            int packExitCode = ProcessHelper.RunProcess(
                @"C:\Users\dorfire\Src\buildpacks\pack.exe",
                new string[] { "build", "--no-color", "--path", ".", "testapp" },
                hostDir,
                (sender, args) =>
                {
                    _output.WriteLine("pack> {0}", args.Data);

                    if (pack.UpdatePhaseFromPackOutputLine(args.Data))
                    {
                        if (pack.Phase == BuildpackPhase.DETECTING) // If the state just changed to DETECTING, start the clock
                        {
                            buildStopwatch.Start();
                        }
                        else if (buildStopwatch.IsRunning && pack.Phase == BuildpackPhase.EXPORTING)
                        {
                            buildStopwatch.Stop();
                        }
                    }
                },
                (sender, args) =>
                {
                    _output.WriteLine("pack> {0}", args.Data);
                },
                TimeSpan.FromMinutes(3));

            Assert.Equal(0, packExitCode);
            Console.WriteLine("Build time on Buildpacks: {0} s", buildStopwatch.Elapsed.TotalSeconds);

            buildStopwatch.Reset();

            // Build & measure on Oryx
            var appVolume = DockerVolume.Create(hostDir);

            buildStopwatch.Start();
            var buildAppResult = _dockerCli.Run(
                Settings.OryxBuildImageName,
                appVolume,
                commandToExecuteOnRun: "oryx",
                commandArguments: new[] { "build", appVolume.ContainerDir /* "-l", "nodejs", "--language-version", nodeVersion */ });

            await EndToEndTestHelper.RunAssertsAsync(
                _output,
                () =>
                {
                    Assert.True(buildAppResult.IsSuccess);
                    return Task.CompletedTask;
                },
                buildAppResult.GetDebugInfo());

            buildStopwatch.Stop();
            Console.WriteLine("Build time on Oryx: {0} s", buildStopwatch.Elapsed.TotalSeconds);
        }
    }
}

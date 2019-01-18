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
        private readonly DockerCli _dockerCli = new DockerCli();
        private readonly Random _rand = new Random();
        //private readonly HttpClient _httpClient = new HttpClient();

        public Benchmarks(ITestOutputHelper output)
        {
            _output = output;
            _hostSamplesDir = Path.Combine(Directory.GetCurrentDirectory(), "SampleApps");
        }

        private string GetRandomAppName()
        {
            return "testapp" + _rand.Next(99999).ToString();
        }

        private void WriteTitleLine(string title)
        {
            _output.WriteLine(Environment.NewLine);
            title = title + " ";
            _output.WriteLine(title.PadRight(120, '-'));
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

            string[] packArgs = new[] { "build", "--no-color", "--path", ".", GetRandomAppName() }; // App name has to be random to avoid re-use of a previously built image

            WriteTitleLine(string.Format("pack {0}", string.Join(' ', packArgs)));

            int packExitCode = ProcessHelper.RunProcess(
                @"C:\Users\dorfire\Src\buildpacks\pack.exe",
                packArgs,
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
            double packSecs = buildStopwatch.Elapsed.TotalSeconds;
            buildStopwatch.Reset();

            // Build & measure on Oryx
            var appVolume = DockerVolume.Create(hostDir);

            buildStopwatch.Start();
            string[] oryxArgs = new[] { "build", appVolume.ContainerDir, "-o", "/tmp/app" /* "-l", "nodejs", "--language-version", nodeVersion */ };
            var buildAppResult = _dockerCli.Run(Settings.OryxBuildImageName, appVolume, commandToExecuteOnRun: "oryx", commandArguments: oryxArgs);
            
            await EndToEndTestHelper.RunAssertsAsync(
                _output,
                () =>
                {
                    Assert.True(buildAppResult.IsSuccess);
                    return Task.CompletedTask;
                },
                buildAppResult.GetDebugInfo());

            buildStopwatch.Stop();
            double oryxSecs = buildStopwatch.Elapsed.TotalSeconds;
            
            WriteTitleLine(string.Format("oryx {0}", string.Join(' ', oryxArgs)));
            foreach (string line in buildAppResult.Output.Split('\n'))
            {
                _output.WriteLine("oryx> {0}", line.Trim());
            }
            
            WriteTitleLine("Summary");
            _output.WriteLine("Build time on Buildpacks: {0} s", packSecs);
            _output.WriteLine("Build time on Oryx: {0} s", oryxSecs);
        }
    }
}

// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Oryx.BuildScriptGenerator;
using Microsoft.Oryx.Common;

namespace Microsoft.Oryx.BuildScriptGeneratorCli
{
    [Command(Name, Description = "Install the required runtime components for a platform.")]
    internal class InstallRunTimeCommand : CommandBase
    {
        public const string Name = "install-runtime";

        [Option(
            OptionTemplates.Platform,
            CommandOptionType.SingleValue,
            Description = "The name of the platform for which the runtime components should be installed.")]
        public string Platform { get; set; }

        [Option(
            OptionTemplates.PlatformVersion,
            CommandOptionType.SingleValue,
            Description = "The version of the platform for which the runtime components should be installed.")]
        public string PlatformVersion { get; set; }

        internal override int Execute(IServiceProvider serviceProvider, IConsole console)
        {
            var scriptGenerator = serviceProvider.GetRequiredService<IRunTimeInstallationScriptGenerator>();
            var logger = serviceProvider.GetRequiredService<ILogger<InstallRunTimeCommand>>();
            var options = new RunTimeInstallationScriptGeneratorOptions { PlatformVersion = PlatformVersion };

            var scriptContent = scriptGenerator.GenerateBashScript(Platform, options);
            if (string.IsNullOrEmpty(scriptContent))
            {
                console.WriteErrorLine("Couldn't generate startup script.");
                return ProcessConstants.ExitFailure;
            }

            // Get the path where the generated script should be written into.
            var tempDirProvider = serviceProvider.GetRequiredService<ITempDirectoryProvider>();
            var scriptPath = Path.Combine(tempDirProvider.GetTempDirectory(), "install.sh");
            File.WriteAllText(scriptPath, scriptContent);

            // Run the generated script
            int exitCode;
            using (var timedEvent = logger.LogTimedEvent("RunInstallationScript"))
            {
                exitCode = serviceProvider.GetRequiredService<IScriptExecutor>().ExecuteScript(
                    scriptPath,
                    null,
                    null,
                    (sender, args) => { if (args.Data != null) console.WriteLine(args.Data); },
                    (sender, args) => { if (args.Data != null) console.Error.WriteLine(args.Data); });
                timedEvent.AddProperty("exitCode", exitCode.ToString());
            }

            return ProcessConstants.ExitSuccess;
        }

        internal override bool IsValidInput(IServiceProvider serviceProvider, IConsole console)
        {
            // TODO: validate?
            return true;
        }
    }
}

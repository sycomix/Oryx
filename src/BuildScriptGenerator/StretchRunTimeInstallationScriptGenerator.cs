// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Oryx.BuildScriptGenerator.Exceptions;
using Microsoft.Oryx.Common.Extensions;

namespace Microsoft.Oryx.BuildScriptGenerator
{
    public class StretchRunTimeInstallationScriptGenerator : IRunTimeInstallationScriptGenerator
    {
        private const string DebianVersionFilePath = "/etc/debian_version";
        private const string DebianStretchMajorVersion = "9";

        private readonly IEnumerable<IProgrammingPlatform> _programmingPlatforms;
        private readonly ILogger<StretchRunTimeInstallationScriptGenerator> _logger;

        public StretchRunTimeInstallationScriptGenerator(
            IEnumerable<IProgrammingPlatform> programmingPlatforms,
            ILogger<StretchRunTimeInstallationScriptGenerator> logger)
        {
            _programmingPlatforms = programmingPlatforms;
            _logger = logger;
        }

        public bool IsCompatibleWithCurrentOs()
        {
            if (!File.Exists(DebianVersionFilePath))
            {
                return false;
            }

            try
            {
                var debianVersion = File.ReadAllText(DebianVersionFilePath);
                return debianVersion.Trim().StartsWith(DebianStretchMajorVersion);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Exception caught while trying to open Debian version file");
            }

            return false;
        }

        public string GenerateBashScript(string targetPlatformName, RunTimeInstallationScriptGeneratorOptions opts)
        {
            var targetPlatform = _programmingPlatforms
                .Where(p => p.Name.EqualsIgnoreCase(targetPlatformName))
                .FirstOrDefault();

            if (targetPlatform == null)
            {
                throw new UnsupportedLanguageException($"Platform '{targetPlatformName}' is not supported.");
            }

            if (!targetPlatform.SupportedVersions.Contains(opts.PlatformVersion))
            {
                throw new UnsupportedVersionException(
                    targetPlatformName,
                    opts.PlatformVersion,
                    targetPlatform.SupportedVersions);
            }

            var runScript = targetPlatform.GenerateBashRunTimeInstallationScript(opts);
            return runScript;
        }
    }
}
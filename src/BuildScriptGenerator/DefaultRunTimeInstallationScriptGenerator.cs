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
    public class DefaultRunTimeInstallationScriptGenerator : IRunTimeInstallationScriptGenerator
    {
        private const string DebianVersionFilePath = "/etc/debian_version";
        private const string DebianStretchMajorVersion = "9";

        private readonly IEnumerable<IProgrammingPlatform> _programmingPlatforms;

        public DefaultRunTimeInstallationScriptGenerator(
            IEnumerable<IProgrammingPlatform> programmingPlatforms)
        {
            _programmingPlatforms = programmingPlatforms;
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

            return targetPlatform.GenerateBashRunTimeInstallationScript(opts);
        }
    }
}
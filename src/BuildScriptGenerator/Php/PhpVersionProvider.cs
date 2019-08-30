// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Microsoft.Oryx.BuildScriptGenerator.Php
{
    internal class PhpVersionProvider : IPhpVersionProvider
    {
        private readonly PhpScriptGeneratorOptions _opts;
        private IEnumerable<string> _supportedPhpVersions;

        public PhpVersionProvider(IOptions<PhpScriptGeneratorOptions> options)
        {
            _opts = options.Value;
        }

        public IEnumerable<string> SupportedPhpVersions
        {
            get
            {
                if (_supportedPhpVersions == null)
                {
                    var supportedVersions = new List<string>();
                    var versions = VersionProviderHelper.GetSupportedVersions(
                        _opts.SupportedPhpVersions,
                        _opts.BuiltInPhpInstallVersionsDir);
                    supportedVersions.AddRange(versions);
                    versions = VersionProviderHelper.GetSupportedVersions(
                        _opts.SupportedPhpVersions,
                        _opts.DynamicPhpInstallVersionsDir);
                    supportedVersions.AddRange(versions);
                    _supportedPhpVersions = supportedVersions;
                }

                return _supportedPhpVersions;
            }
        }
    }
}
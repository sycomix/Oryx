// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Microsoft.Oryx.BuildScriptGenerator.Python
{
    internal class PythonVersionProvider : IPythonVersionProvider
    {
        private readonly PythonScriptGeneratorOptions _options;
        private IEnumerable<string> _supportedPythonVersions;

        public PythonVersionProvider(IOptions<PythonScriptGeneratorOptions> options)
        {
            _options = options.Value;
        }

        public IEnumerable<string> SupportedPythonVersions
        {
            get
            {
                if (_supportedPythonVersions == null)
                {
                    var supportedVersions = new List<string>();
                    var versions = VersionProviderHelper.GetSupportedVersions(
                        _options.SupportedPythonVersions,
                        _options.BuiltInPythonInstallVersionsDir);
                    supportedVersions.AddRange(versions);
                    versions = VersionProviderHelper.GetSupportedVersions(
                        _options.SupportedPythonVersions,
                        _options.DynamicPythonInstallVersionsDir);
                    supportedVersions.AddRange(versions);
                    _supportedPythonVersions = supportedVersions;
                }

                return _supportedPythonVersions;
            }
        }
    }
}
// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Oryx.BuildScriptGenerator.Python
{
    internal class PythonVersionProvider : IPythonVersionProvider
    {
        private readonly VersionProviderHelper _versionProviderHelper;
        private IEnumerable<string> _supportedPythonVersions;

        public PythonVersionProvider(VersionProviderHelper versionProviderHelper)
        {
            _versionProviderHelper = versionProviderHelper;
        }

        public IEnumerable<string> SupportedPythonVersions
        {
            get
            {
                if (_supportedPythonVersions == null)
                {
                    _supportedPythonVersions = _versionProviderHelper.GetVersionsFromDirectory(
                        PythonConstants.InstalledPythonVersionsDir);
                }

                return _supportedPythonVersions;
            }
        }
    }
}
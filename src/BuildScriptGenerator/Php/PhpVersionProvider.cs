// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Oryx.BuildScriptGenerator.Php
{
    internal class PhpVersionProvider : IPhpVersionProvider
    {
        private readonly VersionProviderHelper _versionProviderHelper;
        private IEnumerable<string> _supportedPhpVersions;

        public PhpVersionProvider(VersionProviderHelper versionProviderHelper)
        {
            _versionProviderHelper = versionProviderHelper;
        }

        public IEnumerable<string> SupportedPhpVersions
        {
            get
            {
                if (_supportedPhpVersions == null)
                {
                    _supportedPhpVersions = _versionProviderHelper.GetVersionsFromDirectory(
                        PhpConstants.InstalledPhpVersionsDir);
                }

                return _supportedPhpVersions;
            }
        }
    }
}
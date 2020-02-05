// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Oryx.BuildScriptGenerator.DotNetCore
{
    internal class DotNetCoreVersionProvider : IDotNetCoreVersionProvider
    {
        private readonly VersionProviderHelper _versionProviderHelper;
        private IEnumerable<string> _supportedVersions;

        public DotNetCoreVersionProvider(VersionProviderHelper versionProviderHelper)
        {
            _versionProviderHelper = versionProviderHelper;
        }

        public IEnumerable<string> SupportedDotNetCoreVersions
        {
            get
            {
                if (_supportedVersions == null)
                {
                    _supportedVersions = _versionProviderHelper.GetVersionsFromDirectory(
                        DotNetCoreConstants.InstalledVersionsDir);
                }

                return _supportedVersions;
            }
        }
    }
}
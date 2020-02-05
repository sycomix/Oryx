// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Oryx.BuildScriptGenerator.Node
{
    internal class NodeVersionProvider : INodeVersionProvider
    {
        private readonly VersionProviderHelper _versionProviderHelper;
        private IEnumerable<string> _supportedNodeVersions;
        private IEnumerable<string> _supportedNpmVersions;

        public NodeVersionProvider(VersionProviderHelper versionProviderHelper)
        {
            _versionProviderHelper = versionProviderHelper;
        }

        public IEnumerable<string> SupportedNodeVersions
        {
            get
            {
                if (_supportedNodeVersions == null)
                {
                    _supportedNodeVersions = _versionProviderHelper.GetVersionsFromDirectory(
                        NodeConstants.InstalledNodeVersionsDir);
                }

                return _supportedNodeVersions;
            }
        }

        public IEnumerable<string> SupportedNpmVersions
        {
            get
            {
                if (_supportedNpmVersions == null)
                {
                    _supportedNpmVersions = _versionProviderHelper.GetVersionsFromDirectory(
                        NodeConstants.InstalledNpmVersionsDir);
                }

                return _supportedNpmVersions;
            }
        }
    }
}
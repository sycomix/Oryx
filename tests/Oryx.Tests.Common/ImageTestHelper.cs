// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;

namespace Microsoft.Oryx.Tests.Common
{
    /// <summary>
    /// Helper class for operations involving images in Oryx test projects.
    /// </summary>
    public class ImageTestHelper
    {
        public const string DefaultRepoPrefix = "oryxdevmcr.azurecr.io/public";

        private const string _imageBaseEnvironmentVariable = "ORYX_TEST_IMAGE_BASE";
        private const string _tagSuffixEnvironmentVariable = "ORYX_TEST_TAG_SUFFIX";

        private const string _buildRepository = "build";
        private const string _packRepository = "pack";
        private const string _latestTag = "latest";
        private const string _slimTag = "slim";

        private string _repoPrefix;
        private string _tagSuffix;

        public ImageTestHelper()
        {
            _repoPrefix = Environment.GetEnvironmentVariable(_imageBaseEnvironmentVariable);
            if (string.IsNullOrEmpty(_repoPrefix))
            {
                // If the ORYX_TEST_IMAGE_BASE environment variable was not set in the .sh script calling this test,
                // then use the default value of 'oryxdevmcr.azurecr.io/public/oryx' as the image base for the tests.
                // This should be used in cases where a image base should be used for the tests rather than the
                // development registry (e.g., oryxmcr.azurecr.io/public/oryx)
                _repoPrefix = DefaultRepoPrefix;
            }

            _tagSuffix = Environment.GetEnvironmentVariable(_tagSuffixEnvironmentVariable);
            if (string.IsNullOrEmpty(_tagSuffix))
            {
                // If the ORYX_TEST_TAG_SUFFIX environment variable was not set in the .sh script calling this test,
                // then don't append a suffix to the tag of this image. This should be used in cases where a specific
                // runtime version tag should be used (e.g., node:8.8-20191025.1 instead of node:8.8)
                _tagSuffix = string.Empty;
            }
            _tagSuffix = _tagSuffix.TrimStart('-');
        }

        /// <summary>
        /// NOTE: This constructor should only be used for ImageTestHelper unit tests.
        /// </summary>
        /// <param name="output">XUnit output helper for logging.</param>
        /// <param name="repoPrefix">The image base used to mimic the ORYX_TEST_IMAGE_BASE environment variable.</param>
        /// <param name="tagSuffix">The tag suffix used to mimic the ORYX_TEST_TAG_SUFFIX environment variable.</param>
        public ImageTestHelper(string repoPrefix, string tagSuffix)
        {
            if (string.IsNullOrEmpty(repoPrefix))
            {
                repoPrefix = DefaultRepoPrefix;
            }
            _repoPrefix = repoPrefix;

            if (string.IsNullOrEmpty(tagSuffix))
            {
                tagSuffix = string.Empty;
            }
            _tagSuffix = tagSuffix.TrimStart('-');
        }

        /// <summary>
        /// Constructs a runtime image from the given parameters that follows the format
        /// '{image}/{platformName}:{platformVersion}{tagSuffix}'. The base image can be set with the environment
        /// variable ORYX_TEST_IMAGE_BASE, otherwise the default base 'oryxdevmcr.azurecr.io/public/oryx' will be used.
        /// If a tag suffix was set with the environment variable ORYX_TEST_TAG_SUFFIX, it will be appended to the tag.
        /// </summary>
        /// <param name="platformName">The platform to pull the runtime image from.</param>
        /// <param name="platformVersion">The version of the platform to pull the runtime image from.</param>
        /// <returns>A runtime image that can be pulled for testing.</returns>
        public string GetRuntimeImage(string platformName, string platformVersion)
        {
            var tagSuffix = GetTag(extendedTag: true);
            return $"{_repoPrefix}/oryx/{platformName}:{platformVersion}{tagSuffix}";
        }

        /// <summary>
        /// Constructs a 'build' image using either the default repo prefix (oryxdevmcr.azurecr.io/public/oryx), or the
        /// repo prefix set by the ORYX_TEST_IMAGE_BASE environment variable. If a tag suffix was set with the environment
        /// variable ORYX_TEST_TAG_SUFFIX, it will be used as the tag, otherwise, the 'latest' tag will be used.
        /// </summary>
        /// <returns>A 'build' image that can be pulled for testing.</returns>
        public string GetBuildImage(bool withElevatedPrivileges = false)
        {
            if (!withElevatedPrivileges)
            {
                return "oryxtests/build:latest";
            }

            var tag = GetTag(extendedTag: false);
            return $"{_repoPrefix}/oryx/build:{tag}";
        }

        /// <summary>
        /// Constructs a 'build:slim' image using either the default repo prefix (oryxdevmcr.azurecr.io/public/oryx), or the
        /// repo prefix set by the ORYX_TEST_IMAGE_BASE environment variable. If a tag suffix was set with the environment
        /// variable ORYX_TEST_TAG_SUFFIX, it will be used as the tag, otherwise, the 'latest' tag will be used.
        /// </summary>
        /// <returns>A 'build:slim' image that can be pulled for testing.</returns>
        public string GetSlimBuildImage(bool withElevatedPrivileges = false)
        {
            if (!withElevatedPrivileges)
            {
                return "oryxtests/build:slim";
            }

            var tagSuffix = GetTag(extendedTag: true);
            return $"{_repoPrefix}/oryx/build:{_slimTag}{tagSuffix}";
        }

        /// <summary>
        /// Constructs a 'pack' image using either the default repo prefix (oryxdevmcr.azurecr.io/public/oryx), or the
        /// repo prefix set by the ORYX_TEST_IMAGE_BASE environment variable. If a tag suffix was set with the environment
        /// variable ORYX_TEST_TAG_SUFFIX, it will be used as the tag, otherwise, the 'latest' tag will be used.
        /// </summary>
        /// <returns>A 'pack' image that can be pulled for testing.</returns>
        public string GetPackImage()
        {
            var tag = GetTag(extendedTag: false);
            return $"{_repoPrefix}/oryx/pack:{tag}";
        }

        private string GetTag(bool extendedTag)
        {
            var tag = string.Empty;
            if (string.IsNullOrEmpty(_tagSuffix))
            {
                tag = extendedTag ? string.Empty : "latest";
            }
            else
            {
                tag = extendedTag ? $"-{_tagSuffix}" : _tagSuffix;
            }
            return tag;
        }
    }
}

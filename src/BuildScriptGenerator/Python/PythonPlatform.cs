// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Oryx.BuildScriptGenerator.Exceptions;

namespace Microsoft.Oryx.BuildScriptGenerator.Python
{
    [BuildProperty(
        VirtualEnvironmentNamePropertyKey,
        "Name of the virtual environment to be created. Defaults to 'pythonenv<Python version>'.")]
    [BuildProperty(
        TargetPackageDirectoryPropertyKey,
        "If provided, packages will be downloaded to the given directory instead of to a virtual environment.")]
    [BuildProperty(
        CompressVirtualEnvPropertyKey,
        "Indicates how and if virtual environment folder should be compressed into a single file in the output " +
        "folder. Options are '" + ZipOption + "', and '" + TarGzOption + "'. Default is to not compress. " +
        "If this option is used, when running the app the virtual environment folder must be extracted from " +
        "this file.")]
    [BuildProperty(
        CompressPackageDirPropertyKey,
        "Indicates how and if the packages directory folder should be compressed into a single file in the output " +
        "folder. Options are '" + ZipOption + "', and '" + TarGzOption + "'. Default is to not compress. " +
        "If this option is used, when running the app the packages directory must be extracted from " +
        "this file.")]
    internal class PythonPlatform : IProgrammingPlatform
    {
        internal const string VirtualEnvironmentNamePropertyKey = "virtualenv_name";
        internal const string TargetPackageDirectoryPropertyKey = "packagedir";

        internal const string CompressVirtualEnvPropertyKey = "compress_virtualenv";
        internal const string CompressPackageDirPropertyKey = "compress_packagedir";
        internal const string ZipOption = "zip";
        internal const string TarGzOption = "tar-gz";

        private readonly PythonScriptGeneratorOptions _pythonScriptGeneratorOptions;
        private readonly IPythonVersionProvider _pythonVersionProvider;
        private readonly IEnvironment _environment;
        private readonly ILogger<PythonPlatform> _logger;
        private readonly PythonLanguageDetector _detector;

        public PythonPlatform(
            IOptions<PythonScriptGeneratorOptions> pythonScriptGeneratorOptions,
            IPythonVersionProvider pythonVersionProvider,
            IEnvironment environment,
            ILogger<PythonPlatform> logger,
            PythonLanguageDetector detector)
        {
            _pythonScriptGeneratorOptions = pythonScriptGeneratorOptions.Value;
            _pythonVersionProvider = pythonVersionProvider;
            _environment = environment;
            _logger = logger;
            _detector = detector;
        }

        public string Name => PythonConstants.PythonName;

        public IEnumerable<string> SupportedLanguageVersions => _pythonVersionProvider.SupportedPythonVersions;

        public LanguageDetectorResult Detect(ISourceRepo sourceRepo)
        {
            return _detector.Detect(sourceRepo);
        }

        public BuildScriptSnippet GenerateBashBuildScriptSnippet(BuildScriptGeneratorContext context)
        {
            var buildProperties = new Dictionary<string, string>();
            var packageDirName = GetPackageDirectory(context);
            var virtualEnvName = GetVirtualEnvironmentName(context);

            if (!string.IsNullOrEmpty(virtualEnvName) && !string.IsNullOrEmpty(packageDirName))
            {
                throw new InvalidUsageException($"Options '{TargetPackageDirectoryPropertyKey}' and " +
                    $"'{VirtualEnvironmentNamePropertyKey}' are mutually exclusive. Please provide " +
                    $"only the target package directory or virtual environment name.");
            }

            var pythonVersion = context.PythonVersion;
            _logger.LogDebug("Selected Python version: {pyVer}", pythonVersion);

            string folderToCompress;
            string compressPropertyKey;
            var virtualEnvModule = string.Empty;
            var virtualEnvCopyParam = string.Empty;
            if (string.IsNullOrEmpty(packageDirName))
            {
                // If the package directory was not provided, we default to virtual envs
                if (string.IsNullOrWhiteSpace(virtualEnvName))
                {
                    virtualEnvName = GetDefaultVirtualEnvName(context);
                }

                buildProperties[PythonConstants.VirtualEnvNameBuildProperty] = virtualEnvName;
                folderToCompress = virtualEnvName;
                compressPropertyKey = CompressVirtualEnvPropertyKey;

                (virtualEnvModule, virtualEnvCopyParam) = GetVirtualEnvModules(pythonVersion);
                _logger.LogDebug(
                    "Using virtual environment {venv}, module {venvModule}",
                    virtualEnvName,
                    virtualEnvModule);
            }
            else
            {
                buildProperties[PythonConstants.PackageDirNameBuildProperty] = packageDirName;
                folderToCompress = packageDirName;
                compressPropertyKey = CompressPackageDirPropertyKey;
            }

            bool enableCollectStatic = IsCollectStaticEnabled();

            string compressCommand = null;
            string compressedFileName = null;
            var isPackaged = GetPackOptions(
                compressPropertyKey,
                context,
                folderToCompress,
                out compressCommand,
                out compressedFileName);

            if (!string.IsNullOrWhiteSpace(virtualEnvName))
            {
                buildProperties[PythonConstants.CompressedVirtualEnvFileBuildProperty] = compressedFileName;
            }
            else
            {
                buildProperties[PythonConstants.CompressedPackageDirFileBuildProperty] = compressedFileName;
            }

            TryLogDependencies(pythonVersion, context.SourceRepo);

            var scriptProps = new PythonBashBuildSnippetProperties(
                virtualEnvironmentName: virtualEnvName,
                virtualEnvironmentModule: virtualEnvModule,
                virtualEnvironmentParameters: virtualEnvCopyParam,
                packagesDirectory: packageDirName,
                disableCollectStatic: !enableCollectStatic,
                compressCommand: compressCommand,
                compressedFileName: compressedFileName);
            string script = TemplateHelpers.Render(
                TemplateHelpers.TemplateResource.PythonSnippet,
                scriptProps,
                _logger);

            return new BuildScriptSnippet()
            {
                BashBuildScriptSnippet = script,
                BuildProperties = buildProperties
            };
        }

        public bool IsCleanRepo(ISourceRepo repo)
        {
            // TODO: support venvs
            return true;
        }

        public string GenerateBashRunScript(RunScriptGeneratorOptions runScriptGeneratorOptions)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(BuildScriptGeneratorContext scriptGeneratorContext)
        {
            return scriptGeneratorContext.EnablePython;
        }

        public bool IsEnabledForMultiPlatformBuild(BuildScriptGeneratorContext scriptGeneratorContext)
        {
            return true;
        }

        public void SetRequiredTools(
            ISourceRepo sourceRepo,
            string targetPlatformVersion,
            [NotNull] IDictionary<string, string> toolsToVersion)
        {
            Debug.Assert(toolsToVersion != null, $"{nameof(toolsToVersion)} must not be null");
            if (!string.IsNullOrWhiteSpace(targetPlatformVersion))
            {
                toolsToVersion[PythonConstants.PythonName] = targetPlatformVersion;
            }
        }

        public void SetVersion(BuildScriptGeneratorContext context, string version)
        {
            context.PythonVersion = version;
        }

        public IEnumerable<string> GetDirectoriesToExcludeFromCopyToBuildOutputDir(BuildScriptGeneratorContext context)
        {
            var dirs = new List<string>();
            var virtualEnvName = GetVirtualEnvironmentName(context);
            var packageDir = GetPackageDirectory(context);

            string folderToCompress;
            string propertyKey;
            if (string.IsNullOrEmpty(virtualEnvName))
            {
                propertyKey = CompressPackageDirPropertyKey;
                folderToCompress = packageDir;
            }
            else
            {
                propertyKey = CompressVirtualEnvPropertyKey;
                folderToCompress = virtualEnvName;
            }

            if (GetPackOptions(
                    propertyKey,
                    context,
                    folderToCompress,
                    out string compressCommand,
                    out string compressedFileName))
            {
                dirs.Add(folderToCompress);
            }
            else if (!string.IsNullOrWhiteSpace(compressedFileName))
            {
                dirs.Add(compressedFileName);
            }

            return dirs;
        }

        public IEnumerable<string> GetDirectoriesToExcludeFromCopyToIntermediateDir(
            BuildScriptGeneratorContext context)
        {
            var excludeDirs = new List<string>();
            var virtualEnvName = GetVirtualEnvironmentName(context);

            if (!string.IsNullOrEmpty(virtualEnvName))
            {
                excludeDirs.Add(virtualEnvName);
                excludeDirs.Add(string.Format(PythonConstants.ZipFileNameFormat, virtualEnvName));
                excludeDirs.Add(string.Format(PythonConstants.TarGzFileNameFormat, virtualEnvName));
            }

            return excludeDirs;
        }

        private static string GetDefaultVirtualEnvName(BuildScriptGeneratorContext context)
        {
            string pythonVersion = context.PythonVersion;
            if (!string.IsNullOrWhiteSpace(pythonVersion))
            {
                var versionSplit = pythonVersion.Split('.');
                if (versionSplit.Length > 1)
                {
                    pythonVersion = $"{versionSplit[0]}.{versionSplit[1]}";
                }
            }

            return $"pythonenv{pythonVersion}";
        }

        private static string GetPackageDirectory(BuildScriptGeneratorContext context)
        {
            string packageDir = null;
            if (context.Properties != null)
            {
                context.Properties.TryGetValue(TargetPackageDirectoryPropertyKey, out packageDir);
            }

            return packageDir;
        }

        private static bool GetPackOptions(
            string compressPropertyKey,
            BuildScriptGeneratorContext context,
            string folderNameToCompress,
            out string compressFolderCommand,
            out string compressedFileName)
        {
            var isFolderPackaged = false;
            compressFolderCommand = null;
            compressedFileName = null;
            if (context.Properties != null &&
                context.Properties.TryGetValue(compressPropertyKey, out string compressOption))
            {
                // default to tar.gz if the property was provided with no value.
                if (string.IsNullOrEmpty(compressOption) ||
                    string.Equals(compressOption, TarGzOption, StringComparison.InvariantCultureIgnoreCase))
                {
                    compressedFileName = string.Format(PythonConstants.TarGzFileNameFormat, folderNameToCompress);
                    compressFolderCommand = $"tar -zcf";
                    isFolderPackaged = true;
                }
                else if (string.Equals(compressOption, ZipOption, StringComparison.InvariantCultureIgnoreCase))
                {
                    compressedFileName = string.Format(PythonConstants.ZipFileNameFormat, folderNameToCompress);
                    compressFolderCommand = $"zip -y -q -r";
                    isFolderPackaged = true;
                }
            }

            return isFolderPackaged;
        }

        private bool IsCollectStaticEnabled()
        {
            // Collect static is enabled by default, but users can opt-out of it
            var enableCollectStatic = true;
            var disableCollectStaticEnvValue = _environment.GetEnvironmentVariable(
                EnvironmentSettingsKeys.DisableCollectStatic);
            if (string.Equals(disableCollectStaticEnvValue, "true", StringComparison.OrdinalIgnoreCase))
            {
                enableCollectStatic = false;
            }

            return enableCollectStatic;
        }

        private (string virtualEnvModule, string virtualEnvCopyParam) GetVirtualEnvModules(string pythonVersion)
        {
            string virtualEnvModule;
            string virtualEnvCopyParam = string.Empty;
            switch (pythonVersion.Split('.')[0])
            {
                case "2":
                    virtualEnvModule = "virtualenv";
                    break;

                case "3":
                    virtualEnvModule = "venv";
                    virtualEnvCopyParam = "--copies";
                    break;

                default:
                    string errorMessage = "Python version '" + pythonVersion + "' is not supported";
                    _logger.LogError(errorMessage);
                    throw new NotSupportedException(errorMessage);
            }

            return (virtualEnvModule, virtualEnvCopyParam);
        }

        private void TryLogDependencies(string pythonVersion, ISourceRepo repo)
        {
            if (!repo.FileExists(PythonConstants.RequirementsFileName))
            {
                return;
            }

            try
            {
                var deps = repo.ReadAllLines(PythonConstants.RequirementsFileName)
                    .Where(line => !line.TrimStart().StartsWith("#"));
                _logger.LogDependencies(PythonConstants.PythonName, pythonVersion, deps);
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Exception caught while logging dependencies");
            }
        }

        private string GetVirtualEnvironmentName(BuildScriptGeneratorContext context)
        {
            if (context.Properties == null ||
                !context.Properties.TryGetValue(VirtualEnvironmentNamePropertyKey, out var virtualEnvName))
            {
                virtualEnvName = string.Empty;
            }

            return virtualEnvName;
        }
    }
}
// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

namespace Microsoft.Oryx.BuildScriptGenerator.Node
{
    /// <summary>
    /// Build script template for NodeJs in Bash.
    /// </summary>
    public class NodeBashBuildSnippetProperties
    {
        public NodeBashBuildSnippetProperties(
            string packageInstallCommand,
            string runBuildCommand,
            string runBuildAzureCommand,
            bool installProductionOnlyDependencies,
            bool copyNodeModulesFolderFromSource,
            string productionOnlyPackageInstallCommand,
            bool zipNodeModulesDir)
        {
            PackageInstallCommand = packageInstallCommand;
            NpmRunBuildCommand = runBuildCommand;
            NpmRunBuildAzureCommand = runBuildAzureCommand;
            InstallProductionOnlyDependencies = installProductionOnlyDependencies;
            CopyNodeModulesFolderFromSource = copyNodeModulesFolderFromSource;
            ProductionOnlyPackageInstallCommand = productionOnlyPackageInstallCommand;
            ZipNodeModulesDir = zipNodeModulesDir;
        }

        public string PackageInstallCommand { get; set; }

        public string NpmRunBuildCommand { get; set; }

        public string NpmRunBuildAzureCommand { get; set; }

        public bool InstallProductionOnlyDependencies { get; set; }

        public bool CopyNodeModulesFolderFromSource { get; set; }

        public string ProductionOnlyPackageInstallCommand { get; set; }

        public bool ZipNodeModulesDir { get; set; }
    }
}
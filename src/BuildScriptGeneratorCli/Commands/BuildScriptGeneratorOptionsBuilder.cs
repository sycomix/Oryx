// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Oryx.BuildScriptGenerator;
using Microsoft.Oryx.BuildScriptGenerator.Exceptions;

namespace Microsoft.Oryx.BuildScriptGeneratorCli
{
    internal class BuildScriptGeneratorOptionsBuilder
    {
        private readonly BuildScriptGeneratorOptions _options;

        public BuildScriptGeneratorOptionsBuilder()
        {
            _options = new BuildScriptGeneratorOptions();
        }

        public BuildScriptGeneratorOptionsBuilder(BuildScriptGeneratorOptions options)
        {
            _options = options;
        }

        public BuildScriptGeneratorOptionsBuilder WithSourceDir(string sourceDir)
        {
            if (string.IsNullOrEmpty(sourceDir))
            {
                _options.SourceDir = Directory.GetCurrentDirectory();
            }
            else
            {
                _options.SourceDir = Path.GetFullPath(sourceDir);
            }

            return this;
        }

        public BuildScriptGeneratorOptionsBuilder WithDestinationDir(string destinationDir)
        {
            if (!string.IsNullOrEmpty(destinationDir))
            {
                _options.DestinationDir = Path.GetFullPath(destinationDir);
            }

            return this;
        }

        public BuildScriptGeneratorOptionsBuilder WithIntermediateDir(string intermediateDir)
        {
            if (!string.IsNullOrEmpty(intermediateDir))
            {
                _options.IntermediateDir = Path.GetFullPath(intermediateDir);
            }

            return this;
        }

        public BuildScriptGeneratorOptionsBuilder WithLanguageAndVersion(string language, string version)
        {
            _options.Language = language;
            _options.LanguageVersion = version;
            return this;
        }

        public BuildScriptGeneratorOptionsBuilder WithScriptOnly(bool scriptOnly)
        {
            _options.ScriptOnly = scriptOnly;
            return this;
        }

        public BuildScriptGeneratorOptionsBuilder WithProperties(string[] properties)
        {
            if (properties != null)
            {
                _options.Properties = ProcessProperties(properties);
            }

            return this;
        }

        public BuildScriptGeneratorOptions Build()
        {
            return _options;
        }

        // To enable testing
        internal static IDictionary<string, string> ProcessProperties(string[] properties)
        {
            var propertyList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in properties)
            {
                if (NameAndValuePairParser.TryParse(property, out var key, out var value))
                {
                    key = key.Trim('"');
                    value = value.Trim('"');

                    propertyList[key] = value;
                }
                else
                {
                    throw new InvalidUsageException($"Property key cannot start with '=' for property '{property}'.");
                }
            }

            return propertyList;
        }
    }
}

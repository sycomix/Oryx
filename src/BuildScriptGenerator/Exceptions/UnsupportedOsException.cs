// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

namespace Microsoft.Oryx.BuildScriptGenerator.Exceptions
{
    /// <summary>
    /// Current operating system is unsupported.
    /// </summary>
    public class UnsupportedOsException : InvalidUsageException
    {
        public UnsupportedOsException(string message)
            : base(message)
        {
        }
    }
}

// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Defines the various environments
/// </summary>
public enum Environment
{
    /// <summary>
    /// Localhost environment
    /// </summary>
    Development,

    /// <summary>
    /// Dogfood environment
    /// </summary>
    Dogfood,

    /// <summary>
    /// The dev environment
    /// </summary>
    Dev,

    /// <summary>
    /// CI environment
    /// </summary>
    Ci,

    /// <summary>
    /// Int environment
    /// </summary>
    Int,

    /// <summary>
    /// Perf environment
    /// </summary>
    Perf,

    /// <summary>
    /// Canary environment
    /// </summary>
    Canary,

    /// <summary>
    /// Production environment
    /// </summary>
    Prod
}

// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;

using Microsoft.Purview.DataGovernance.Common;

/// <summary>
/// Key vault configuration.
/// </summary>
public class KeyVaultConfiguration : AuthConfiguration
{
    /// <summary>
    /// Represents the base URL.
    /// </summary>
    public string BaseUrl { get; set; }
}

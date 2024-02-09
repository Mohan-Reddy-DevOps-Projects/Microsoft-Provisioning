// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Enumerates different states while performing creation
/// </summary>
public enum CreationType
{
    /// <summary>
    /// new resource was created
    /// </summary>
    NewResource,

    /// <summary>
    /// resource was updated
    /// </summary>
    Update
}

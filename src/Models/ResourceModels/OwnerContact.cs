// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using Newtonsoft.Json;

/// <summary>
/// Owner contact model.
/// </summary>
public class OwnerContact
{
    /// <summary>
    /// User Display Name
    /// </summary>
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    /// <summary>
    /// User ObjectId
    /// </summary>
    [JsonProperty("objectId")]
    public Guid ObjectId { get; set; }
}

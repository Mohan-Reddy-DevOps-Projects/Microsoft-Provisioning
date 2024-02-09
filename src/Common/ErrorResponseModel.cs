// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

using Newtonsoft.Json;

/// <summary>
/// An error returned from the service.
/// </summary>
public class ErrorResponseModel
{
    /// <summary>
    /// The error body.
    /// </summary>
    [JsonProperty(PropertyName = "error", Required = Required.Always)]
    public ErrorModel Error { get; set; }
}

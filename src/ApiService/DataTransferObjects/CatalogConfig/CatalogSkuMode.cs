// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum CatalogSkuMode
{
    /// <summary>
    /// feature is on
    /// </summary>
    On,

    /// <summary>
    /// feature is off
    /// </summary>
    Off,
}

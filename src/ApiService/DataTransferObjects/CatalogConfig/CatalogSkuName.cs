// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum CatalogSkuName
{
    /// <summary>
    /// catalog config sku is Basic
    /// </summary>
    Basic = 0,

    /// <summary>
    /// catalog config sku is Standard
    /// </summary>
    Standard,

    /// <summary>
    /// catalog config sku is Advanced
    /// </summary>
    Advanced,
}

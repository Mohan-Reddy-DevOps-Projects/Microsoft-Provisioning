// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class CatalogConfigModel : ICatalogConfigModel
{
    [JsonProperty("id")]
    [ReadOnly(true)]
    [Required]
    public Guid Id { get; init; }

    [JsonProperty("tenantId")]
    [Required]
    public Guid TenantId { get; set; }

    [JsonProperty("sku")]
    [Required]
    public CatalogSkuName Sku { get; set; }

    [JsonProperty("features")]
    [Required]
    public CatalogFeaturesModel Features { get; set; }

    [JsonProperty("createdAt")]
    [ReadOnly(true)]
    public DateTime CreatedAt { get; init; }

    [JsonProperty("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
}

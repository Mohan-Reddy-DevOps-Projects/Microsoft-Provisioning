// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;

public class CatalogConfig
{
    /// <summary>
    /// Id of catalog config
    /// </summary>
    [ReadOnly(true)]
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    #nullable enable
    public string? Id { get; internal set; }

    /// <summary>
    /// sku of catalog config.
    /// </summary>
    [JsonProperty("sku")]
    [Required]
    #nullable disable
    public string Sku { get; set; }

    /// <summary>
    /// features of catalog config.
    /// </summary>
    [JsonProperty("features")]
    [Required]
    #nullable disable
    public CatalogFeatures Features { get; set; }

    /// <summary>
    /// Created time of catalog config.
    /// </summary>
    [JsonProperty("createdAt", NullValueHandling = NullValueHandling.Ignore)]
    [ReadOnly(true)]
    public DateTime? CreatedAt { get; internal set; }

    /// <summary>
    /// Updated time of catalog config
    /// </summary>
    [JsonProperty("modifiedAt", NullValueHandling = NullValueHandling.Ignore)]
    [ReadOnly(true)]
    public DateTime? ModifiedAt { get; internal set; }
}

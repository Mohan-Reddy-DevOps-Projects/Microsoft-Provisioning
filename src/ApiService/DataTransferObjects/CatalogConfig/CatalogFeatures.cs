// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class CatalogFeatures
{
    /// <summary>
    /// Data estate health feature settings.
    /// </summary>
    [Required]
    [JsonProperty("dataEstateHealth")]
    public CatalogFeatureSettings DataEstateHealth { get; set; }

    /// <summary>
    /// Data quality feature settings.
    /// </summary>
    [Required]
    [JsonProperty("dataQuality")]
    public CatalogFeatureSettings DataQuality { get; set; }
}

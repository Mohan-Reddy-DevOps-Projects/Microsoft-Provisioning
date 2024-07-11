// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class CatalogFeaturesModel : ICatalogFeaturesModel
{
    [Required]
    [JsonProperty("dataEstateHealth")]
    public CatalogFeatureSettingsModel DataEstateHealth { get; set; }

    [Required]
    [JsonProperty("dataQuality")]
    public CatalogFeatureSettingsModel DataQuality { get; set; }
}

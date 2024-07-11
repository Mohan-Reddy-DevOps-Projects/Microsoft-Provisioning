// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class CatalogFeatureSettingsModel : ICatalogFeatureSettingsModel
{
    [Required]
    [JsonProperty("mode")]
    public CatalogSkuMode Mode { get; set; }
}

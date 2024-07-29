// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class CatalogFeatureSettings
{
	/// <summary>
	/// mode of the catalog feature settings.
	/// </summary>
	[Required]
    [JsonProperty("mode")]
    public string Mode { get; set; }
}

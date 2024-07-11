// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;

using Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Adapter to convert CatalogFeaturesSettings between model and DTO.
/// </summary>
internal static class CatalogFeaturesSettingsAdapter
{
    /// <inheritdoc />
    public static CatalogFeatureSettings FromModel(CatalogFeatureSettingsModel model)
    {
        return new CatalogFeatureSettings
        {
            Mode = model.Mode.ToDto(),
        };
    }

    /// <inheritdoc />
    public static CatalogFeatureSettingsModel ToModel(CatalogFeatureSettings catalogFeatureSettings)
    {
        return new CatalogFeatureSettingsModel
        {
            Mode = catalogFeatureSettings.Mode.ToModel(),
        };
    }
}

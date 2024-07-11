namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;

using Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Adapter to convert CatalogFeatures between model and DTO.
/// </summary>
internal static class CatalogFeaturesAdapter
{ 
    public static CatalogFeatures FromModel(CatalogFeaturesModel model)
    {
        return new CatalogFeatures
        {
            DataEstateHealth = CatalogFeaturesSettingsAdapter.FromModel(model.DataEstateHealth),
            DataQuality = CatalogFeaturesSettingsAdapter.FromModel(model.DataQuality),
        };
    }

    public static CatalogFeaturesModel ToModel(CatalogFeatures catalogFeaturesPayload)
    {
        return new CatalogFeaturesModel()
        {
            DataEstateHealth = CatalogFeaturesSettingsAdapter.ToModel(catalogFeaturesPayload.DataEstateHealth),
            DataQuality = CatalogFeaturesSettingsAdapter.ToModel(catalogFeaturesPayload.DataQuality),
        };
    }
}

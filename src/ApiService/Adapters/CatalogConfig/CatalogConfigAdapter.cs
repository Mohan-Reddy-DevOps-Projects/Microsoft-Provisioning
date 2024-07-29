// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Catalog Config Adapter
/// </summary>
internal class CatalogConfigAdapter
{
    public CatalogConfig FromModel(CatalogConfigModel model)
    {
        return new CatalogConfig
        {
            Id = model.Id.ToString(),
            Sku = model.Sku.ToDto(),
            Features = CatalogFeaturesAdapter.FromModel(model.Features),
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
        };
    }

    public CatalogConfigModel ToModel(CatalogConfig catalogConfigPayload)
    {
        return new CatalogConfigModel()
        {
            Sku = catalogConfigPayload.Sku.ToModel(),
            Features = CatalogFeaturesAdapter.ToModel(catalogConfigPayload.Features)
        };
    }
}

// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;
using Microsoft.Purview.DataGovernance.Provisioning.ApiService.DataTransferObjects;
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
            Sku = model.Sku.ToString(),
            Features = CatalogFeaturesAdapter.FromModel(model.Features),
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
        };
    }

    public CatalogConfigModel ToModel(CatalogConfig catalogConfigPayload)
    {
        if (Enum.TryParse<CatalogSkuName>(catalogConfigPayload.Sku, true, out var sku))
        {
            return new CatalogConfigModel()
            {
                Id = catalogConfigPayload.Id != null ? Guid.Parse(catalogConfigPayload.Id) : Guid.NewGuid(),
                Sku = sku,
                Features = CatalogFeaturesAdapter.ToModel(catalogConfigPayload.Features),
                CreatedAt = catalogConfigPayload.CreatedAt ?? DateTime.UtcNow,
                ModifiedAt = catalogConfigPayload.ModifiedAt ?? DateTime.UtcNow,
            };
        }
        else
        {
            throw new ArgumentException("Invalid sku value: " + catalogConfigPayload.Sku);
        }
       
    }
}

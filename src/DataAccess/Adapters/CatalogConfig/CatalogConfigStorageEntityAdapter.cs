// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System.Text.Json;
using global::Azure;
using global::Microsoft.Purview.DataGovernance.Provisioning.DataAccess.EntityModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

internal sealed class CatalogConfigStorageEntityAdapter : TableEntityConverter<CatalogConfigModel, CatalogConfigStorageEntity>
{
    public static string DefaultRowKey = "default";
    public override CatalogConfigStorageEntity ToEntity(CatalogConfigModel model, string accountId)
    {
        return new CatalogConfigStorageEntity()
        {
            ETag = ETag.All,
            Id = model.Id,
            TenantId = model.TenantId,
            PartitionKey = accountId,
            Sku = JsonSerializer.Serialize(model.Sku),
            Features = JsonSerializer.Serialize(model.Features),
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            RowKey = DefaultRowKey,
        };
    }

    public override CatalogConfigModel ToModel(CatalogConfigStorageEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new CatalogConfigModel()
        {
            TenantId = entity.TenantId,
            Id = entity.Id,
            Sku = JsonSerializer.Deserialize<CatalogSkuName>(entity.Sku),
            Features = JsonSerializer.Deserialize<CatalogFeaturesModel>(entity.Features),
            CreatedAt = entity.CreatedAt,
            ModifiedAt = entity.ModifiedAt,
        };
    }
}


// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System.Text.Json;
using global::Azure;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess.EntityModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

internal sealed class ProcessingStorageEntityAdapter : TableEntityConverter<ProcessingStorageModel, ProcessingStorageEntity>
{
    public override ProcessingStorageEntity ToEntity(ProcessingStorageModel model, string accountId)
    {
        return new ProcessingStorageEntity()
        {
            ETag = ETag.All,
            Id = model.Id,
            PartitionKey = accountId,
            Properties = JsonSerializer.Serialize(model.Properties),
            RowKey = model.Name,
            TenantId = model.TenantId.ToString(),
            CatalogId = model.CatalogId.ToString(),
        };
    }

    public override ProcessingStorageModel ToModel(ProcessingStorageEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new ProcessingStorageModel()
        {
            AccountId = Guid.Parse(entity.PartitionKey),
            Id = entity.Id,
            Name = entity.RowKey,
            Properties = JsonSerializer.Deserialize<ProcessingStoragePropertiesModel>(entity.Properties),
            TenantId = Guid.Parse(entity.TenantId),
            CatalogId = Guid.Parse(entity.CatalogId)
        };
    }
}


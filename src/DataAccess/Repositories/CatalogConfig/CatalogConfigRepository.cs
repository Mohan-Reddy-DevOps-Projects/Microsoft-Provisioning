// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess.EntityModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Extensions.Options;

internal class CatalogConfigRepository : ICatalogConfigRepository 
{ 
    private readonly ITableStorageClient<CatalogConfigTableConfiguration> tableStorageClient;
    private readonly CatalogConfigStorageEntityAdapter converter = new CatalogConfigStorageEntityAdapter();
    private readonly CatalogConfigTableConfiguration tableConfiguration;

    public CatalogConfigRepository(ITableStorageClient<CatalogConfigTableConfiguration> tableStorageClient, IOptions<CatalogConfigTableConfiguration> tableConfiguration)
    {
        this.tableStorageClient = tableStorageClient;
        this.tableConfiguration = tableConfiguration.Value;
    }

    public async Task<CatalogConfigModel> Create(string accountId, CatalogConfigModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        CatalogConfigStorageEntity entity = this.converter.ToEntity(model, accountId);
        await this.tableStorageClient.AddEntityAsync(this.tableConfiguration.TableName, entity, cancellationToken);

        return this.converter.ToModel(entity);
    }

    public async Task<CatalogConfigModel> GetSingle(string accountId, CancellationToken cancellationToken)
    {
        CatalogConfigStorageEntity existingStorageEntity = await this.tableStorageClient.GetEntityIfExistsAsync<CatalogConfigStorageEntity>(this.tableConfiguration.TableName, accountId, CatalogConfigStorageEntityAdapter.DefaultRowKey, cancellationToken);

        return this.converter.ToModel(existingStorageEntity);
    }

    public async Task<CatalogConfigModel> Update(string accountId, CatalogConfigModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        CatalogConfigStorageEntity entity = this.converter.ToEntity(model, accountId);
        await this.tableStorageClient.UpdateEntityAsync(this.tableConfiguration.TableName, entity, cancellationToken);
        return this.converter.ToModel(entity);
    }

    public async Task Delete(string accountId, CancellationToken cancellationToken)
    {
        await this.tableStorageClient.DeleteEntityAsync(this.tableConfiguration.TableName, accountId, CatalogConfigStorageEntityAdapter.DefaultRowKey, cancellationToken);
    }
}

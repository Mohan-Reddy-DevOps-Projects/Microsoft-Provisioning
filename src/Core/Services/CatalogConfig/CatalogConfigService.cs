// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using System;
using System.Threading.Tasks;
using Microsoft.Purview.DataGovernance.Loggers;
using global::Azure;
using System.Net;
using Microsoft.Purview.DataGovernance.Provisioning.Common;

public class CatalogConfigService : ICatalogConfigService
{
    private readonly ICatalogConfigRepository catalogConfigRepository;
    private readonly IServiceRequestLogger logger;

    public CatalogConfigService(ICatalogConfigRepository catalogConfigRepository, IServiceRequestLogger logger)
    {
        this.catalogConfigRepository = catalogConfigRepository;
        this.logger = logger;
    }

    public async Task<CatalogConfigModel> CreateCatalogConfigAsync(string accountId, string tenantId, CancellationToken cancellation)
    {
        DateTime now = DateTime.UtcNow;
        var catalogConfigModel = new CatalogConfigModel()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.Parse(tenantId),
            Sku = CatalogSkuName.Basic,
            Features = new CatalogFeaturesModel()
            {
                DataEstateHealth = new CatalogFeatureSettingsModel()
                {
                    Mode = CatalogSkuMode.On,
                },
                DataQuality = new CatalogFeatureSettingsModel()
                {
                    Mode = CatalogSkuMode.On,
                }
            },
            CreatedAt = now,
            ModifiedAt = now,
        };
        return await this.catalogConfigRepository.Create(accountId, catalogConfigModel, cancellation).ConfigureAwait(false);
    }

    public async Task DeleteCatalogConfigAsync(string accountId, CancellationToken cancellationToken)
    {
        try
        {
            await this.catalogConfigRepository.Delete(accountId, cancellationToken).ConfigureAwait(false);
        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
        {
            this.logger.LogWarning($"Catalog config already deleted for accout: {accountId}");
        }
    }

    public async Task<CatalogConfigModel> GetCatalogConfigAsync(string accountId, CancellationToken cancellationToken)
    {
        CatalogConfigModel model = await this.catalogConfigRepository.GetSingle(accountId, cancellationToken).ConfigureAwait(false);
        if (model == null)
        {
            throw new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.Config_GetConfigError,
                "Catalog config not found"
              ).ToException();
        }
        return model;
    }

    public async Task<CatalogConfigModel> SetCatalogConfigAsync(string accountId, CatalogConfigModel model, CancellationToken cancellationToken)
    {
        CatalogConfigModel currentModel = await this.catalogConfigRepository.GetSingle(accountId, cancellationToken).ConfigureAwait(false);
        if (currentModel == null)
        {
            throw new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.Config_GetConfigError,
                "Catalog config not found"
              ).ToException();
        }
        CatalogConfigModel updatedModel = new CatalogConfigModel()
        {
            Id = currentModel.Id,
            TenantId = currentModel.TenantId,
            CreatedAt = currentModel.CreatedAt,
            Sku = model.Sku,
            Features = model.Features,
            ModifiedAt = DateTime.UtcNow,
        };
        return await this.catalogConfigRepository.Update(accountId, updatedModel, cancellationToken).ConfigureAwait(false);
    }
}
